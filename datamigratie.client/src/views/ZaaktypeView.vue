<template>
  <router-link
    :to="{ name: 'detZaaktypes', ...(search && { query: { search } }) }"
    class="button button-secondary"
    >&lt; Terug</router-link
  >

  <h2>e-Suite zaaktype "{{ detZaaktypeNaam || "..." }}"</h2>

  <alert-inline v-if="!isGeneralConfigLoading && !isGeneralConfigComplete" type="warning">
    Let op: de migratie kan pas worden gestart als alle gegevens bij "Algemeen" ook zijn ingevuld.
  </alert-inline>

  <zaaktype-mapping-section
    v-if="detZaaktypeId"
    :det-zaaktype-id="detZaaktypeId"
    :disabled="isThisMigrationRunning"
    v-model:zaaktype-mapping="zaaktypeMapping"
    @update:det-zaaktype-naam="detZaaktypeNaam = $event"
  />

  <template v-if="zaaktypeMapping">
    <h3>Mapping</h3>

    <auto-selecter v-if="featureFlags.showTestHelpers" />

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

    <publicatie-niveau-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      :disabled="isThisMigrationRunning"
      @update:complete="publicatieNiveauMappingsComplete = $event"
    />

    <documenttype-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      :disabled="isThisMigrationRunning"
      @update:complete="documenttypeMappingsComplete = $event"
    />

    <vertrouwelijkheid-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      :disabled="isThisMigrationRunning"
      @update:complete="vertrouwelijkheidMappingsComplete = $event"
    />

    <pdf-informatieobjecttype-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      :disabled="isThisMigrationRunning"
      @update:complete="generatedPdfMappingComplete = $event"
    />

    <menu class="reset" v-if="!error && !isThisMigrationRunning && canStartMigration">
      <li>
        <start-migration-button
          :det-zaaktype-id="detZaaktypeId"
          :zaaktype-naam="zaaktypeMapping?.detZaaktype?.naam ?? ''"
        />
      </li>
      <li>
        <start-partial-migration-button
          :det-zaaktype-id="detZaaktypeId"
          :zaaktype-naam="zaaktypeMapping?.detZaaktype?.naam ?? ''"
        />
      </li>
    </menu>
  </template>

  <migration-history-table v-if="!error" :det-zaaktype-id="detZaaktypeId" />
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import AlertInline from "@/components/AlertInline.vue";
import StatusMappingSection from "@/components/StatusMappingSection.vue";
import BesluittypeMappingSection from "@/components/BesluittypeMappingSection.vue";
import StartMigrationButton from "@/components/StartMigrationButton.vue";
import StartPartialMigrationButton from "@/components/StartPartialMigrationButton.vue";
import ResultaattypeMappingSection from "@/components/ResultaattypeMappingSection.vue";
import PublicatieNiveauMappingSection from "@/components/PublicatieNiveauMappingSection.vue";
import DocumenttypeMappingSection from "@/components/DocumenttypeMappingSection.vue";
import VertrouwelijkheidMappingSection from "@/components/VertrouwelijkheidMappingSection.vue";
import PdfInformatieobjecttypeMappingSection from "@/components/PdfInformatieobjecttypeMappingSection.vue";
import MigrationHistoryTable from "@/components/MigrationHistoryTable.vue";
import ZaaktypeMappingSection, {
  type ZaaktypeMappingModel
} from "@/components/ZaaktypeMappingSection.vue";
import { useMigration } from "@/composables/migration-store";
import { useGeneralConfig } from "@/composables/use-general-config";
import { MigrationStatus } from "@/types/datamigratie";
import { featureFlags } from "@/config/featureFlags";
import AutoSelecter from "@/components/AutoSelecter.vue";
const { detZaaktypeId } = defineProps<{ detZaaktypeId: string }>();

const route = useRoute();
const search = computed(() => String(route.query.search || "").trim());

const zaaktypeMapping = ref<ZaaktypeMappingModel>();
const detZaaktypeNaam = ref<string>("");

const statusMappingsComplete = ref(false);
const resultaattypeMappingsComplete = ref(false);
const besluittypeMappingsComplete = ref(false);
const publicatieNiveauMappingsComplete = ref(false);
const documenttypeMappingsComplete = ref(false);
const vertrouwelijkheidMappingsComplete = ref(false);
const generatedPdfMappingComplete = ref(false);

const { error, migration } = useMigration();
const isThisMigrationRunning = computed(
  () =>
    migration.value?.status === MigrationStatus.inProgress &&
    migration.value.detZaaktypeId === detZaaktypeId
);

const {
  isGeneralConfigComplete,
  checkGeneralConfig,
  loading: isGeneralConfigLoading
} = useGeneralConfig();

onMounted(() => {
  checkGeneralConfig();
});

const allIsComplete = computed(
  () =>
    statusMappingsComplete.value &&
    besluittypeMappingsComplete.value &&
    resultaattypeMappingsComplete.value &&
    publicatieNiveauMappingsComplete.value &&
    documenttypeMappingsComplete.value &&
    vertrouwelijkheidMappingsComplete.value &&
    generatedPdfMappingComplete.value
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
