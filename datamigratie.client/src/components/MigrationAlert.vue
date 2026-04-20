<template>
  <alert-inline v-if="error">{{ error }}</alert-inline>

  <alert-inline v-else-if="migrationJustCompleted" type="info">
    <div class="migration-completed-content">
      <span class="checkmark" aria-hidden="true">✓</span>
      <p>
        De migratie van zaken voor e-Suite zaaktype <em>{{ migration?.detZaaktypeId }}</em> is
        voltooid.
      </p>
      <button type="button" class="secondary" @click="dismissCompletedAlert">Sluiten</button>
    </div>
  </alert-inline>

  <alert-inline v-else-if="migration?.status === MigrationStatus.inProgress" type="warning">
    <div class="migration-in-progress-content">
      <span class="spinner" role="presentation" aria-hidden="true"></span>

      <p>
        Voor e-Suite zaaktype <em>{{ migration.detZaaktypeId }}</em> loopt nu een migratie van zaken
        van de e-Suite naar Open Zaak. Ondertussen kan er geen andere migratie gestart worden.
      </p>
    </div>
  </alert-inline>
</template>

<script lang="ts" setup>
import { onMounted } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import { MigrationStatus } from "@/types/datamigratie";
import { useMigration } from "@/composables/migration-store";

const { migration, migrationJustCompleted, fetchMigration, dismissCompletedAlert, error } =
  useMigration();
onMounted(() => fetchMigration());
</script>

<style lang="scss" scoped>
p:first-of-type,
button:first-of-type {
  margin-block: 0;
}

.migration-completed-content {
  display: flex;
  align-items: center;
  gap: var(--spacing-default);

  p {
    flex: 1;
    margin-block: 0;
  }

  .checkmark {
    color: var(--success);
    font-size: 1.5rem;
    font-weight: var(--font-bold);
    flex-shrink: 0;
  }

  button {
    flex-shrink: 0;
    margin-inline-start: auto;
  }
}

.migration-in-progress-content {
  --_spinner-size: 2rem;

  display: grid;
  grid-template-columns: var(--_spinner-size) 1fr;
  gap: var(--spacing-default);

  .spinner {
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
