<template>
  <h1>e-Suite zaaktype</h1>

  <simple-spinner v-if="loading" />

  <form v-else @submit.prevent="submitMapping">
    <alert-inline v-if="errors.length"
      >Fout(en) bij ophalen gegevens - {{ errors.join(" | ") }}</alert-inline
    >

    <dl v-else-if="detZaaktype">
      <dt>Naam:</dt>
      <dd>{{ detZaaktype.naam }}</dd>

      <dt>Omschrijving:</dt>
      <dd>{{ detZaaktype.omschrijving }}</dd>

      <dt>Functionele identificatie:</dt>
      <dd>{{ detZaaktype.functioneleIdentificatie }}</dd>

      <dt>Actief:</dt>
      <dd>{{ detZaaktype.actief ? "Ja" : "Nee" }}</dd>

      <dt>Aantal gesloten zaken:</dt>
      <dd>{{ detZaaktype?.closedZakenCount }}</dd>

      <dt id="mapping">Koppeling OZ zaaktype:</dt>
      <dd v-if="!isEditingZaaktypeMapping && mapping.detZaaktypeId" class="mapping-display">
        {{ ozZaaktypes?.find((type) => type.id == mapping.ozZaaktypeId)?.identificatie }}
        <button
          type="button"
          class="secondary mapping-edit-button"
          @click="setEditingZaaktypeMapping(true)"
          v-if="!isThisMigrationRunning"
        >
          Mapping aanpassen
        </button>
      </dd>
      <dd v-else class="mapping-controls">
        <select
          name="ozZaaktypeId"
          aria-labelledby="mapping"
          v-model="mapping.ozZaaktypeId"
          required
        >
          <option v-if="!mapping.ozZaaktypeId" value="">Kies Open Zaak zaaktype</option>

          <option v-for="{ id, identificatie, omschrijving } in ozZaaktypes" :value="id" :key="id">
            {{ identificatie }} – {{ omschrijving }}
          </option>
        </select>
        <button type="submit" class="mapping-save-button">Mapping opslaan</button>
      </dd>
    </dl>

    <status-mapping-section
      ref="statusMappingSectionRef"
      :mapping-id="mapping.id"
      :det-zaaktype="detZaaktype"
      :oz-zaaktype="ozZaaktype"
      :disabled="isThisMigrationRunning"
      :show-warning="!!mapping.detZaaktypeId"
      :show-mapping="showDetailMapping"
      @update:complete="statusMappingsComplete = $event"
      @update:editing="isEditingStatusMapping = $event"
    />

    <menu class="reset edit-menu">
      <li v-if="canEditStatusMappings">
        <button type="button" class="secondary" @click="statusMappingSectionRef?.setEditing(true)">
          Statusmappings aanpassen
        </button>
      </li>
    </menu>

    <resultaattype-mapping-section
      ref="resultaattypeMappingSectionRef"
      :mapping-id="mapping.id"
      :det-zaaktype="detZaaktype"
      :oz-zaaktype="ozZaaktype"
      :disabled="isThisMigrationRunning"
      :show-warning="!!mapping.detZaaktypeId"
      :show-mapping="showDetailMapping"
      @update:complete="resultaattypeMappingsComplete = $event"
      @update:editing="isEditingResultaattypeMapping = $event"
    />

    <menu class="reset edit-menu">
      <li v-if="canEditResultaattypeMappings">
        <button
          type="button"
          class="secondary"
          @click="resultaattypeMappingSectionRef?.setEditing(true)"
        >
          Resultaattypemappings aanpassen
        </button>
      </li>
    </menu>

    <besluittype-mapping-section
      ref="besluittypeMappingSectionRef"
      :mapping-id="mapping.id"
      :oz-zaaktype="ozZaaktype"
      :disabled="isThisMigrationRunning"
      :show-warning="!!mapping.detZaaktypeId"
      :show-mapping="showDetailMapping"
      @update:complete="besluittypeMappingsComplete = $event"
      @update:editing="isEditingBesluittypeMapping = $event"
    />

    <menu class="reset edit-menu">
      <li v-if="canEditBesluittypeMappings">
        <button
          type="button"
          class="secondary"
          @click="besluittypeMappingSectionRef?.setEditing(true)"
        >
          Besluittypemappings aanpassen
        </button>
      </li>
    </menu>

    <document-property-mapping-section
      ref="documentPropertyMappingSectionRef"
      :mapping-id="mapping.id"
      :det-documenttypen="detZaaktype?.documenttypen"
      :oz-zaaktype="ozZaaktype"
      :disabled="isThisMigrationRunning"
      :show-warning="!!mapping.detZaaktypeId"
      :show-mapping="showDetailMapping"
      @update:complete="documentPropertyMappingsComplete = $event"
      @update:editing-publicatieniveau="isEditingPublicatieNiveauMapping = $event"
      @update:editing-documenttype="isEditingDocumenttypeMapping = $event"
    />

    <menu class="reset">
      <li>
        <router-link
          :to="{ name: 'detZaaktypes', ...(search && { query: { search } }) }"
          class="button button-secondary"
          >&lt; Terug</router-link
        >
      </li>

      <template v-if="!errors.length && !isThisMigrationRunning">
        <li v-if="canStartMigration">
          <button type="button" @click="startMigration">Start migratie</button>
        </li>
      </template>
    </menu>

    <prompt-modal
      :dialog="confirmDialog"
      cancel-text="Nee, niet migreren"
      confirm-text="Ja, start migratie"
    >
      <h2>Migratie starten</h2>

      <p>
        Weet je zeker dat je de migratie van zaken van het e-Suite zaaktype
        <em>{{ detZaaktype?.naam }}</em> wilt starten?
      </p>
    </prompt-modal>

    <zaaktype-change-confirmation-modal
      :dialog="confirmOzZaaktypeChangeDialog"
      warning-text="Als je het Open Zaak zaaktype wijzigt, worden alle bestaande mappings verwijderd."
      description-text="Je moet de mappings opnieuw configureren voor het nieuwe zaaktype."
    />

    <migration-history-table
      v-if="!errors.length"
      :history="migrationHistory"
      @row-click="(id) => navigateToMigrationDetail(id, search)"
    />
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import { useRoute } from "vue-router";
import { useConfirmDialog } from "@vueuse/core";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import PromptModal from "@/components/PromptModal.vue";
import StatusMappingSection from "@/components/StatusMappingSection.vue";
import BesluittypeMappingSection from "@/components/BesluittypeMappingSection.vue";
import ZaaktypeChangeConfirmationModal from "@/components/ZaaktypeChangeConfirmationModal.vue";
import type { OZZaaktype } from "@/services/ozService";
import { ozService } from "@/services/ozService";
import { MigrationStatus } from "@/types/datamigratie";
import { useMigration } from "@/composables/use-migration-status";
import { useZaaktypeMapping } from "@/composables/use-zaaktype-mapping";
import { useMigrationControl } from "@/composables/use-migration-control";
import ResultaattypeMappingSection from "@/components/ResultaattypeMappingSection.vue";
import DocumentPropertyMappingSection from "@/components/DocumentPropertyMappingSection.vue";
import MigrationHistoryTable from "@/components/MigrationHistoryTable.vue";

const { detZaaktypeId } = defineProps<{ detZaaktypeId: string }>();

const route = useRoute();
const search = computed(() => String(route.query.search || "").trim());

const zaaktypeMappingComposable = useZaaktypeMapping(detZaaktypeId);
const {
  detZaaktype,
  ozZaaktypes,
  mapping,
  migrationHistory,
  isEditing: isEditingZaaktypeMapping,
  isLoading: loading,
  errors,
  setEditing: setEditingZaaktypeMapping,
  fetchData: fetchMappingData,
  saveMapping,
  hasExistingMappings,
  hasZaaktypeChanged,
  revertMapping
} = zaaktypeMappingComposable;

const ozZaaktype = ref<OZZaaktype>();

const statusMappingSectionRef = ref<InstanceType<typeof StatusMappingSection>>();
const resultaattypeMappingSectionRef = ref<InstanceType<typeof ResultaattypeMappingSection>>();
const besluittypeMappingSectionRef = ref<InstanceType<typeof BesluittypeMappingSection>>();
const documentPropertyMappingSectionRef =
  ref<InstanceType<typeof DocumentPropertyMappingSection>>();

const showDetailMapping = computed(
  () => !!(mapping.value.detZaaktypeId && mapping.value.ozZaaktypeId)
);

const { migration, fetchMigration } = useMigration();

const mappingId = computed(() => (mapping.value.id ? +mapping.value.id : undefined));
const ozZaaktypeId = computed(() => mapping.value.ozZaaktypeId);

const statusMappingsComplete = ref(false);
const isEditingStatusMapping = ref(false);

const resultaattypeMappingsComplete = ref(false);
const isEditingResultaattypeMapping = ref(false);

const besluittypeMappingsComplete = ref(false);
const isEditingBesluittypeMapping = ref(false);

const documentPropertyMappingsComplete = ref(false);
const isEditingPublicatieNiveauMapping = ref(false);
const isEditingDocumenttypeMapping = ref(false);

const migrationControlComposable = useMigrationControl(
  detZaaktypeId,
  mapping,
  migration,
  isEditingZaaktypeMapping,
  isEditingStatusMapping,
  statusMappingsComplete,
  isEditingResultaattypeMapping,
  resultaattypeMappingsComplete,
  isEditingBesluittypeMapping,
  besluittypeMappingsComplete,
  isEditingPublicatieNiveauMapping,
  isEditingDocumenttypeMapping,
  documentPropertyMappingsComplete,
  fetchMappingData
);

const {
  isThisMigrationRunning,
  canStartMigration,
  confirmDialog,
  startMigration: startMigrationAction,
  navigateToMigrationDetail
} = migrationControlComposable;

const confirmOzZaaktypeChangeDialog = useConfirmDialog();

const canEditStatusMappings = computed(
  () =>
    !isThisMigrationRunning.value &&
    mapping.value.detZaaktypeId &&
    mapping.value.ozZaaktypeId &&
    !isEditingZaaktypeMapping.value &&
    !isEditingStatusMapping.value &&
    statusMappingsComplete.value
);

const canEditResultaattypeMappings = computed(
  () =>
    !isThisMigrationRunning.value &&
    mapping.value.detZaaktypeId &&
    mapping.value.ozZaaktypeId &&
    !isEditingZaaktypeMapping.value &&
    !isEditingResultaattypeMapping.value &&
    resultaattypeMappingsComplete.value
);

const canEditBesluittypeMappings = computed(
  () =>
    !isThisMigrationRunning.value &&
    mapping.value.detZaaktypeId &&
    mapping.value.ozZaaktypeId &&
    !isEditingZaaktypeMapping.value &&
    !isEditingBesluittypeMapping.value &&
    besluittypeMappingsComplete.value
);

const canEditDocumentPropertyMappings = computed(
  () =>
    !isThisMigrationRunning.value &&
    mapping.value.detZaaktypeId &&
    mapping.value.ozZaaktypeId &&
    !isEditingZaaktypeMapping.value &&
    (!isEditingPublicatieNiveauMapping.value || !isEditingDocumenttypeMapping.value) &&
    documentPropertyMappingsComplete.value
);

const submitMapping = async () => {
  const hasStatusMappings = statusMappingsComplete.value;
  const hasResultaattypeMappings = resultaattypeMappingsComplete.value;
  const hasBesluittypeMappings = besluittypeMappingsComplete.value;
  const hasDocumentPropertyMappings = documentPropertyMappingsComplete.value;

  // Check if zaaktype is being changed and there are existing mappings
  if (
    mapping.value.detZaaktypeId &&
    hasZaaktypeChanged() &&
    hasExistingMappings({
      hasStatusMappings,
      hasResultaattypeMappings,
      hasBesluittypeMappings,
      hasDocumentPropertyMappings
    })
  ) {
    const result = await confirmOzZaaktypeChangeDialog.reveal();

    if (result.isCanceled) {
      revertMapping();
      return;
    }
  }

  // Save the mapping with callback to refresh related mappings if zaaktype changed.
  // When the OpenZaak zaaktype changes, all child mappings (status, resultaattype,
  // besluittype, document properties) must be deleted because they depend on
  // the specific zaaktype's available options (statustypen, resultaattypen, etc.)
  await saveMapping(async () => {
    try {
      await Promise.all([
        statusMappingSectionRef.value?.fetchMappings() ?? Promise.resolve(),
        resultaattypeMappingSectionRef.value?.fetchMappings() ?? Promise.resolve(),
        besluittypeMappingSectionRef.value?.fetchMappings() ?? Promise.resolve(),
        documentPropertyMappingSectionRef.value?.fetchMappings() ?? Promise.resolve()
      ]);
    } catch (error) {
      // handled in respective components
    }
  });
};

const startMigration = () => startMigrationAction(fetchMigration);

watch(
  ozZaaktypeId,
  async (newId) => {
    if (newId) {
      try {
        ozZaaktype.value = await ozService.getZaaktypeById(newId);
      } catch (error) {
        ozZaaktype.value = undefined;
      }
    } else {
      ozZaaktype.value = undefined;
    }
  },
  { immediate: true }
);

onMounted(async () => {
  await fetchMappingData();
});
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

dl {
  display: grid;
  gap: var(--spacing-default);
  margin-block-end: var(--spacing-large);

  dt {
    color: var(--text);
    font-weight: 600;

    &[id] {
      align-self: center;
    }
  }

  dd {
    margin-inline: 0;

    &.mapping-display {
      display: flex;
      gap: var(--spacing-default);
      align-items: center;
      flex-wrap: wrap;

      .mapping-edit-button {
        white-space: nowrap;
        margin-block-end: 0;
      }
    }

    &.mapping-controls {
      display: flex;
      gap: var(--spacing-default);
      align-items: center;
      flex-wrap: wrap;

      select {
        flex: 1;
        min-width: 200px;
        max-width: 100%;

        option {
          white-space: normal;
          overflow-wrap: break-word;
          word-wrap: break-word;
          padding: var(--spacing-small);
        }
      }

      .mapping-save-button {
        white-space: nowrap;
        margin-block-end: 0;
      }
    }
  }

  select {
    min-inline-size: var(--section-width-small);
    margin-block-end: 0;
  }

  @media (min-width: variables.$breakpoint-md) {
    & {
      grid-template-columns: max-content 1fr;

      dd {
        grid-column: 2;
      }
    }
  }
}

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
