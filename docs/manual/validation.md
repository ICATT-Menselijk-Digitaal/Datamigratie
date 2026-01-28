# Validatie

Tijdens de migratie worden zaken gevalideerd voordat ze worden gemigreerd naar OpenZaak. Zaken die niet voldoen aan de validatieregels worden overgeslagen en verschijnen als niet-succesvol in de resultatenlog.

## Zaaknummer lengte

Zaken met een zaaknummer (identificatie) van meer dan 40 tekens worden niet gemigreerd. Deze zaken verschijnen in de resultatenlog met de melding "Het zaaknummer is te lang."
