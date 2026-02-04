# Validatie

Tijdens de migratie worden zaken gevalideerd voordat ze worden gemigreerd naar OpenZaak. Zaken die niet voldoen aan de validatieregels worden overgeslagen en verschijnen als niet-succesvol in de resultatenlog.

## Zaaknummer lengte

Zaken met een zaaknummer (identificatie) van meer dan 40 tekens worden niet gemigreerd. Deze zaken verschijnen in de resultatenlog met de melding "Het zaaknummer is te lang."

## Documenttype niet gevonden in zaaktype

Documenten met een documenttype dat niet voorkomt in de lijst van documenttypen voor het zaaktype worden niet gemigreerd. Deze moeten eerst in de esuite worden aangepast.

**Foutmelding:** `Documenttype '<naam>' was found in the document of the zaak, but not in the list of possible DocumentTypen in the zaaktype. Please configure this correctly in the esuite`
