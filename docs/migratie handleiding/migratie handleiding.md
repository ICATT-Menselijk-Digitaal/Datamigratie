# Handleiding

## Overzicht

**Deze handleiding is nog in ontwikkeling!**

Datamigratie is een applicatie voor het migreren van Zaak-data uit de e-Suite naar Open Zaak.



# Validatie

Tijdens de migratie worden zaken gevalideerd voordat ze worden gemigreerd naar OpenZaak. 
Zaken die niet voldoen aan de validatieregels worden overgeslagen en verschijnen als niet-succesvol in de resultatenlog.

## Zaaknummer lengte

Zaken met een zaaknummer (identificatie) van meer dan 40 tekens worden niet gemigreerd. Deze zaken verschijnen in de resultatenlog met de melding "Het zaaknummer is te lang."

## Documenttype niet gevonden in zaaktype

Documenten met een documenttype dat niet voorkomt in de lijst van documenttypen voor het zaaktype worden niet gemigreerd. Deze moeten eerst in de esuite worden aangepast.

## ...


**Deze handleiding is nog in ontwikkeling!**


