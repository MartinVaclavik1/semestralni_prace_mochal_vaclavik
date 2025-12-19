using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using semestralni_prace_mochal_vaclavik.OracleConn;
using semestralni_prace_mochal_vaclavik.Services;
using semestralni_prace_mochal_vaclavik.Tridy;
using semestralni_prace_mochal_vaclavik.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace semestralni_prace_mochal_vaclavik.Repository
{
    public class PrihlasenyUzivatelRepository
    {
        private readonly IOracleConnectionFactory connectionFactory;

        public PrihlasenyUzivatelRepository(IOracleConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public Uzivatel Prihlas(string uzivatelskeJmeno, string heslo)
        {

            using var conn = connectionFactory.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                        select * from datauctuview where id = 
                        (select prihlaseni(:prihlJmeno,:heslo) from Dual)";

            cmd.Parameters.Add(new OracleParameter("prihlJmeno", uzivatelskeJmeno));
            cmd.Parameters.Add(new OracleParameter("heslo", heslo));

            using var reader = cmd.ExecuteReader();

            Uzivatel uzivatel = new();

            if (reader.Read())
            {

                uzivatel.Id = reader.GetInt32("id");
                uzivatel.Username = reader.GetString("prihlasovacijmeno");
                var blob = reader.GetOracleBlob(2);
                if (!blob.IsNull)
                {
                    uzivatel.ObrazekBytes = nactiByteZBLOB(blob);
                    uzivatel.Obrazek = vytvorObrazek(uzivatel.ObrazekBytes);
                }


                uzivatel.Opravneni = reader.GetString("nazevopravneni");

                if (uzivatel.Opravneni == "obcan")
                {
                    uzivatel.Jmeno = reader.IsDBNull("o_jmeno") ? "" : reader.GetString("o_jmeno");
                    uzivatel.Prijmeni = reader.IsDBNull("o_prijmeni") ? "" : reader.GetString("o_prijmeni");

                }
                else
                {
                    uzivatel.Jmeno = reader.IsDBNull("p_jmeno") ? "" : reader.GetString("p_jmeno");
                    uzivatel.Prijmeni = reader.IsDBNull("p_prijmeni") ? "" : reader.GetString("p_prijmeni");
                }

                return uzivatel;

            }
            else
            {
                throw new Exception("Uživatel nenalezen");
            }

        }

        public void Registrovat(Registrace novaRegistrace)
        {
            using var conn = connectionFactory.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"vytvor_uzivatele_obcana";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;

            cmd.Parameters.Add("p_prihlasovacijmeno", OracleDbType.Varchar2).Value = novaRegistrace.Username;
            cmd.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = novaRegistrace.Heslo;
            cmd.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = novaRegistrace.Jmeno;
            cmd.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = novaRegistrace.Prijmeni;

            cmd.Parameters.Add("p_cisloop", OracleDbType.Decimal).Value = Convert.ToInt32(novaRegistrace.CisloOP);
            cmd.Parameters.Add("p_psc", OracleDbType.Char, 5).Value = novaRegistrace.PSC;
            cmd.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = novaRegistrace.Ulice;
            cmd.Parameters.Add("p_cislopopisne", OracleDbType.Decimal).Value = Convert.ToInt32(novaRegistrace.CisloPopisne);

            cmd.Parameters.Add("p_obec", OracleDbType.Varchar2).Value = novaRegistrace.Obec;
            cmd.Parameters.Add("p_zeme", OracleDbType.Varchar2).Value = novaRegistrace.Zeme;

            cmd.ExecuteNonQuery();

            conn.Commit();

        }

        public void AktualizujUcet(Uzivatel uzivatel)
        {
            using var conn = connectionFactory.CreateConnection();
            conn.Open();

            using (OracleCommand cmdUcet = new OracleCommand("aktualizuj_ucet", conn))
            {
                cmdUcet.CommandType = CommandType.StoredProcedure;
                cmdUcet.BindByName = true;
                cmdUcet.Parameters.Add("p_id", OracleDbType.Int32).Value = uzivatel.Id;
                cmdUcet.Parameters.Add("p_prihlasovacijmeno", OracleDbType.Varchar2).Value = uzivatel.Username;
                cmdUcet.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = uzivatel.Password;
                cmdUcet.Parameters.Add("p_obrazek", OracleDbType.Blob).Value = uzivatel.ObrazekBytes;
                cmdUcet.ExecuteNonQuery();
            }
            conn.Commit();


            string volanaMetoda;
            if (uzivatel.Opravneni == "obcan")
            {
                volanaMetoda = "aktualizuj_jmeno_prijmeni_obcana";
            }
            else
            {
                volanaMetoda = "aktualizuj_jmeno_prijmeni_policisty";
            }

            using (OracleCommand cmdJmenoPrijmeni = new OracleCommand(volanaMetoda, conn))
            {
                cmdJmenoPrijmeni.CommandType = CommandType.StoredProcedure;
                cmdJmenoPrijmeni.BindByName = true;

                // p_idUzivatele number, p_jmeno varchar2, p_prijmeni varchar2
                cmdJmenoPrijmeni.Parameters.Add("p_idUzivatele", OracleDbType.Int32).Value = uzivatel.Id;
                cmdJmenoPrijmeni.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = uzivatel.Jmeno;
                cmdJmenoPrijmeni.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = uzivatel.Prijmeni;

                cmdJmenoPrijmeni.ExecuteNonQuery();
            }
            conn.Commit();
        }

        /// <summary>
        /// Načítá binární data z Oracle BLOB objektu do pole bajtů.
        /// </summary>
        /// <param name="imgBlob">Oracle BLOB objekt s daty obrázku</param>
        /// <returns>
        /// Pole bajtů obsahující obsah BLOBu, nebo null pokud je BLOB prázdný
        /// </returns>
        public byte[] nactiByteZBLOB(OracleBlob imgBlob)
        {
            if (imgBlob == null)
                return null;

            byte[] imgBytes = new byte[imgBlob.Length];
            imgBlob.Read(imgBytes, 0, (int)imgBlob.Length);

            return imgBytes;
        }

        /// <summary>
        /// Konvertuje binární data obrázku na BitmapImage pro zobrazení v UI.
        /// </summary>
        /// <param name="imageBytes">Pole bajtů s daty obrázku</param>
        /// <returns>BitmapImage objekt připravený k zobrazení</returns>
        /// <remarks>
        /// Obrázek je "zmrazen" (Freeze) pro optimalizaci výkonu v WPF.
        /// </remarks>
        public BitmapImage vytvorObrazek(byte[] imageBytes)
        {
            BitmapImage img = new BitmapImage();
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = stream;
                img.EndInit();
                img.Freeze();
            }
            return img;
        }

    }
}
