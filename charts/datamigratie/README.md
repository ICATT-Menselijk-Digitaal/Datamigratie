# datamigratie

![Version: 0.0.0](https://img.shields.io/badge/Version-0.0.0-informational?style=flat-square) ![Type: application](https://img.shields.io/badge/Type-application-informational?style=flat-square) ![AppVersion: 0.0.0](https://img.shields.io/badge/AppVersion-0.0.0-informational?style=flat-square)

A Helm chart for the Datamigratie Component

## Requirements

| Repository | Name | Version |
|------------|------|---------|
| https://charts.bitnami.com/bitnami | postgresql | 12.5.6 |
> [!NOTE]
> For production environments, it is recommended to use [CloudNativePG](https://github.com/cloudnative-pg/cloudnative-pg) for PostgreSQL in stead of the dependent Helm charts included here. The bundled charts are primarily intended for testing and development purposes. Also, be aware of the upcoming changes to the bitnami catalog described in this [issue](https://github.com/bitnami/containers/issues/83267).

## Values

| Key | Type | Default | Description |
|-----|------|---------|-------------|
| affinity | object | `{}` |  |
| apiConnections.detApi.apiKey | string | `""` |  |
| apiConnections.detApi.baseUrl | string | `""` |  |
| apiConnections.openZaakApi.apiKey | string | `""` |  |
| apiConnections.openZaakApi.apiUser | string | `""` |  |
| apiConnections.openZaakApi.baseUrl | string | `""` |  |
| autoscaling.enabled | bool | `false` |  |
| autoscaling.maxReplicas | int | `100` |  |
| autoscaling.minReplicas | int | `1` |  |
| autoscaling.targetCPUUtilizationPercentage | int | `80` |  |
| autoscaling.targetMemoryUtilizationPercentage | int | `80` |  |
| existingSecret | string | `nil` |  |
| extraIngress | list | `[]` |  |
| fullnameOverride | string | `""` |  |
| image.pullPolicy | string | `"IfNotPresent"` |  |
| image.repository | string | `"ghcr.io/icatt-menselijk-digitaal/datamigratie-server"` |  |
| image.tag | string | `""` |  |
| imagePullSecrets | list | `[]` |  |
| ingress.annotations | object | `{}` |  |
| ingress.className | string | `""` |  |
| ingress.enabled | bool | `false` |  |
| ingress.hosts | list | `[]` |  |
| ingress.tls | list | `[]` |  |
| livenessProbe.failureThreshold | int | `6` |  |
| livenessProbe.initialDelaySeconds | int | `60` |  |
| livenessProbe.periodSeconds | int | `10` |  |
| livenessProbe.successThreshold | int | `1` |  |
| livenessProbe.timeoutSeconds | int | `5` |  |
| migrations.image.pullPolicy | string | `"IfNotPresent"` |  |
| migrations.image.repository | string | `"ghcr.io/icatt-menselijk-digitaal/datamigratie-migrations"` |  |
| migrations.image.tag | string | `""` |  |
| migrations.restartPolicy | string | `"Never"` |  |
| nameOverride | string | `""` |  |
| nodeSelector | object | `{}` |  |
| pdb.create | bool | `false` |  |
| pdb.maxUnavailable | string | `""` |  |
| pdb.minAvailable | int | `1` |  |
| persistence.enabled | bool | `true` |  |
| persistence.existingClaim | string | `nil` |  |
| persistence.size | string | `"1Gi"` |  |
| persistence.storageClassName | string | `""` |  |
| podAnnotations | object | `{}` |  |
| podLabels | object | `{}` |  |
| podSecurityContext.fsGroup | int | `1000` |  |
| postgresql.auth.database | string | `"datamigratie"` |  |
| postgresql.auth.password | string | `"changeme"` |  |
| postgresql.auth.postgresPassword | string | `"changeme"` |  |
| postgresql.auth.username | string | `"datamigratie"` |  |
| postgresql.enabled | bool | `false` |  |
| postgresql.metrics.enabled | bool | `false` |  |
| postgresql.primary.persistence.enabled | bool | `true` |  |
| postgresql.primary.persistence.size | string | `"8Gi"` |  |
| readinessProbe.failureThreshold | int | `6` |  |
| readinessProbe.initialDelaySeconds | int | `30` |  |
| readinessProbe.periodSeconds | int | `10` |  |
| readinessProbe.successThreshold | int | `1` |  |
| readinessProbe.timeoutSeconds | int | `5` |  |
| replicaCount | int | `1` |  |
| resources | object | `{}` |  |
| securityContext.capabilities.drop[0] | string | `"ALL"` |  |
| securityContext.readOnlyRootFilesystem | bool | `false` |  |
| securityContext.runAsNonRoot | bool | `true` |  |
| securityContext.runAsUser | int | `1000` |  |
| service.port | int | `80` |  |
| service.type | string | `"ClusterIP"` |  |
| serviceAccount.annotations | object | `{}` |  |
| serviceAccount.automountServiceAccountToken | bool | `true` |  |
| serviceAccount.create | bool | `true` |  |
| serviceAccount.name | string | `""` |  |
| settings.apiKeys | list | `[]` |  |
| settings.aspnetcore.environment | string | `""` |  |
| settings.aspnetcore.forwardedHeadersEnabled | bool | `true` |  |
| settings.aspnetcore.httpPorts | string | `""` |  |
| settings.database.host | string | `""` |  |
| settings.database.name | string | `"datamigratie"` |  |
| settings.database.password | string | `""` |  |
| settings.database.port | int | `5432` |  |
| settings.database.username | string | `""` |  |
| settings.featureFlags.enableTestHelpers | bool | `false` |  |
| settings.oidc.authority | string | `""` |  |
| settings.oidc.clientId | string | `""` |  |
| settings.oidc.clientSecret | string | `""` |  |
| settings.oidc.emailClaimType | string | `""` |  |
| settings.oidc.functioneelBeheerderRole | string | `""` |  |
| settings.oidc.nameClaimType | string | `""` |  |
| settings.oidc.roleClaimType | string | `""` |  |
| tolerations | list | `[]` |  |

----------------------------------------------
Autogenerated from chart metadata using [helm-docs v1.14.2](https://github.com/norwoodj/helm-docs/releases/v1.14.2)
