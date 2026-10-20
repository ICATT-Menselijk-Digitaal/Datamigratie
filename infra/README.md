# infra

Infrastructuur-manifesten die het draaien van Datamigratie ondersteunen, maar geen onderdeel zijn van de applicatie zelf (zie `charts/datamigratie` daarvoor).

## podiumd-proxy-bridge

Specifiek voor interne testdoeleinden van ICATT toegevoegd.

Een tijdelijke proxy waarmee de applicatie het DET/esuite-systeem kan bereiken via een bestaand netwerkpad dat nog niet rechtstreeks bereikbaar is.

Het draait als een kleine reverse proxy (`deployment.yaml`, `service.yaml`, `configmap.yaml`), ontsloten via een Ingress (`ingress.yaml`) die alleen verzoeken op een specifiek pad vanaf een specifieke bekende bron accepteert, via HTTPS.

Implementeren met:

```bash
kubectl apply -f infra/podiumd-proxy-bridge/
```

Indien je onderdeel bent van de ICATT/Info organisatie, bekijk de volledige documentatie op:

https://infonl.atlassian.net/wiki/spaces/PRJ/pages/1959886849/Setting+up+PodiumD+proxy+in+an+Azure+kubernetes+cluster+for+Cyso+or+any+other+host

Dit is tijdelijke scaffolding, geen permanente architectuur — het bestaat om test/migratie te ontgrendelen totdat een directe verbinding is opgezet. Breid dit niet verder uit zonder goede reden.
