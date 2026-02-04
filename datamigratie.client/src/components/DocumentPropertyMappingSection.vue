<template>
  <div v-if="showMapping" class="document-property-mapping-section">
    <mapping-grid
      v-model="publicatieNiveauMappingsModel"
      title="Document publicatieniveau mapping"
      description="Koppel de e-Suite publicatieniveaus voor documenten aan de Open Zaak vertrouwelijkheidaanduiding."
      source-label="e-Suite Publicatieniveau"
      target-label="Open Zaak Vertrouwelijkheidaanduiding"
      :source-items="publicatieNiveauSourceItems"
      :target-items="vertrouwelijkheidaanduidingTargetItems"
      :all-mapped="allPublicatieNiveauMapped"
      :is-editing="isEditingPublicatieniveau"
      :disabled="disabled"
      :loading="showLoading"
      empty-message="Er zijn geen publicatieniveaus beschikbaar."
      target-placeholder="Kies een vertrouwelijkheidaanduiding"
      save-button-text="Publicatieniveau mappings opslaan"
      edit-button-text="Publicatieniveau mappings aanpassen"
      :show-edit-button="true"
      :show-warning="showWarning && !allPublicatieNiveauMapped"
      warning-message="Niet alle publicatieniveaus zijn gekoppeld."
      @save="handleSavePublicatieNiveau"
      @edit="handleEditPublicatieNiveau"
    />

    <div class="documenttype-section">
      <!-- only shown when feature flag is enabled -->
      <div
        v-if="
          featureFlags.showDocumenttypeTestHelper &&
          isEditingDocumenttype &&
          documenttypeSourceItems.length > 0
        "
        class="test-helper"
      >
        <label>
          <input type="checkbox" @change="fillRandomDocumenttypeMappings($event)" />
          <span style="color: #e74c3c; font-weight: bold"
            >for testing: check to autofill with random selections</span
          >
        </label>
      </div>

      <mapping-grid
        v-model="documenttypeMappingsModel"
        title="Documenttype mapping"
        description="Koppel de e-Suite documenttypes aan de Open Zaak informatieobjecttypes."
        source-label="e-Suite Documenttype"
        target-label="Open Zaak Informatieobjecttype"
        :source-items="documenttypeSourceItems"
        :target-items="informatieobjecttypeTargetItems"
        :all-mapped="allDocumenttypeMapped"
        :is-editing="isEditingDocumenttype"
        :disabled="disabled"
        :loading="showLoading"
        empty-message="Er zijn geen documenttypes beschikbaar."
        target-placeholder="Kies een informatieobjecttype"
        save-button-text="Documenttype mappings opslaan"
        edit-button-text="Documenttype mappings aanpassen"
        :show-edit-button="true"
        :show-warning="showWarning && !allDocumenttypeMapped"
        warning-message="Niet alle documenttypes zijn gekoppeld."
        @save="handleSaveDocumenttype"
        @edit="handleEditDocumenttype"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import type { DetDocumenttype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";
import { get, post } from "@/utils/fetchWrapper";
import { featureFlags } from "@/config/featureFlags";

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

type VertrouwelijkheidaanduidingOption = {
  value: string;
  label: string;
};

interface Props {
  mappingId?: string;
  detDocumenttypen?: DetDocumenttype[];
  ozZaaktype?: OZZaaktype;
  disabled?: boolean;
  showWarning?: boolean;
  showMapping: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false,
  showWarning: true
});

const emit = defineEmits<{
  (e: "update:complete", value: boolean): void;
  (e: "update:editingPublicatieniveau", value: boolean): void;
  (e: "update:editingDocumenttype", value: boolean): void;
}>();

const mappings = ref<DocumentPropertyMappingItem[]>([]);
const publicatieNiveauOptions = ref<string[]>([]);
const vertrouwelijkheidaanduidingOptions = ref<{ value: string; label: string }[]>([]);
const isEditingPublicatieniveau = ref(false);
const isEditingDocumenttype = ref(false);
const isLoading = ref(false);
const isFetching = ref(false);

const isDataLoaded = computed(() => {
  return !!(
    publicatieNiveauOptions.value.length > 0 &&
    vertrouwelijkheidaanduidingOptions.value.length > 0 &&
    props.ozZaaktype?.informatieobjecttypen
  );
});

const showLoading = computed(() => {
  return isLoading.value || !isDataLoaded.value;
});

const detDocumenttypen = computed(() => {
  if (!props.detDocumenttypen) return [];
  return props.detDocumenttypen.filter((dt) => dt.actief);
});

const fetchMappings = async () => {
  if (!props.ozZaaktype?.id || isFetching.value) return;

  isFetching.value = true;
  isLoading.value = true;
  try {
    publicatieNiveauOptions.value = await get<string[]>(`/api/det/options/publicatieniveau`);
    vertrouwelijkheidaanduidingOptions.value = await get<VertrouwelijkheidaanduidingOption[]>(
      `/api/oz/options/vertrouwelijkheidaanduiding`
    );

    const savedMappings = props.mappingId
      ? await get<DocumentPropertyMappingResponse[]>(
          `/api/mappings/${props.mappingId}/documentproperties`
        )
      : [];

    const publicatieNiveauMappingsArray = publicatieNiveauOptions.value.map((niveau) => {
      const existing = savedMappings.find(
        (m) => m.detPropertyName === "publicatieniveau" && m.detValue === niveau
      );
      return {
        detPropertyName: "publicatieniveau",
        detValue: niveau,
        ozValue: existing?.ozValue ?? null
      };
    });

    const documenttypeMappingsArray = detDocumenttypen.value.map((dt) => {
      const existing = savedMappings.find(
        (m) => m.detPropertyName === "documenttype" && m.detValue === dt.naam
      );
      return {
        detPropertyName: "documenttype",
        detValue: dt.naam,
        ozValue: existing?.ozValue ?? null
      };
    });

    mappings.value = [...publicatieNiveauMappingsArray, ...documenttypeMappingsArray];

    // decide which sections should be in edit mode
    const publicatieNiveauComplete = publicatieNiveauMappingsArray.every((m) => m.ozValue !== null);
    const documenttypeComplete = documenttypeMappingsArray.every((m) => m.ozValue !== null);

    const hasPublicatieNiveauSaved = savedMappings.some(
      (m) => m.detPropertyName === "publicatieniveau"
    );
    const hasDocumenttypeSaved = savedMappings.some((m) => m.detPropertyName === "documenttype");

    isEditingPublicatieniveau.value = !publicatieNiveauComplete || !hasPublicatieNiveauSaved;
    isEditingDocumenttype.value = !documenttypeComplete || !hasDocumenttypeSaved;

    emit("update:editingPublicatieniveau", isEditingPublicatieniveau.value);
    emit("update:editingDocumenttype", isEditingDocumenttype.value);
    emit("update:complete", isComplete.value);
  } catch (error) {
    console.error("Error fetching document property mappings:", error);
  } finally {
    isFetching.value = false;
    isLoading.value = false;
  }
};

const saveMappings = async () => {
  if (!props.mappingId) return;

  isLoading.value = true;
  try {
    const mappingsToSave = mappings.value.filter((m) => m.ozValue !== null);
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

watch(
  () => [props.ozZaaktype?.id, props.detDocumenttypen],
  () => {
    fetchMappings();
  },
  { immediate: true, deep: true }
);

watch(isEditingPublicatieniveau, (value) => {
  emit("update:editingPublicatieniveau", value);
});

watch(isEditingDocumenttype, (value) => {
  emit("update:editingDocumenttype", value);
});

defineExpose({
  fetchMappings,
  setEditingPublicatieniveau: (value: boolean) => {
    isEditingPublicatieniveau.value = value;
  },
  setEditingDocumenttype: (value: boolean) => {
    isEditingDocumenttype.value = value;
  }
});

const publicatieNiveauSourceItems = computed<MappingItem[]>(() => {
  return publicatieNiveauOptions.value.map((option) => ({
    id: option,
    name: option.charAt(0).toUpperCase() + option.slice(1),
    description: undefined
  }));
});

const vertrouwelijkheidaanduidingTargetItems = computed<MappingItem[]>(() => {
  return vertrouwelijkheidaanduidingOptions.value.map((option) => ({
    id: option.value,
    name: option.label,
    description: undefined
  }));
});

const documenttypeSourceItems = computed<MappingItem[]>(() => {
  if (!props.detDocumenttypen) return [];
  return props.detDocumenttypen
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

const publicatieNiveauMappings = computed(() =>
  mappings.value.filter((m) => m.detPropertyName === "publicatieniveau")
);

const documenttypeMappings = computed(() =>
  mappings.value.filter((m) => m.detPropertyName === "documenttype")
);

// check if all are mapped
const allPublicatieNiveauMapped = computed(() => {
  const mappedValues = new Set(publicatieNiveauMappings.value.map((m) => m.detValue));
  return publicatieNiveauSourceItems.value.every(
    (item) =>
      mappedValues.has(item.id) &&
      publicatieNiveauMappings.value.find((m) => m.detValue === item.id)?.ozValue !== null
  );
});

const allDocumenttypeMapped = computed(() => {
  const mappedValues = new Set(documenttypeMappings.value.map((m) => m.detValue));
  return documenttypeSourceItems.value.every(
    (item) =>
      mappedValues.has(item.id) &&
      documenttypeMappings.value.find((m) => m.detValue === item.id)?.ozValue !== null
  );
});

const isComplete = computed(() => allPublicatieNiveauMapped.value && allDocumenttypeMapped.value);

watch(isComplete, (value) => {
  emit("update:complete", value);
});

// mapping for MappingGrid
const publicatieNiveauMappingsModel = computed<Mapping[]>({
  get: () => {
    return publicatieNiveauMappings.value.map((m) => ({
      sourceId: m.detValue,
      targetId: m.ozValue
    }));
  },
  set: (newMappings: Mapping[]) => {
    const updatedPublicatieNiveau = newMappings.map((m) => ({
      detPropertyName: "publicatieniveau",
      detValue: m.sourceId,
      ozValue: m.targetId
    }));

    mappings.value = [...updatedPublicatieNiveau, ...documenttypeMappings.value];
    isEditingPublicatieniveau.value = true;
    emit("update:editingPublicatieniveau", true);
  }
});

const documenttypeMappingsModel = computed<Mapping[]>({
  get: () => {
    return documenttypeMappings.value.map((m) => ({
      sourceId: m.detValue,
      targetId: m.ozValue
    }));
  },
  set: (newMappings: Mapping[]) => {
    const updatedDocumenttypes = newMappings.map((m) => ({
      detPropertyName: "documenttype",
      detValue: m.sourceId,
      ozValue: m.targetId
    }));

    mappings.value = [...publicatieNiveauMappings.value, ...updatedDocumenttypes];
    isEditingDocumenttype.value = true;
    emit("update:editingDocumenttype", true);
  }
});

const handleSavePublicatieNiveau = async () => {
  await saveMappings();
  isEditingPublicatieniveau.value = false;
};

const handleSaveDocumenttype = async () => {
  await saveMappings();
  isEditingDocumenttype.value = false;
};

const handleEditPublicatieNiveau = () => {
  isEditingPublicatieniveau.value = true;
};

const handleEditDocumenttype = () => {
  isEditingDocumenttype.value = true;
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
</script>

<style lang="scss" scoped>
.document-property-mapping-section {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-large);
}

.documenttype-section {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-default);
}

.test-helper {
  padding: var(--spacing-default);
  background-color: var(--marked);
  border: 2px dashed var(--accent);
  border-radius: var(--radius-default);

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
