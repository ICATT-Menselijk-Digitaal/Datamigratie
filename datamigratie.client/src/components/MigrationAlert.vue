<template>
  <alert-inline v-if="isRunning">
    Voor e-Suite zaaktype <em>{{ detFunctioneleIdentificatie }}</em> loopt nu een migratie van zaken
    van de e-Suite naar Open Zaak. Ondertussen kan er geen andere migratie gestart worden.
  </alert-inline>
</template>

<script lang="ts" setup>
import AlertInline from "@/components/AlertInline.vue";
import type { MigrationStatus } from "@/services/datamigratieService";

defineProps<MigrationStatus>();
</script>

<style lang="scss" scoped></style>
