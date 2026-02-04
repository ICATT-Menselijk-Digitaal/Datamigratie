<template>
  <mapping-grid
    v-if="showMapping"
    v-model="mappingsModel"
    title="Resultaattype mapping"
    description="Koppel de e-Suite resultaattypen aan de Open Zaak resultaattypen."
    source-label="e-Suite Resultaattype"
    target-label="Open Zaak Resultaattype"
    :source-items="sourceResultaattypeItems"
    :target-items="targetResultaattypeItems"
    :all-mapped="allMapped"
    :is-editing="isEditing"
    :disabled="disabled"
    :loading="showLoading"
    empty-message="Er zijn geen resultaattypen beschikbaar voor dit zaaktype."
    target-placeholder="Kies een resultaattype"
    save-button-text="Resultaattypen mappings opslaan"
    :show-warning="showWarning"
    warning-message="Niet alle resultaattypen zijn gekoppeld. Migratie kan niet worden gestart."
    @save="saveMappings"
  />
</template>

<script setup lang="ts">
import { ref, computed, watch } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import toast from "@/components/toast/toast";
import type { DETZaaktype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";
import { get, post } from "@/utils/fetchWrapper";

type ResultaattypeMappingItem = {
  detResultaattypeNaam: string;
  ozResultaattypeId: string | null;
};

type ResultaattypeMappingResponse = {
  detResultaattypeNaam: string;
  ozResultaattypeId: string;
};

type SaveResultaattypeMappingsRequest = {
  mappings: ResultaattypeMappingItem[];
};

interface Props {
  mappingId?: string;
  detZaaktype?: DETZaaktype;
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
  (e: "update:editing", value: boolean): void;
}>();

const mappings = ref<ResultaattypeMappingItem[]>([]);
const isEditing = ref(false);
const isLoading = ref(false);
const isFetching = ref(false);

const isDataLoaded = computed(() => {
  return !!(props.detZaaktype?.resultaten && props.ozZaaktype?.resultaattypen);
});

const showLoading = computed(() => {
  return isLoading.value || !isDataLoaded.value;
});

const allMapped = computed(() => {
  return mappings.value.length > 0 && mappings.value.every((m) => m.ozResultaattypeId !== null);
});

const sourceResultaattypeItems = computed<MappingItem[]>(() => {
  if (!props.detZaaktype?.resultaten) return [];
  return props.detZaaktype.resultaten.map((resultaattype) => ({
    id: resultaattype.resultaat.naam,
    name: resultaattype.resultaat.naam,
    description: resultaattype.resultaat.omschrijving
  }));
});

const targetResultaattypeItems = computed<MappingItem[]>(() => {
  if (!props.ozZaaktype?.resultaattypen) return [];
  return props.ozZaaktype.resultaattypen.map((resultaattype) => ({
    id: resultaattype.id,
    name: resultaattype.omschrijving,
    description: undefined
  }));
});

const mappingsModel = computed<Mapping[]>({
  get: () => {
    return mappings.value.map((m) => ({
      sourceId: m.detResultaattypeNaam,
      targetId: m.ozResultaattypeId
    }));
  },
  set: (newMappings: Mapping[]) => {
    const updated = newMappings.map((m) => ({
      detResultaattypeNaam: m.sourceId,
      ozResultaattypeId: m.targetId
    }));
    mappings.value = updated;
    isEditing.value = true;
    emit("update:editing", true);
  }
});

const fetchMappings = async () => {
  if (!props.ozZaaktype?.id || isFetching.value) {
    mappings.value = [];
    emit("update:complete", false);
    return;
  }

  isFetching.value = true;
  isLoading.value = true;
  try {
    const existingMappings = props.mappingId
      ? await get<ResultaattypeMappingResponse[]>(`/api/mappings/${props.mappingId}/resultaattypen`)
      : [];

    const detResultaattypen = props.detZaaktype?.resultaten || [];

    mappings.value = detResultaattypen.map((detResultaattypen) => {
      const existingMapping = existingMappings.find(
        (m) => m.detResultaattypeNaam === detResultaattypen.resultaat.naam
      );
      return (
        existingMapping || {
          detResultaattypeNaam: detResultaattypen.resultaat.naam,
          ozResultaattypeId: null
        }
      );
    });

    const isComplete =
      mappings.value.length > 0 && mappings.value.every((m) => m.ozResultaattypeId !== null);
    emit("update:complete", isComplete);

    if (isComplete && existingMappings.length > 0) {
      isEditing.value = false;
      emit("update:editing", false);
    }
  } catch (error) {
    mappings.value = [];
    emit("update:complete", false);
    throw error;
  } finally {
    isFetching.value = false;
    isLoading.value = false;
  }
};

const saveMappings = async () => {
  isLoading.value = true;
  try {
    const mappingsToSave = mappings.value.filter((m) => m.ozResultaattypeId !== null);

    await post(`/api/mappings/${props.mappingId}/resultaattypen`, {
      mappings: mappingsToSave
    } as SaveResultaattypeMappingsRequest);

    toast.add({ text: "De resultaattype mappings zijn succesvol opgeslagen." });

    const isComplete =
      mappings.value.length > 0 && mappings.value.every((m) => m.ozResultaattypeId !== null);

    emit("update:complete", isComplete);
    isEditing.value = false;
    emit("update:editing", false);
  } catch (error) {
    toast.add({ text: `Fout bij opslaan van de resultaattype mappings - ${error}`, type: "error" });
    throw error;
  } finally {
    isLoading.value = false;
  }
};

watch(
  () => [props.ozZaaktype?.id, props.detZaaktype?.resultaten],
  () => {
    fetchMappings();
  },
  { immediate: true, deep: true }
);

defineExpose({
  fetchMappings,
  setEditing: (value: boolean) => {
    isEditing.value = value;
    emit("update:editing", value);
  }
});
</script>
