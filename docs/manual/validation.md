# Validatie

Tijdens de migratie worden zaken gevalideerd voordat ze worden gemigreerd naar OpenZaak. Zaken die niet voldoen aan de validatieregels worden overgeslagen en verschijnen als niet-succesvol in de resultatenlog.

## Zaaknummer lengte

Zaken met een zaaknummer (identificatie) van meer dan 40 tekens worden niet gemigreerd. Deze zaken verschijnen in de resultatenlog met de melding "Het zaaknummer is te lang."

## Documenttype niet gevonden in zaaktype

Documenten met een documenttype dat niet voorkomt in de lijst van documenttypen voor het zaaktype worden niet gemigreerd. Deze moeten eerst in de esuite worden aangepast.

**Foutmelding:** `Documenttype '<naam>' is set on this document, but is not mapped. Please add this documenttype to the corresponding zaaktype in the ESuite`
