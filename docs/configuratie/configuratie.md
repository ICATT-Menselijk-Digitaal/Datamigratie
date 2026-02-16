# Configuratie

## Environment Variabelen

De applicatie gebruikt de onderstaande environment variabelen. <br />
Let op: De API- en andere environment variabelen gebruiken dubbele underscores (`__`) voor .NET configuratie binding.

| Variabele                         | Uitleg                                                                                           |
| --------------------------------- | ------------------------------------------------------------------------------------------------ |
| `OpenZaakApi__BaseUrl` | De base url van de API's bij de Open Zaak-instantie, waar naartoe gemigreerd wordt. <details> <summary>Meer informatie </summary> Bijvoorbeeld https://openzaak.example.nl/ </details>                    |
| `OpenZaakApi__ApiUser`            | ClientID van Datamigratie in Open Zaak                                                           |
| `OpenZaakApi__ApiKey`             | Client Secret van Datamigratie in Open Zaak                                                      |
| `DetApi__BaseUrl`                 | De base url van het API Endpoint in de DET <details> <summary>Meer informatie </summary> Bijvoorbeeld https://esuite-data-extractie-gcp2.example.nl/ </details>                                     |
| `DetApi__ApiKey`                  | API Key van Datamigratie in de DET                                                               |
| `Oidc__Authority`                 | Authority URL van de OIDC provider via welke gebruikers inloggen op Datamigratie                 |
| `Oidc__ClientId`                  | ClientID van de OIDC provider                                                                    |
| `Oidc__ClientSecret`              | Client Secret van de OIDC Provider                                                               |
| `Oidc__FunctioneelBeheerderRole`  | Naam van de Rol die beheerfuncties mag uitvoeren in Datamigratie <details> <summary>Meer informatie </summary> Bijvoorbeeld DMT-Functioneel-Beheerder</details>                        |
| `Oidc__NameClaimType`             | De naam van de claim in het JWT token van de OpenID Connect Provider waarin de volledige naam van de ingelogde gebruiker staat                                                      |
| `Oidc__RoleClaimType`             | De naam van de claim in het JWT token van de OpenID Connect Provider waarin de rollen van de ingelogde gebruiker staan.                                                     |
| `Oidc__EmailClaimType`            | De naam van de claim in het JWT token van de OpenID Connect Provider waarin het e-mailadres van de ingelogde gebruiker staat.                                                     |
|                                   |                      |

## OIDC Configuratie om in te kunnen loggen op de UI

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

De Datamigratie Tool vereist een geldig RSIN (Rechtspersonen Samenwerkingsverbanden Informatienummer) om migraties uit te kunnen voeren. Dit RSIN voert de Functioneel Beheerder in, in de Configuratie-pagina's van Datamigratie. Zorg ervoor dat je hier hetzelde RSIN configureert als in Open Zaak is geconfigureerd.

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

## Feature Flags

De applicatie gebruikt feature flags om bepaalde functionaliteit in of uit te schakelen. Deze kunnen geconfigureerd worden via environment variabelen.

### Beschikbare Feature Flags

| Flag                | Environment Variable              | Standaard | Beschrijving                                                                    |
| ------------------- | --------------------------------- | --------- | ------------------------------------------------------------------------------- |
| `EnableTestHelpers` | `FeatureFlags__EnableTestHelpers` | `false`   | Schakelt test helper UI in voor batch selectie bij het mappen van grote aantallen documenttypes. Biedt een pre-selectie optie om het mapping proces te versnellen.                                                                                             |