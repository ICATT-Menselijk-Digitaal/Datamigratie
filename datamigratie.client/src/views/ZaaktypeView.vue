<template>
  <router-link
    :to="{ name: 'detZaaktypes', ...(search && { query: { search } }) }"
    class="button button-secondary"
    >&lt; Terug</router-link
  >

  <h2>e-Suite zaaktype "{{ zaaktypeMapping?.detZaaktype?.naam || "..." }}"</h2>

  <zaaktype-mapping-section
    v-if="detZaaktypeId"
    :det-zaaktype-id="detZaaktypeId"
    :disabled="isThisMigrationRunning"
    v-model:zaaktype-mapping="zaaktypeMapping"
  />

  <template v-if="zaaktypeMapping">
    <h3>Mapping</h3>

    <status-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      :disabled="isThisMigrationRunning"
      @update:complete="statusMappingsComplete = $event"
    />

    <resultaattype-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      :disabled="isThisMigrationRunning"
      @update:complete="resultaattypeMappingsComplete = $event"
    />

    <besluittype-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      :disabled="isThisMigrationRunning"
      @update:complete="besluittypeMappingsComplete = $event"
    />

    <document-property-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      :disabled="isThisMigrationRunning"
      @update:complete="documentPropertyMappingsComplete = $event"
    />

    <vertrouwelijkheid-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      :disabled="isThisMigrationRunning"
      @update:complete="vertrouwelijkheidMappingsComplete = $event"
    />

    <menu class="reset" v-if="!error && !isThisMigrationRunning && canStartMigration">
      <li>
        <button type="button" @click="startMigration">Start migratie</button>
      </li>
    </menu>
  </template>

  <prompt-modal
    :dialog="confirmDialog"
    cancel-text="Nee, niet migreren"
    confirm-text="Ja, start migratie"
  >
    <h2>Migratie starten</h2>

    <p>
      Weet je zeker dat je de migratie van zaken van het e-Suite zaaktype
      <em>{{ zaaktypeMapping?.detZaaktype?.naam }}</em> wilt starten?
    </p>
  </prompt-modal>

  <migration-history-table v-if="!error" :det-zaaktype-id="detZaaktypeId" />
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useRoute } from "vue-router";
import PromptModal from "@/components/PromptModal.vue";
import StatusMappingSection from "@/components/StatusMappingSection.vue";
import BesluittypeMappingSection from "@/components/BesluittypeMappingSection.vue";
import { useMigrationControl } from "@/composables/use-migration-control";
import ResultaattypeMappingSection from "@/components/ResultaattypeMappingSection.vue";
import DocumentPropertyMappingSection from "@/components/DocumentPropertyMappingSection.vue";
import VertrouwelijkheidMappingSection from "@/components/VertrouwelijkheidMappingSection.vue";
import MigrationHistoryTable from "@/components/MigrationHistoryTable.vue";
import ZaaktypeMappingSection, {
  type ZaaktypeMappingModel
} from "@/components/ZaaktypeMappingSection.vue";
import { useMigration } from "@/composables/migration-store";
import { MigrationStatus } from "@/types/datamigratie";
const { detZaaktypeId } = defineProps<{ detZaaktypeId: string }>();

const route = useRoute();
const search = computed(() => String(route.query.search || "").trim());

const zaaktypeMapping = ref<ZaaktypeMappingModel>();

const statusMappingsComplete = ref(false);
const resultaattypeMappingsComplete = ref(false);
const besluittypeMappingsComplete = ref(false);
const documentPropertyMappingsComplete = ref(false);
const vertrouwelijkheidMappingsComplete = ref(false);

const { error, migration } = useMigration();
const { isThisMigrationRunning, confirmDialog, startMigration } = useMigrationControl(
  () => detZaaktypeId
);

const allIsComplete = computed(
  () =>
    statusMappingsComplete.value &&
    besluittypeMappingsComplete.value &&
    resultaattypeMappingsComplete.value &&
    documentPropertyMappingsComplete.value &&
    vertrouwelijkheidMappingsComplete.value
);

const canStartMigration = computed(
  () => allIsComplete.value && migration.value?.status !== MigrationStatus.inProgress
);
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

.status-mapping {
  margin-block-end: var(--spacing-large);

  h2 {
    font-size: 1.5rem;
    margin-block-end: var(--spacing-small);
  }

  p {
    margin-block-end: var(--spacing-default);
  }
}

menu {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-default);

  li > * {
    text-align: center;
    inline-size: 100%;
  }

  @media (min-width: variables.$breakpoint-md) {
    & {
      flex-direction: row;

      li:first-of-type {
        margin-inline-end: auto;
      }
    }
  }

  &.edit-menu {
    @media (min-width: variables.$breakpoint-md) {
      li:only-of-type {
        margin-inline-start: auto;
        margin-inline-end: 0;
      }
    }
  }
}
</style>
