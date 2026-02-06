<template>
  <mapping-grid
    v-model="mappingsModel"
    title="Vertrouwelijkheid mapping"
    description="Koppel de e-Suite vertrouwelijkheid waarden aan de Open Zaak vertrouwelijkheidaanduiding."
    source-label="e-Suite Vertrouwelijk"
    target-label="Open Zaak Vertrouwelijkheidaanduiding"
    :source-items="sourceItems"
    :target-items="targetItems"
    :all-mapped="allMapped"
    :is-editing="isInEditMode"
    :disabled="disabled"
    :loading="isLoading"
    empty-message="Er zijn geen vertrouwelijkheid opties beschikbaar."
    target-placeholder="Kies een vertrouwelijkheidaanduiding"
    save-button-text="Vertrouwelijkheidmappings opslaan"
    :show-warning="true"
    warning-message="Niet alle vertrouwelijkheid opties zijn gekoppeld. Migratie kan niet worden gestart."
    @save="saveMappings"
    edit-button-text="Vertrouwelijkheidmappings aanpassen"
    :show-edit-button="true"
    @edit="forceEdit = true"
  />
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import toast from "@/components/toast/toast";
import { get, post } from "@/utils/fetchWrapper";

type VertrouwelijkheidMappingItem = {
  detVertrouwelijkheid: boolean;
  ozVertrouwelijkheidaanduiding: string | null;
};

type VertrouwelijkheidMappingResponse = {
  detVertrouwelijkheid: boolean;
  ozVertrouwelijkheidaanduiding: string;
};

type SaveVertrouwelijkheidMappingsRequest = {
  mappings: VertrouwelijkheidMappingItem[];
};

type VertrouwelijkheidaanduidingOption = {
  value: string;
  label: string;
};

interface Props {
  mappingId: string;
  disabled: boolean;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  (e: "update:complete", value: boolean): void;
}>();

// Fixed source items: True and False for the boolean e-Suite property
const detVertrouwelijkheidOptions: { value: boolean; label: string }[] = [
  { value: true, label: "Ja (Vertrouwelijk)" },
  { value: false, label: "Nee (Niet vertrouwelijk)" }
];

const ozVertrouwelijkheidOptions = ref<VertrouwelijkheidaanduidingOption[]>([]);
const mappingsFromServer = ref<VertrouwelijkheidMappingItem[]>([]);

const validMappings = computed(
  () =>
    detVertrouwelijkheidOptions.map(({ value }) => ({
      sourceId: String(value),
      targetId:
        mappingsFromServer.value.find((m) => m.detVertrouwelijkheid === value)
          ?.ozVertrouwelijkheidaanduiding || null
    }))
);

const isLoading = ref(false);
const forceEdit = ref(false);
const isInEditMode = computed(() => forceEdit.value || !allMapped.value);

const allMapped = computed(() => {
  return validMappings.value.length > 0 && validMappings.value.every((m) => m.targetId);
});

const isComplete = computed(() => !isInEditMode.value);

const sourceItems = computed<MappingItem[]>(() => {
  return detVertrouwelijkheidOptions.map((option) => ({
    id: String(option.value),
    name: option.label,
    description: undefined
  }));
});

const targetItems = computed<MappingItem[]>(() => {
  return ozVertrouwelijkheidOptions.value.map((option) => ({
    id: option.value,
    name: option.label,
    description: undefined
  }));
});

const mappingsModel = ref<Mapping[]>([]);

const fetchOzOptions = async () => {
  try {
    ozVertrouwelijkheidOptions.value = await get<VertrouwelijkheidaanduidingOption[]>(
      "/api/oz/options/vertrouwelijkheidaanduiding"
    );
  } catch (error) {
    toast.add({
      text: `Fout bij ophalen van de vertrouwelijkheidaanduiding opties - ${error}`,
      type: "error"
    });
    throw error;
  }
};

const fetchMappings = async () => {
  isLoading.value = true;
  try {
    mappingsFromServer.value = await get<VertrouwelijkheidMappingResponse[]>(
      `/api/mappings/${props.mappingId}/vertrouwelijkheid`
    );
  } catch (error) {
    toast.add({
      text: `Fout bij ophalen van de vertrouwelijkheid mappings - ${error}`,
      type: "error"
    });
    throw error;
  } finally {
    isLoading.value = false;
  }
};

const saveMappings = async () => {
  isLoading.value = true;
  try {
    const mappingsToSave = mappingsModel.value
      .filter((m) => m.targetId)
      .map(({ sourceId, targetId }) => ({
        detVertrouwelijkheid: sourceId === "true",
        ozVertrouwelijkheidaanduiding: targetId
      }));

    await post(`/api/mappings/${props.mappingId}/vertrouwelijkheid`, {
      mappings: mappingsToSave
    } as SaveVertrouwelijkheidMappingsRequest);

    toast.add({ text: "De vertrouwelijkheid mappings zijn succesvol opgeslagen." });

    // re-fetch mappings to check for completeness
    await fetchMappings();

    forceEdit.value = false;
  } catch (error) {
    toast.add({
      text: `Fout bij opslaan van de vertrouwelijkheid mappings - ${error}`,
      type: "error"
    });
    throw error;
  } finally {
    isLoading.value = false;
  }
};

// Fetch OZ options once on mount
onMounted(() => {
  fetchOzOptions();
});

// Trigger fetching mappings whenever the mapping id changes
watch(
  () => props.mappingId,
  () => {
    fetchMappings();
  },
  { immediate: true }
);

// Update the mapping model based on server data whenever it changes
watch(
  validMappings,
  (value) => {
    mappingsModel.value = value;
  },
  { immediate: true }
);

watch(isComplete, (v) => emit("update:complete", v));
</script>
