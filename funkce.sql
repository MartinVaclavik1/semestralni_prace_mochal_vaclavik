create or replace trigger vytvoreni_uctu_pro_policistu
before insert on POLICISTE
for each row follows policiste_idpolicisty_trg
DECLARE
    i_iduzivatele NUMBER;
    i_opravneni NUMBER;
BEGIN
    if(:new.iduzivatele is null) then
        i_iduzivatele := uzivatele_iduzivatele_seq.NEXTVAL;
    
        select idopravneni into i_opravneni from opravneni where nazevopravneni = 'policista';
    
        INSERT INTO uzivatele (iduzivatele, prihlasovacijmeno, heslo, idpolicisty, idobcana, idopravneni)
        VALUES (i_iduzivatele, generovat_jmeno(:NEW.jmeno) , generovat_heslo(), :new.idpolicisty, null, i_opravneni);
        
        :NEW.iduzivatele := i_iduzivatele;
    end if;
end;
/

create or replace trigger vytvoreni_uctu_pro_obcana 
before insert on OBCANE 
for each row follows obcane_idobcana_trg
DECLARE
    i_iduzivatele NUMBER;
    i_opravneni NUMBER;
BEGIN
    if(:new.iduzivatele is null) then
        i_iduzivatele := uzivatele_iduzivatele_seq.NEXTVAL;
        
        select idopravneni into i_opravneni from opravneni where nazevopravneni = 'obcan';
    
        INSERT INTO uzivatele (iduzivatele, prihlasovacijmeno, heslo, idpolicisty, idobcana, idopravneni)
        VALUES (i_iduzivatele, generovat_jmeno(:NEW.jmeno),generovat_heslo() , null, :NEW.idobcana, i_opravneni);
        
        :NEW.iduzivatele := i_iduzivatele;
    end if;
end;
/

CREATE OR REPLACE FUNCTION generovat_jmeno(
    p_jmeno IN VARCHAR2
)
RETURN VARCHAR2
AS
    -- Nastavte velikost, kterou vrac?te
    o_vysledek VARCHAR2(20); 
    
    -- Definice rozsahu
    v_min_cislo CONSTANT NUMBER := 10000;
    v_max_cislo CONSTANT NUMBER := 99999;
    
    v_random_cislo NUMBER;
BEGIN

    -- 2. Generov?n? n?hodn?ho ??sla v rozsahu 10000 a? 99999
    -- DBMS_RANDOM.VALUE(low, high) vrac? ??slo >= low a < high
    v_random_cislo := TRUNC(DBMS_RANDOM.VALUE(v_min_cislo, v_max_cislo + 1));
    
    -- 3. Spojen? ??sla a jm?na a p?i?azen? do v?stupn? prom?nn?
    o_vysledek := p_jmeno || v_random_cislo;

    -- 4. Vr?cen? v?sledku
    RETURN o_vysledek;
END;
/

CREATE OR REPLACE FUNCTION generovat_heslo
RETURN VARCHAR2
AS
    -- Nastavte velikost, kterou vrac?te
    o_vysledek VARCHAR2(20); 
    
    -- Definice rozsahu
    v_min_cislo CONSTANT NUMBER := 10000;
    v_max_cislo CONSTANT NUMBER := 99999;
    
    v_random_cislo NUMBER;
BEGIN
    -- 2. Generov?n? n?hodn?ho ??sla v rozsahu 10000 a? 99999
    -- DBMS_RANDOM.VALUE(low, high) vrac? ??slo >= low a < high
    v_random_cislo := TRUNC(DBMS_RANDOM.VALUE(v_min_cislo, v_max_cislo + 1));
    
    -- 3. Spojen? ??sla a jm?na a p?i?azen? do v?stupn? prom?nn?
    o_vysledek := v_random_cislo;

    -- 4. Vr?cen? v?sledku
    RETURN o_vysledek;
END;
/

create or replace FUNCTION prihlaseni(prihlJmeno in VARCHAR2, zadaneHeslo in VARCHAR2)
return NUMERIC
as
    i_idUzivatele NUMERIC;

begin
select iduzivatele into i_idUzivatele from uzivatele
where LOWER(prihlasovacijmeno) = LOWER(prihlJmeno) 
and heslo = zadaneHeslo;
return i_iduzivatele;
end;
/

--select prihlaseni('wallisdate','Heslo123') from Dual;

create or replace view datauctuview
as
    select u.iduzivatele id , u.prihlasovacijmeno, u.obrazek, o.nazevopravneni, ob.jmeno o_jmeno, ob.prijmeni o_prijmeni, p.jmeno p_jmeno, p. prijmeni p_prijmeni from uzivatele u
    left join opravneni o using(idopravneni)
    left join obcane ob using(idobcana)
    left join policiste p using(idpolicisty);

--select * from datauctuview;

create or replace trigger adresy_log
after insert or update or delete on adresy
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idadresy || ', ' || :new.postovnismerovacicislo || ', ' || :new.ulice || ', ' || :new.cislopopisne || ', ' || :new.obec || ', ' || :new.zeme;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idadresy || ', ' || :old.postovnismerovacicislo || ', ' || :old.ulice || ', ' || :old.cislopopisne || ', ' || :old.obec || ', ' || :old.zeme;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('adresy', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger automobily_log
after insert or update or delete on automobily
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idprostredku || ', ' || :new.poznavaciznacka;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idprostredku || ', ' || :old.poznavaciznacka;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('automobily', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger barvy_log
after insert or update or delete on barvy
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idbarvy || ', ' || :new.nazev;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idbarvy || ', ' || :old.nazev;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('barvy', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger dopravni_prostredky_log
after insert or update or delete on dopravni_prostredky
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idprostredku || ', ' || :new.idbarvy || ', ' || :new.idpolicisty || ', ' || :new.discriminator || ', ' || :new.poznamka;
    end if;
    
    if updating or deleting then
       i_old := i_old || :new.idprostredku || ', ' || :old.idbarvy || ', ' || :old.idpolicisty || ', ' || :old.discriminator || ', ' || :old.poznamka;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('dopravni_prostredky', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger evidence_hlidek_log
after insert or update or delete on evidence_hlidek
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idhlidky || ', ' || :new.idpolicisty || ', ' || :new.datum || ', ' || to_char(:new.casod, 'HH24:MI') || ', ' || to_char(:new.casdo, 'HH24:MI');
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idhlidky || ', ' || :old.idpolicisty || ', ' || :old.datum || ', ' || to_char(:old.casod, 'HH24:MI') || ', ' || to_char(:old.casdo, 'HH24:MI');
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('evidence_hlidek', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger evidence_r_prukazu_log
after insert or update or delete on evidence_r_prukazu
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idtypu || ', ' || :new.cislorp;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idtypu || ', ' || :old.cislorp;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('evidence_r_prukazu', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger evidence_zasahu_log
after insert or update or delete on evidence_zasahu
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idzasahu || ', ' || :new.idhlidky;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idzasahu || ', ' || :old.idhlidky;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('evidence_zasahu', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger hlidky_log
after insert or update or delete on hlidky
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idhlidky || ', ' || :new.nazevhlidky || ', ' || :new.idtypu;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idhlidky || ', ' || :old.nazevhlidky || ', ' || :old.idtypu;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('hlidky', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger hodnosti_log
after insert or update or delete on hodnosti
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idhodnosti || ', ' || :new.nazev;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idhodnosti || ', ' || :old.nazev;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('hodnosti', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger komunikace_log
after insert or update or delete on komunikace
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idkomunikace || ', ' || :new.zprava  || ', ' || :new.datum || ', ' || :new.poznamka || ', ' || :new.idobcana || ', ' || :new.idprestupku;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idkomunikace || ', ' || :old.zprava  || ', ' || :old.datum || ', ' || :old.poznamka || ', ' || :old.idobcana || ', ' || :old.idprestupku;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('komunikace', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger kone_log
after insert or update or delete on kone
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idprostredku || ', ' || :new.jmeno  || ', ' || :new.datumnarozeni;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idprostredku || ', ' || :old.jmeno  || ', ' || :old.datumnarozeni;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('kone', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger motocykly_log
after insert or update or delete on motocykly
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idprostredku || ', ' || :new.poznavaciznacka;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idprostredku || ', ' || :old.poznavaciznacka;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('motocykly', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger obcane_log
after insert or update or delete on obcane
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idobcana || ', ' || :new.jmeno || ', ' || :new.prijmeni || ', ' || :new.cisloop || ', ' || :new.poznamka || ', ' || :new.idadresy || ', ' || :new.iduzivatele;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idobcana || ', ' || :old.jmeno || ', ' || :old.prijmeni || ', ' || :old.cisloop || ', ' || :old.poznamka || ', ' || :old.idadresy || ', ' || :old.iduzivatele;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('obcane', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger okrsky_log
after insert or update or delete on okrsky
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idokrsku || ', ' || :new.nazev;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idokrsku || ', ' || :old.nazev;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('okrsky', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger okrsky_hlidky_log
after insert or update or delete on okrsky_hlidky
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idhlidky || ', ' || :new.idokrsku;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idhlidky || ', ' || :old.idokrsku;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('okrsky_hlidky', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger opravneni_log
after insert or update or delete on opravneni
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idopravneni || ', ' || :new.nazevopravneni;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idopravneni || ', ' || :old.nazevopravneni;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('opravneni', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger policejni_stanice_log
after insert or update or delete on policejni_stanice
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idstanice || ', ' || :new.nazev || ', ' || :new.poznamka;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idstanice || ', ' || :old.nazev|| ', ' || :old.poznamka;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('policejni_stanice', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger policiste_log
after insert or update or delete on policiste
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idpolicisty || ', ' || :new.jmeno || ', ' || :new.prijmeni || ', ' || :new.datumnarozeni || ', ' || :new.plat || ', ' || :new.idstanice || ', ' || :new.idhodnosti || ', ' || :new.poznamka || ', ' || :new.iduzivatele;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idpolicisty || ', ' || :old.jmeno || ', ' || :old.prijmeni || ', ' || :old.datumnarozeni || ', ' || :old.plat || ', ' || :old.idstanice || ', ' || :old.idhodnosti || ', ' || :old.poznamka || ', ' || :old.iduzivatele;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('policiste', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger prestupky_log
after insert or update or delete on prestupky
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idprestupku || ', ' || :new.idzasahu || ', ' || :new.idtypuprestupku || ', ' || :new.poznamka;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idprestupku || ', ' || :old.idzasahu || ', ' || :old.idtypuprestupku || ', ' || :old.poznamka;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('prestupky', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger prestupky_obcanu_log
after insert or update or delete on prestupky_obcanu
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idobcana || ', ' || :new.idprestupku || ', ' || :new.datum;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idobcana || ', ' || :old.idprestupku || ', ' || :old.datum;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('prestupky_obcanu', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger ridicske_prukazy_log
after insert or update or delete on ridicske_prukazy
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.cislorp || ', ' || :new.platnostdo || ', ' || :new.idpolicisty;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.cislorp || ', ' || :old.platnostdo || ', ' || :old.idpolicisty;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('ridicske_prukazy', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger typy_hlidky_log
after insert or update or delete on typy_hlidky
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idtypu || ', ' || :new.nazev;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idtypu || ', ' || :old.nazev;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('typy_hlidky', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger typy_prestupku_log
after insert or update or delete on typy_prestupku
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idtypuprestupku || ', ' || :new.prestupek;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idtypuprestupku || ', ' || :old.prestupek;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('typy_prestupku', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger typy_ridicskeho_prukazu_log
after insert or update or delete on typy_ridicskeho_prukazu
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idtypu || ', ' || :new.typ;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idtypu || ', ' || :old.typ;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('typy_ridicskeho_prukazu', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger uzivatele_log
after insert or update or delete on uzivatele
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.iduzivatele || ', ' || :new.prihlasovacijmeno || ', ' || :new.heslo || ', ' || :new.idpolicisty || ', ' || :new.idobcana || ', ' || :new.idopravneni;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.iduzivatele || ', ' || :old.prihlasovacijmeno || ', ' || :old.heslo || ', ' || :old.idpolicisty || ', ' || :old.idobcana || ', ' || :old.idopravneni;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('uzivatele', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace trigger zasahy_log
after insert or update or delete on zasahy
for each row
declare
    i_akce varchar2(20);
    i_old varchar2(4000) := ' ';
    i_new varchar2(4000) := ' ';
begin 
    if inserting or updating then
        i_new := i_new || :new.idzasahu || ', ' || :new.datum || ', ' || to_char(:new.cas, 'HH24:MI:SS') || ', ' || :new.popis || ', ' || :new.idadresy;
    end if;
    
    if updating or deleting then
       i_old := i_old || :old.idzasahu || ', ' || :old.datum || ', ' || to_char(:old.cas, 'HH24:MI:SS') || ', ' || :old.popis || ', ' || :old.idadresy;
    end if;
    
    CASE
            WHEN INSERTING THEN 
                i_akce := 'INSERT';
            WHEN UPDATING  THEN 
                i_akce := 'UPDATE';
            WHEN DELETING  THEN 
                i_akce := 'DELETE';
    end case;
        
    insert into logovaci_tabulka(nazevtabulky, akce, old, new, datum) values('zasahy', i_akce, i_old, i_new, sysdate);
    
end;
/

create or replace view logovaci_tabulkaview as
select nazevtabulky, akce, old, new, to_char(datum, 'DD.MM.YYYY HH24:MI:SS') datum from logovaci_tabulka;

create or replace view prestupkyview as
select o.idobcana, t.prestupek, po.datum, o.jmeno || ' ' || o.prijmeni jmenoobcana, p.poznamka  from prestupky p
    join prestupky_obcanu po on(po.idprestupku = p.idprestupku)
    join typy_prestupku t on(t.idtypuprestupku = p.idtypuprestupku)
    join obcane o on(o.idobcana = po.idobcana);
    
create or replace view hlidkyview as
select h.nazevhlidky, t.nazev from hlidky h
    join typy_hlidky t using(idtypu);
    
--nem·me pr·va na dbms_crypto => nefunguje
--create or replace function get_hash (p_username  IN  VARCHAR2,
--                     p_password  IN  VARCHAR2)
--RETURN VARCHAR2 AS
--    l_salt VARCHAR2(30) := 'ASFfhuewjsa24Ëö'; 
--BEGIN
--    RETURN DBMS_CRYPTO.HASH(UTL_RAW.CAST_TO_RAW(UPPER(p_username) || l_salt || UPPER(p_password)),DBMS_CRYPTO.HASH_SH1);
--END;

create or replace procedure zmen_heslo(p_prihlasovacijmeno varchar2,
    p_nove_heslo varchar2)
as
begin
    update uzivatele set heslo = p_nove_heslo where prihlasovacijmeno = p_prihlasovacijmeno;
end;
/

create or replace procedure vytvor_uzivatele_obcana(p_prihlasovacijmeno varchar2, p_heslo varchar2,
    p_jmeno varchar2, p_prijmeni varchar2, p_cisloop number, p_psc char, p_ulice varchar2, p_cislopopisne number, p_obec varchar2, p_zeme varchar2)
as
    i_idadresy number;
    i_idopravneniuzivatele number;
    i_idobcana number;
    i_iduzivatele number;
begin
    
    insert into adresy (postovnismerovacicislo, ulice, cislopopisne, obec, zeme) values (p_psc, p_ulice, p_cislopopisne, p_obec, p_zeme)
    returning idadresy into i_idadresy;
    
    select idopravneni into i_idopravneniuzivatele from opravneni where nazevopravneni = 'obcan';
    
    insert into uzivatele(prihlasovacijmeno, heslo, idobcana, idopravneni) values(p_prihlasovacijmeno, p_heslo, null, i_idopravneniuzivatele)
    returning iduzivatele into i_iduzivatele;
    
    insert into obcane (jmeno, prijmeni, cisloop, idadresy, iduzivatele) values (p_jmeno, p_prijmeni, p_cisloop, i_idadresy, i_iduzivatele)
    returning idobcana into i_idobcana;
    
    update uzivatele set idobcana = i_idobcana where iduzivatele = i_iduzivatele;

end;
/

create or replace procedure aktualizuj_ucet(p_id number, p_prihlasovacijmeno varchar2, p_heslo varchar2, p_obrazek blob)
as
    cursor c_heslo is select heslo from uzivatele where iduzivatele = p_id;
    i_heslo uzivatele.heslo%type;
begin
    OPEN c_heslo;

    LOOP
        FETCH c_heslo INTO i_heslo;
        EXIT WHEN c_heslo%NOTFOUND;
    END LOOP;

    CLOSE c_heslo;
    
    if i_heslo = p_heslo then
        update uzivatele set prihlasovacijmeno = p_prihlasovacijmeno, obrazek = p_obrazek where iduzivatele = p_id;
    else
        RAISE_APPLICATION_ERROR(-20000, 'äpatnÈ heslo');
    end if;
end;
/

create or replace view systemovy_katalogView as
select * from ALL_OBJECTS where owner = (select user from dual);

create or replace function ziskejJmenoVedouciho(p_id number)
return varchar2
as
    i_jmeno varchar2(100);
begin 
    SELECT 
           jmeno || ' ' || prijmeni into i_jmeno
    FROM policiste
    where level = 2
    START WITH idpolicisty = p_id
    CONNECT BY PRIOR idnadrizeneho = idpolicisty;

    return i_jmeno;
end;
/

create or replace view kontaktyView as
SELECT
    p.jmeno AS Jmeno,
    p.prijmeni AS Prijmeni,
    h.nazev AS Hodnost,
    ziskejJmenoVedouciho(p.idpolicisty) as Nadrizeny,
    s.nazev AS Stanice
FROM 
    policiste p
INNER JOIN 
    hodnosti h ON p.idhodnosti = h.idhodnosti
INNER JOIN 
    policejni_stanice s ON p.idstanice = s.idstanice
ORDER BY 
    p.prijmeni, h.nazev;

create or replace function zjistiPocetVyresenychPrestupkuPolicistyVRozmezi(p_idpolicisty number, p_datumOd date, p_datumDo date)
return number
as
    i_pocet number;
begin

    select count(*) into i_pocet from prestupky pr
    join zasahy z on(pr.idzasahu = z.idzasahu)
    join evidence_zasahu ez on(z.idzasahu = ez.idzasahu)
    join evidence_hlidek eh on(ez.idhlidky = eh.idhlidky)
    join policiste p on(eh.idpolicisty = p.idpolicisty)
    where eh.datum between p_datumOd and p_datumDo
    and p.idpolicisty = p_idpolicisty
    group by p.idpolicisty;

    return i_pocet;
    
    exception when no_data_found then
        return 0;
end;
/
--vypocitejVyplatyZaMesicRok
create or replace function vypocitejVyplatuPolicistoviZaMesicRok(p_idpolicisty number, mesic varchar2, rok varchar2)
return number
as
    i_pocetPrestupku number;
    i_prvniDen date;
    i_plat number; 
begin
    select plat into i_plat from policiste where idpolicisty = p_idpolicisty;
    i_prvniDen := to_date('1.'  || mesic || '.' || rok);
    i_pocetPrestupku := zjistiPocetVyresenychPrestupkuPolicistyVRozmezi(p_idpolicisty,i_prvniDen,last_day(i_prvniDen));
    return i_pocetPrestupku * 1000 + i_plat;
end;
/

create or replace procedure aktualizuj_jmeno_prijmeni_policisty(p_idUzivatele number, p_jmeno varchar2, p_prijmeni varchar2)
as
begin
    
    update policiste set jmeno = p_jmeno, prijmeni = p_prijmeni where iduzivatele = p_idUzivatele;
end;
/

create or replace procedure aktualizuj_jmeno_prijmeni_obcana(p_idUzivatele number, p_jmeno varchar2, p_prijmeni varchar2)
as
begin
    
    update obcane set jmeno = p_jmeno, prijmeni = p_prijmeni where iduzivatele = p_idUzivatele;
end;
/

create or replace view vsichniUzivatele as
SELECT u.iduzivatele, u.prihlasovacijmeno, u.heslo, o.nazevopravneni 
FROM uzivatele u
LEFT JOIN opravneni o on(o.idopravneni = u.idopravneni);

create or replace view prestupkyview as
select o.idobcana, t.prestupek, po.datum, o.jmeno || ' ' ||  o.prijmeni jmenoobcana, p.poznamka  from prestupky p
    join prestupky_obcanu po on(po.idprestupku = p.idprestupku)
    join typy_prestupku t on(t.idtypuprestupku = p.idtypuprestupku)
    join obcane o on(o.idobcana = po.idobcana);
    
create or replace view hlidkyView as
select h.nazevhlidky, t.nazev from hlidky h
join typy_hlidky t using(idtypu);
                               
create or replace view okrskyView as
SELECT * FROM okrsky;



create or replace package upravy_uzivatelu as

    procedure upravitUzivatele(p_prihlasovaciJmeno varchar2, p_heslo varchar2, p_typOpravneni varchar2, p_iduzivatele number);
    
    procedure smazUzivatele(p_iduzivatele number);
end;
/

create or replace package body upravy_uzivatelu as
    
    procedure upravitUzivatele(p_prihlasovaciJmeno varchar2, p_heslo varchar2, p_typOpravneni varchar2, p_iduzivatele number)
    is
        i_idOpravneni number;
        i_existuje number;
        i_nazevOpravneni varchar2(50);
    begin

        select idopravneni into i_idOpravneni from opravneni where nazevopravneni = lower(p_typOpravneni);
        
        select nazevopravneni into i_nazevOpravneni from opravneni join uzivatele using(idopravneni) where iduzivatele = p_iduzivatele;
        
        if p_typOpravneni != i_nazevOpravneni and (i_nazevOpravneni = 'obcan' or p_typOpravneni = 'obcan') then
            RAISE_APPLICATION_ERROR(-20000, 'Nelze mÏnit opr·vnÏnÌ z/na obËana');
        end if;
        
        SELECT COUNT(*) INTO i_existuje FROM uzivatele WHERE iduzivatele = p_iduzivatele;
         
        if i_existuje = 0 then
            RAISE_APPLICATION_ERROR(-20000, 'Uûivatel nenalezen!!');
        end if;
        
        UPDATE UZIVATELE SET PRIHLASOVACIJMENO = p_prihlasovaciJmeno, HESLO = p_heslo, IDOPRAVNENI = i_idOpravneni
        WHERE IDUZIVATELE = p_iduzivatele;
     
     exception when NO_DATA_FOUND then
        RAISE_APPLICATION_ERROR(-20000, 'Opr·vnÏnÌ nenalezeno');
    end upravitUzivatele;

    procedure smazUzivatele(p_iduzivatele number) 
    is
        i_opravneni varchar2(30);
    begin 
        --admin nejde smazat
        select o.nazevopravneni into i_opravneni from opravneni o join uzivatele u on(u.idopravneni = o.idopravneni) where u.iduzivatele = p_iduzivatele;
        
        if i_opravneni = 'administrator' then
            RAISE_APPLICATION_ERROR(-20000, 'Nelze smazat administr·tora!');
        end if;
        
        --jinak smaze 
        DELETE FROM UZIVATELE WHERE IDUZIVATELE = p_iduzivatele;
        
        exception when no_data_found then
            RAISE_APPLICATION_ERROR(-20000, 'Chyba p¯i zjiöùov·nÌ opr·vnÏnÌ uûivatele!');
    end;

end;
/

