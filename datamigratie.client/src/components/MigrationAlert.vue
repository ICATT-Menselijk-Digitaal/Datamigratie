<template>
  <simple-spinner v-if="loading"></simple-spinner>

  <alert-inline v-else-if="error">{{ error }}</alert-inline>

  <alert-inline v-else-if="migration?.status === MigrationStatus.inProgress" >
    <span class="spinner" role="presentation" aria-hidden="true"></span>

    <p>
      Voor e-Suite zaaktype <em>{{ migration.detZaaktypeId }}</em> loopt nu een migratie van zaken
      van de e-Suite naar Open Zaak. Ondertussen kan er geen andere migratie gestart worden.
    </p>

    <menu class="reset">
      <li>
        <button type="button" @click="fetchMigration">Ververs status</button>
      </li>
    </menu>
  </alert-inline>
</template>

<script lang="ts" setup>
import { onMounted } from "vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import AlertInline from "@/components/AlertInline.vue";
import { MigrationStatus } from "@/services/datamigratieService";
import { useMigration } from "@/composables/use-migration-status";

const { migration, fetchMigration, loading, error } = useMigration();
onMounted(() => fetchMigration());
</script>

<style lang="scss" scoped>
p:first-of-type,
button:first-of-type {
  margin-block: 0;
}

.notice {
  --_spinner-size: 2rem;

  display: grid;
  grid-template-columns: var(--_spinner-size) 1fr;
  grid-template-areas: "spinner ." "spinner .";
  gap: var(--spacing-default);

  .spinner {
    grid-area: spinner;
    display: block;
    block-size: var(--_spinner-size);
    inline-size: var(--_spinner-size);
    border: 0.2rem solid var(--disabled);
    border-top: 0.2rem solid var(--accent);
    border-radius: 50%;
    animation: spin 1s linear infinite;
  }
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style>
