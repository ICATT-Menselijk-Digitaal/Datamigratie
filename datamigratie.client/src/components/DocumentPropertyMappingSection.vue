<template>
  <div class="document-property-mapping-section">
    <mapping-grid
      v-model="publicatieNiveauMappingsModel"
      title="Publicatieniveau"
      description="Koppel de e-Suite publicatieniveaus voor documenten aan de Open Zaak vertrouwelijkheidaanduiding."
      source-label="e-Suite Publicatieniveau"
      target-label="Open Zaak Vertrouwelijkheidaanduiding"
      :source-items="publicatieNiveauSourceItems"
      :target-items="vertrouwelijkheidaanduidingTargetItems"
      :all-mapped="allPublicatieNiveauMapped"
      :is-editing="publicatieniveauIsInEditMode"
      :disabled="disabled"
      :loading="isLoading"
      empty-message="Er zijn geen publicatieniveaus beschikbaar."
      target-placeholder="- Kies een vertrouwelijkheidaanduiding -"
      save-button-text="Mapping opslaan"
      cancel-button-text="Annuleren"
      edit-button-text="Mapping aanpassen"
      :show-edit-button="true"
      :show-warning="false"
      :collapsible="true"
      :show-collapse-warning="!allPublicatieNiveauMapped"
      @save="handleSavePublicatieNiveau"
      @cancel="handleCancelPublicatieNiveau"
      @edit="forceEditPublicatieniveau = true"
    />

    <mapping-grid
      v-model="documenttypeMappingsModel"
      title="Documenttype"
      description="Koppel de e-Suite documenttypes aan de Open Zaak informatieobjecttypes."
      source-label="e-Suite Documenttype"
      target-label="Open Zaak Informatieobjecttype"
      :source-items="documenttypeSourceItems"
      :target-items="informatieobjecttypeTargetItems"
      :all-mapped="allDocumenttypeMapped"
      :is-editing="documenttypeIsInEditMode"
      :disabled="disabled"
      :loading="isLoading"
      empty-message="Er zijn geen documenttypes beschikbaar."
      target-placeholder="- Kies een informatieobjecttype -"
      save-button-text="Mapping opslaan"
      cancel-button-text="Annuleren"
      edit-button-text="Mapping aanpassen"
      :show-edit-button="true"
      :show-warning="false"
      :collapsible="true"
      :show-collapse-warning="!allDocumenttypeMapped"
      @save="handleSaveDocumenttype"
      @cancel="handleCancelDocumenttype"
      @edit="forceEditDocumenttype = true"
    >
      <template #extra-content>
        <!-- only shown when feature flag is enabled -->
        <div
          v-if="featureFlags.showDocumenttypeTestHelper && documenttypeSourceItems.length > 0"
          class="test-helper"
        >
          <label>
            <input
              type="checkbox"
              :disabled="!documenttypeIsInEditMode"
              @change="fillRandomDocumenttypeMappings($event)"
            />
            <span style="color: #e74c3c; font-weight: bold"
              >for testing: check to autofill with random selections</span
            >
          </label>
        </div>
      </template>
    </mapping-grid>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import type { DETZaaktype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";
import { get, post } from "@/utils/fetchWrapper";
import { featureFlags } from "@/config/featureFlags";
import toast from "./toast/toast";

type DocumentPropertyMappingItem = {
  detPropertyName: string;
  detValue: string;
  ozValue: string | null;
};

type DocumentPropertyMappingResponse = {
  detPropertyName: string;
  detValue: string;
  ozValue: string;
};

type SaveDocumentPropertyMappingsRequest = {
  mappings: DocumentPropertyMappingItem[];
};

interface Props {
  mappingId: string;
  detZaaktype: DETZaaktype;
  ozZaaktype: OZZaaktype;
  disabled: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false,
  showWarning: true
});

const emit = defineEmits<{
  (e: "update:complete", value: boolean): void;
}>();

const mappingsFromServer = ref<DocumentPropertyMappingItem[]>([]);

const PUBLICATIENIVEAU = "publicatieniveau";
const DOCUMENTTYPE = "documenttype";

const validPublicatieNiveauMappings = computed(() =>
  (props.detZaaktype.publicatieNiveauOptions ?? []).map((option) => ({
    sourceId: option.id,
    targetId:
      mappingsFromServer.value.find(
        ({ detPropertyName, detValue }) =>
          detPropertyName === PUBLICATIENIVEAU && detValue === option.id
      )?.ozValue || null
  }))
);

const validDocumenttypeMappings = computed(
  () =>
    props.detZaaktype.documenttypen?.map(({ naam }) => ({
      sourceId: naam,
      targetId:
        mappingsFromServer.value.find(
          ({ detPropertyName, detValue }) => detPropertyName === DOCUMENTTYPE && detValue === naam
        )?.ozValue || null
    })) || []
);

const forceEditPublicatieniveau = ref(false);
const publicatieniveauIsInEditMode = computed(
  () => forceEditPublicatieniveau.value || !allPublicatieNiveauMapped.value
);

const forceEditDocumenttype = ref(false);
const documenttypeIsInEditMode = computed(
  () => forceEditDocumenttype.value || !allDocumenttypeMapped.value
);
const isLoading = ref(false);

const fetchMappings = async () => {
  isLoading.value = true;
  try {
    mappingsFromServer.value = await get<DocumentPropertyMappingResponse[]>(
      `/api/mappings/${props.mappingId}/documentproperties`
    );
  } catch (error) {
    toast.add({ text: `Fout bij ophalen van de document mappings - ${error}`, type: "error" });
  } finally {
    isLoading.value = false;
  }
};

const saveMappings = async () => {
  isLoading.value = true;
  try {
    const mappingsToSave = [
      ...documenttypeMappingsModel.value.map(({ sourceId, targetId }) => ({
        detValue: sourceId,
        ozValue: targetId,
        detPropertyName: DOCUMENTTYPE
      })),
      ...publicatieNiveauMappingsModel.value.map(({ sourceId, targetId }) => ({
        detValue: sourceId,
        ozValue: targetId,
        detPropertyName: PUBLICATIENIVEAU
      }))
    ].filter(({ ozValue }) => ozValue);

    await post(`/api/mappings/${props.mappingId}/documentproperties`, {
      mappings: mappingsToSave
    } as SaveDocumentPropertyMappingsRequest);

    await fetchMappings();
  } catch (error) {
    console.error("Error saving document property mappings:", error);
  } finally {
    isLoading.value = false;
  }
};

const publicatieNiveauSourceItems = computed<MappingItem[]>(
  () => props.detZaaktype.publicatieNiveauOptions ?? []
);

const vertrouwelijkheidaanduidingTargetItems = computed<MappingItem[]>(
  () => props.ozZaaktype.ozDocumentVertrouwelijkheidaanduidingen ?? []
);

const documenttypeSourceItems = computed<MappingItem[]>(() => {
  if (!props.detZaaktype.documenttypen) return [];
  return props.detZaaktype.documenttypen
    .filter((dt) => dt.actief)
    .map((dt) => ({
      id: dt.naam,
      name: dt.naam,
      description: dt.omschrijving
    }));
});

const informatieobjecttypeTargetItems = computed<MappingItem[]>(() => {
  if (!props.ozZaaktype?.informatieobjecttypen) return [];
  return props.ozZaaktype.informatieobjecttypen.map((iot) => ({
    id: iot.url,
    name: iot.omschrijving,
    description: undefined
  }));
});

// check if all are mapped
const allPublicatieNiveauMapped = computed(
  () =>
    validPublicatieNiveauMappings.value.length > 0 &&
    validPublicatieNiveauMappings.value.every(({ targetId }) => targetId)
);

const allDocumenttypeMapped = computed(
  () =>
    validDocumenttypeMappings.value.length > 0 &&
    validDocumenttypeMappings.value.every(({ targetId }) => targetId)
);

const isComplete = computed(
  () => !publicatieniveauIsInEditMode.value && !documenttypeIsInEditMode.value
);

watch(isComplete, (value) => {
  emit("update:complete", value);
});

// mapping for MappingGrid
const publicatieNiveauMappingsModel = ref<Mapping[]>([]);

const documenttypeMappingsModel = ref<Mapping[]>([]);

const handleSavePublicatieNiveau = async () => {
  await saveMappings();
  forceEditPublicatieniveau.value = false;
};

const handleCancelPublicatieNiveau = () => {
  fetchMappings();
  forceEditPublicatieniveau.value = false;
};

const handleSaveDocumenttype = async () => {
  await saveMappings();
  forceEditDocumenttype.value = false;
};

const handleCancelDocumenttype = () => {
  fetchMappings();
  forceEditDocumenttype.value = false;
};

// fills documenttype mappings with random selections (when VITE_ENABLE_TEST_HELPERS=true)
const fillRandomDocumenttypeMappings = (event: Event) => {
  const checkbox = event.target as HTMLInputElement;

  if (checkbox.checked && informatieobjecttypeTargetItems.value.length > 0) {
    const randomMappings = documenttypeSourceItems.value.map((sourceItem) => {
      const randomIndex = Math.floor(Math.random() * informatieobjecttypeTargetItems.value.length);
      const randomTarget = informatieobjecttypeTargetItems.value[randomIndex];

      return {
        sourceId: sourceItem.id,
        targetId: randomTarget.id
      };
    });

    documenttypeMappingsModel.value = randomMappings;
  } else {
    const clearedMappings = documenttypeSourceItems.value.map((sourceItem) => ({
      sourceId: sourceItem.id,
      targetId: null
    }));

    documenttypeMappingsModel.value = clearedMappings;
  }
};

watch(isComplete, (c) => emit("update:complete", c));

// trigger fetching mappings whenever the mapping id or target zaaktype changes
watch(
  [() => props.mappingId, () => props.ozZaaktype],
  () => {
    fetchMappings();
  },
  { immediate: true }
);

watch(validDocumenttypeMappings, (value) => {
  documenttypeMappingsModel.value = value;
});

watch(validPublicatieNiveauMappings, (value) => {
  publicatieNiveauMappingsModel.value = value;
});
</script>

<style lang="scss" scoped>
.document-property-mapping-section {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-small);
  align-self: stretch;
  width: 100%;

  // Remove bottom margin from nested collapsible sections since parent has gap
  :deep(.collapsible-mapping-section) {
    margin-block-end: 0;
  }

  // Add margin-bottom only to the last collapsible section
  // to maintain proper spacing with the next section (Vertrouwelijkheid)
  :deep(.collapsible-mapping-section:last-child) {
    margin-block-end: var(--spacing-small);
  }
}

.test-helper {
  padding: var(--spacing-default);
  background-color: var(--marked);
  border: 2px dashed var(--accent);
  border-radius: var(--radius-default);
  margin-bottom: var(--spacing-default);

  label {
    display: flex;
    align-items: center;
    gap: var(--spacing-small);
    cursor: pointer;
    margin: 0;

    input[type="checkbox"] {
      cursor: pointer;
    }

    span {
      color: var(--code);
      font-weight: var(--font-bold);
    }
  }
}
</style>
