# Installatie

## Lokaal Draaien

Om de Datamigratie applicatie lokaal te draaien:

1. Open met Visual Studio
2. Stel Datamigratie.AppHost in als startup project
3. Start de applicatie

## Helm Deployment

Voor het deployen van de applicatie met Helm:

1. Kopieer `charts/datamigratie/values.yaml` naar je eigen values bestand
2. Pas de waardes aan voor jouw omgeving (API keys, database, etc.)
3. Installeer met Helm:

```bash
helm install datamigratie ./charts/datamigratie -f jouw-values.yaml
```
