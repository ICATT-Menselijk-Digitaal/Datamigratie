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

    <status-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      @update:complete="statusMappingsComplete = $event"
    />

    <resultaattype-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      @update:complete="resultaattypeMappingsComplete = $event"
    />

    <besluittype-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      @update:complete="besluittypeMappingsComplete = $event"
    />

    <document-publicatie-niveau-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      @update:complete="publicatieNiveauMappingsComplete = $event"
    />

    <document-type-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
      @update:complete="documentTypeMappingsComplete = $event"
    />

    <vertrouwelijkheid-mapping-section
      :mapping-id="zaaktypeMapping.id"
      :det-zaaktype="zaaktypeMapping.detZaaktype"
      :oz-zaaktype="zaaktypeMapping.ozZaaktype"
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
import { computed, ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import PromptModal from "@/components/PromptModal.vue";
import AlertInline from "@/components/AlertInline.vue";
import StatusMappingSection from "@/components/StatusMappingSection.vue";
import BesluittypeMappingSection from "@/components/BesluittypeMappingSection.vue";
import { useMigrationControl } from "@/composables/use-migration-control";
import ResultaattypeMappingSection from "@/components/ResultaattypeMappingSection.vue";
import VertrouwelijkheidMappingSection from "@/components/VertrouwelijkheidMappingSection.vue";
import MigrationHistoryTable from "@/components/MigrationHistoryTable.vue";
import ZaaktypeMappingSection, {
  type ZaaktypeMappingModel
} from "@/components/ZaaktypeMappingSection.vue";
import { useMigration } from "@/composables/migration-store";
import { useGeneralConfig } from "@/composables/use-general-config";
import { MigrationStatus } from "@/types/datamigratie";
import DocumentPublicatieNiveauMappingSection from "@/components/DocumentPublicatieNiveauMappingSection.vue";
import DocumentTypeMappingSection from "@/components/DocumentTypeMappingSection.vue";
const { detZaaktypeId } = defineProps<{ detZaaktypeId: string }>();

const route = useRoute();
const search = computed(() => String(route.query.search || "").trim());

const zaaktypeMapping = ref<ZaaktypeMappingModel>();
const detZaaktypeNaam = ref<string>("");

const statusMappingsComplete = ref(false);
const resultaattypeMappingsComplete = ref(false);
const besluittypeMappingsComplete = ref(false);
const publicatieNiveauMappingsComplete = ref(false);
const documentTypeMappingsComplete = ref(false);
const vertrouwelijkheidMappingsComplete = ref(false);

const { error, migration } = useMigration();
const { isThisMigrationRunning, confirmDialog, startMigration } = useMigrationControl(
  () => detZaaktypeId
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
    documentTypeMappingsComplete.value &&
    vertrouwelijkheidMappingsComplete.value &&
    isGeneralConfigComplete.value
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
