<template>
  <simple-spinner v-if="loading"></simple-spinner>

  <alert-inline v-else-if="error">{{ error }}</alert-inline>

  <alert-inline v-else-if="migrationStatus?.isRunning" class="notice--loading">
    <p>
      Voor e-Suite zaaktype <em>{{ migrationStatus.detFunctioneleIdentificatie }}</em> loopt nu een
      migratie van zaken van de e-Suite naar Open Zaak. Ondertussen kan er geen andere migratie
      gestart worden.
    </p>

    <menu class="reset">
      <li>
        <button type="button" @click="fetchMigrationStatus">Ververs status</button>
      </li>
    </menu>
  </alert-inline>
</template>

<script lang="ts" setup>
import { onMounted } from "vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import AlertInline from "@/components/AlertInline.vue";
import { useMigrationStatus } from "@/composables/use-migration-status";

const { migrationStatus, fetchMigrationStatus, loading, error } = useMigrationStatus();

onMounted(() => fetchMigrationStatus());
</script>

<style lang="scss" scoped>
p:first-of-type {
  margin-block-start: 0;
}

button {
  margin-block: 0;
}
</style>
