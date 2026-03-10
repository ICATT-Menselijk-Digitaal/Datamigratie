# Datamigratie handleiding voor eindgebruikers

## Introductie
De DataMigratieTool (DMT) is een applicatie waarmee e-Suite-data vanuit de database van de e-Suite naar het OpenZaak (OZ) register kan worden gemigreerd. Het ophalen van e-Suite-data uit de database wordt uitgevoerd via de DataExtractieTool (DET). De huidige versie van de DMT is uitsluitend geschikt voor het migreren van *gesloten* zaken.

## Mapping configureren
Om een migratie te kunnen starten moet eerst een mapping worden geconfigureerd voor de velden die niet direct overgezet kunnen worden. Deze configuratie vindt plaats op twee niveaus:
1. Algemeen (deze configuratie geldt voor *alle* zaaktypes)
2. Op zaaktype-niveau (deze configuratie geldt alleen voor het geselecteerde zaaktype)

Om een zaaktype te kunnen migreren moet zowel de algemene configuratie als de configuratie van dat specifieke zaaktype ingevuld zijn. De startpagina van de datamigratieapplicatie is de lijst met e-Suite-zaaktypes. Nadat een e-Suite-zaaktype is geselecteerd, wordt de zaaktypepagina geopend. 

**Let op:** als de zaaktypekoppeling wordt aangepast nadat er al onderdelen geconfigureerd zijn, worden deze verwijderd op het moment dat de koppeling wijzigt.

### Algemene configuratie
Onderdeel van de algemene configuratie zijn:
- **RSIN:** Vul hier het RSIN-nummer van de gemeente in.
- **Documentstatussen:** De subselectie van documentstatussen verschilt per zaaktype in de e-Suite, maar elke subselectie is onderdeel van één algemene lijst. Map hier alle mogelijke documentstatussen in de e-Suite naar één van de vier vaste opties voor een documentstatus in OZ.

### Configuratie per zaaktype
Voordat de DMT kan worden gebruikt om een bepaald zaaktype te migreren moet dit aan de OZ-kant *volledig* ingericht zijn (inclusief alle objecten die onderdeel zijn van de migratie). Zaaktypes die geen volledige OZ-inrichting hebben kunnen *niet* gemigreerd worden.

Om de verdere configuratie voor een zaaktype uit te voeren moet het geselecteerde e-Suite-zaaktype worden gekoppeld aan een OZ-zaaktype. Nadat deze koppeling is opgeslagen kan de rest van de mapping worden geconfigureerd. Als er in OZ onderdelen ontbreken van de te mappen velden, kunnen deze niet gemapt worden en kan voor dit zaaktype geen migratie worden gestart.  
Voor elk onderdeel van de mapping geldt dat elke e-Suite-optie naar precies één OZ-optie gemapt moet worden. Er kunnen meerdere e-Suite-opties naar dezelfde OZ-optie worden gemapt.

Onderdeel van de configuratie per zaaktype zijn:
- **Status:** Map hier alle mogelijke e-Suite-statussen naar een OZ-statustype voor het geselecteerde zaaktype.
- **Resultaattype:** Map hier alle mogelijke e-Suite-resultaattypes naar een OZ-resultaattype voor het geselecteerde zaaktype.
- **Besluittype:** Indien van toepassing voor dit zaaktype: map hier alle mogelijke e-Suite-besluittypes naar een OZ-besluittype voor het geselecteerde zaaktype.
- **Publicatieniveau:** Map hier alle mogelijke e-Suite-publicatieniveaus *van een document* naar een OZ-vertrouwelijkheidaanduiding *van een document* voor het geselecteerde zaaktype.
- **Documenttype:** Map hier alle mogelijke e-Suite-documenttypes naar een OZ-informatieobjecttype voor het geselecteerde zaaktype.
- **Vertrouwelijkheid:** Map hier alle mogelijke e-Suite-vertrouwelijkheidswaardes *van een zaak* naar een OZ-vertrouwelijkheidaanduiding *van een zaak* voor het geselecteerde zaaktype.
- **Informatieobjecttype voor gegenereerde PDF:** Aan elke zaak wordt in OZ een gegenereerde PDF toegevoegd. Deze PDF bevat de volgende informatie:
    - Alle velden die gemigreerd moeten worden maar geen passende plek hebben in OZ.
    - Alle velden die niet optimaal gemigreerd worden. Enkele waardes zijn ingekort tijdens de migratie vanwege lengtebeperkingen van OZ. In de PDF zijn deze waardes volledig terug te vinden.

## Migreren
Op het moment dat voor alle onderdelen van een zaaktype een mapping is opgeslagen verschijnt de knop **"Migreren"**. Zonder dat de onderdelen van de **Algemene** configuratie volledig zijn ingevuld kan er *geen* migratie worden gestart. Nadat een migratie is gestart verschijnt bovenin het scherm een melding dat er een migratie actief is. Tijdens een migratierun wordt geen tussentijdse informatie weergegeven over het aantal zaken dat al is verwerkt. Wanneer een migratie is afgerond verdwijnt de melding en wordt de migratie onderaan de zaaktypepagina weergegeven in de tabel met migratiegeschiedenis. Door op de regel van de migratie te klikken wordt de migratiedetailpagina geopend. Hier staat de migratie-informatie op zaakniveau: welke zaken zijn gefaald (en waarom) en welke zaken zijn geslaagd.

**Let op:** het is *niet* mogelijk om meerdere migraties tegelijk te starten, ook niet vanaf meerdere accounts of voor verschillende zaaktypes.

De knop **"Migreren"** biedt drie verschillende mogelijkheden:

1. **Alle niet gemigreerde zaken**  
    Met de optie "Alle niet gemigreerde zaken" worden alleen de zaken gemigreerd die nog niet als succesvol gemigreerd zijn gemarkeerd door de DMT.

2. **Alle zaken**  
    Met de optie "Alle zaken" worden *alle* zaken gemigreerd. Wanneer tijdens een eerdere migratierun al zaken succesvol zijn gemigreerd van de e-Suite naar OZ, worden deze uit OZ verwijderd en *opnieuw* gemigreerd wanneer deze optie wordt gekozen.

3. **Eén zaak**  
    Met deze optie is het mogelijk om één enkele zaak te migreren op basis van het zaaknummer van de e-Suite. Deze functie is vooral bedoeld om te testen of de mapping van het zaaktype goed is geconfigureerd en om te checken of de gekozen zaak correct in OpenZaak terecht komt. Het wordt aangeraden om hier mee te beginnen voordat een volledige migratie wordt gestart, aangezien dat erg lang kan duren.
