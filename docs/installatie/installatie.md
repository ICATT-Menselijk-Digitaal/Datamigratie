# Installatie

## Helm Deployment

Voor het deployen van de applicatie met Helm:

1. Kopieer [`charts/datamigratie/values.yaml`](https://github.com/ICATT-Menselijk-Digitaal/Datamigratie/blob/main/charts/datamigratie/values.yaml) naar je eigen values bestand
2. Pas de waardes aan voor jouw omgeving (API keys, database, etc.)
3. Waardes die al goed staan kan je weglaten
3. Installeer met Helm:

```bash
helm install jouw-release-naam-voor-datamigratie oci://ghcr.io/icatt-menselijk-digitaal/datamigratie -f jouw-values.yaml
```


## Environment Variabelen

De applicatie gebruikt de onderstaande environment variabelen. <br />
Let op: De API- en andere environment variabelen gebruiken dubbele underscores (`__`) voor .NET configuratie binding.

| Variabele                         | Uitleg                                                                                           |
| --------------------------------- | ------------------------------------------------------------------------------------------------ |
| `OpenZaakApi__BaseUrl`            | De base url van de API's bij de Open Zaak-instantie, waar naartoe gemigreerd wordt. <details> <summary>Meer informatie </summary> Bijvoorbeeld https://openzaak.example.nl/ </details>                    |
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


### Feature Flags

De applicatie gebruikt feature flags om bepaalde functionaliteit in of uit te schakelen. Deze kunnen geconfigureerd worden via environment variabelen.

| Flag                | Environment Variable              | Standaard | Beschrijving                                                                    |
| ------------------- | --------------------------------- | --------- | ------------------------------------------------------------------------------- |
| `EnableTestHelpers` | `FeatureFlags__EnableTestHelpers` | `false`   | Schakelt test helper UI in voor batch selectie bij het mappen van grote aantallen documenttypes. Biedt een pre-selectie optie om het mapping proces te versnellen.                                                                                             |
