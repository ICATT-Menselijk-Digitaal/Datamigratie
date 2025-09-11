{{/*
Expand the name of the chart.
*/}}
{{- define "datamigratie.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "datamigratie.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "datamigratie.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "datamigratie.labels" -}}
helm.sh/chart: {{ include "datamigratie.chart" . }}
{{ include "datamigratie.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "datamigratie.selectorLabels" -}}
app.kubernetes.io/name: {{ include "datamigratie.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "datamigratie.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "datamigratie.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

{{/* Database connection string helper */}}
{{- define "datamigratie.databaseConnectionString" -}}
{{- if .Values.postgresql.enabled -}}
  {{- printf "Host=%s-postgresql;Port=%d;Database=%s;Username=%s;Password=%s;" .Release.Name (.Values.settings.database.port | int) .Values.settings.database.name .Values.settings.database.username .Values.settings.database.password -}}
{{- else -}}
  {{- printf "Host=%s;Port=%d;Database=%s;Username=%s;Password=%s;" .Values.settings.database.host (.Values.settings.database.port | int) .Values.settings.database.name .Values.settings.database.username .Values.settings.database.password -}}
{{- end -}}
{{- end -}}