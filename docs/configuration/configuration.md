# Configuratie

## Environment Variabelen

De applicatie gebruikt de volgende environment variabelen:

```
OpenZaakApi__BaseUrl
OpenZaakApi__ApiUser
OpenZaakApi__ApiKey

DetApi__BaseUrl
DetApi__ApiKey

ConnectionStrings__Datamigratie

Oidc__Authority
Oidc__ClientId
Oidc__ClientSecret
Oidc__FunctioneelBeheerderRole
```

Let op: De API variabelen gebruiken dubbele underscores (`__`) voor .NET configuratie binding.

## OIDC Configuratie voor UI Inloggen

Om in de UI in te kunnen loggen, moet je in de user secrets van het Datamigratie.Server project de benodigde configuratie invullen:

```json
{
  "Oidc": {
    "Authority": "",
    "ClientId": "",
    "ClientSecret": "",
    "FunctioneelBeheerderRole": ""
  }
}
```

## RSIN Globale Configuratie

### Overzicht

De Datamigratie Tool vereist een geldig RSIN (Rechtspersonen Samenwerkingsverbanden Informatienummer) om migraties uit te kunnen voeren.

### Technische Details

#### Validatie

Het RSIN wordt gevalideerd volgens de 11-proef (elfproef), zoals beschreven in de [Wikipedia over BSN-nummers](https://nl.wikipedia.org/wiki/Burgerservicenummer#11-proef).

De validatie controleert het volgende:
- Het RSIN moet precies **9 cijfers** lang zijn
- Het RSIN mag alleen **numerieke tekens** bevatten
- Het RSIN moet voldoen aan de **11-proef**

**11-proef algoritme:**
1. Vermenigvuldig elk cijfer met zijn positiewaarde (9, 8, 7, 6, 5, 4, 3, 2, -1)
2. Tel alle resultaten op
3. De som moet deelbaar zijn door 11 (modulo 11 = 0)

Voorbeeld voor RSIN `123456782`:
```
(1×9) + (2×8) + (3×7) + (4×6) + (5×5) + (6×4) + (7×3) + (8×2) + (2×-1) = 165
165 % 11 = 0 ✓ (geldig)
```

### Gebruikersflow

1. **Configuratie invoeren:**
   - De functioneel beheerder navigeert naar de globale configuratiepagina
   - Voert een 9-cijferig RSIN in
   - Het systeem valideert het RSIN volgens de 11-proef
   - Bij een geldig RSIN wordt de waarde opgeslagen

2. **Mappings aanmaken:**
   - Mappings kunnen worden aangemaakt en opgeslagen ongeacht of het RSIN is geconfigureerd
   - Dit stelt gebruikers in staat om hun mappings voor te bereiden

3. **Migratie starten:**
   - Bij het starten van een migratie wordt het RSIN gevalideerd
   - Als het RSIN ontbreekt of ongeldig is, wordt de migratie geblokkeerd
   - Er wordt een duidelijke foutmelding getoond met instructies om het RSIN te configureren

### Gebruik tijdens Migratie

Het RSIN wordt gebruikt voor:
- **Zaak metadata**: Het RSIN identificeert de bronorganisatie voor elke zaak en wordt ook gebruikt als standaardwaarde voor andere zaak data velden.
