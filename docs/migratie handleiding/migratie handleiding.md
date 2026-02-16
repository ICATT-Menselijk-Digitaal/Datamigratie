# Handleiding

## Overzicht

**Deze handleiding is nog in ontwikkeling!**

Datamigratie is een applicatie voor het migreren van Zaak-data uit de e-Suite naar Open Zaak.



### Gebruikersflow

1. **Algemene Configuratie invoeren:**
   - De Datamigratie Tool vereist een geldig 9-cijferig RSIN (Rechtspersonen Samenwerkingsverbanden Informatienummer) om migraties uit te kunnen voeren. Deze kan men invoeren in de Configuratie-pagina van Datamigratie.
   - Documentstatus mapping ...
   - 
2. **Zaaktype Mappings aanmaken:**
   - ...

3. **Migratie starten:**
   - Bij het starten van een migratie wordt de algmene en de zaaktypemapping gevalideerd.
   - Als het mapping ontbreekt of ongeldig is, wordt de migratie geblokkeerd



# Validatie

Tijdens de migratie worden zaken gevalideerd voordat ze worden gemigreerd naar OpenZaak. 
Zaken die niet voldoen aan de validatieregels worden overgeslagen en verschijnen als niet-succesvol in de resultatenlog.

## Zaaknummer lengte

Zaken met een zaaknummer (identificatie) van meer dan 40 tekens worden niet gemigreerd. Deze zaken verschijnen in de resultatenlog met de melding "Het zaaknummer is te lang."

## Documenttype niet gevonden in zaaktype

Documenten met een documenttype dat niet voorkomt in de lijst van documenttypen voor het zaaktype worden niet gemigreerd. Deze moeten eerst in de esuite worden aangepast.

## ...


**Deze handleiding is nog in ontwikkeling!**


