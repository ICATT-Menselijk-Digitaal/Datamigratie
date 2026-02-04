<template>
  <mapping-grid
    v-if="showMapping"
    v-model="mappingsModel"
    title="Besluittype mapping"
    description="Koppel de e-Suite besluittypen aan de Open Zaak besluittypen."
    source-label="e-Suite Besluittype"
    target-label="Open Zaak Besluittype"
    :source-items="sourceBesluittypeItems"
    :target-items="targetBesluittypeItems"
    :all-mapped="allMapped"
    :is-editing="isEditing"
    :disabled="disabled"
    :loading="showLoading"
    empty-message="Er zijn geen besluittypen beschikbaar voor dit zaaktype."
    target-placeholder="Kies een besluittype"
    save-button-text="Besluittypemappings opslaan"
    :show-warning="showWarning"
    warning-message="Niet alle besluittypen zijn gekoppeld. Migratie kan niet worden gestart."
    @save="saveMappings"
  />
</template>

<script setup lang="ts">
import { ref, computed, watch } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import toast from "@/components/toast/toast";
import type { DetBesluittype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";
import { get, post } from "@/utils/fetchWrapper";
import { detService } from "@/services/detService";

type BesluittypeMappingItem = {
  detBesluittypeNaam: string;
  ozBesluittypeId: string | null;
};

type BesluittypeMappingsResponse = {
  detBesluittypeNaam: string;
  ozBesluittypeId: string;
};

type SaveBesluittypeMappingsRequest = {
  mappings: BesluittypeMappingItem[];
};

interface Props {
  mappingId?: string;
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

// State
const mappings = ref<BesluittypeMappingItem[]>([]);
const detBesluittypen = ref<DetBesluittype[]>([]);
const isEditing = ref(false);
const isLoading = ref(false);
const isFetching = ref(false);

const isDataLoaded = computed(() => {
  return !!(detBesluittypen.value.length > 0 && props.ozZaaktype?.besluittypen);
});

const showLoading = computed(() => {
  return isLoading.value || !isDataLoaded.value;
});

// Computed
const allMapped = computed(() => {
  return mappings.value.length > 0 && mappings.value.every((m) => m.ozBesluittypeId !== null);
});

const sourceBesluittypeItems = computed<MappingItem[]>(() => {
  return detBesluittypen.value
    .filter((besluittype) => besluittype.actief)
    .map((besluittype) => ({
      id: besluittype.naam,
      name: besluittype.naam,
      description: besluittype.omschrijving
    }));
});

const targetBesluittypeItems = computed<MappingItem[]>(() => {
  if (!props.ozZaaktype?.besluittypen) return [];
  return props.ozZaaktype.besluittypen.map((besluittype) => ({
    id: besluittype.id,
    name: besluittype.omschrijving,
    description: undefined
  }));
});

const mappingsModel = computed<Mapping[]>({
  get: () => {
    return mappings.value.map((m) => ({
      sourceId: m.detBesluittypeNaam,
      targetId: m.ozBesluittypeId
    }));
  },
  set: (newMappings: Mapping[]) => {
    const updated = newMappings.map((m) => ({
      detBesluittypeNaam: m.sourceId,
      ozBesluittypeId: m.targetId
    }));
    mappings.value = updated;
    isEditing.value = true;
    emit("update:editing", true);
  }
});

const fetchMappings = async () => {
  if (!props.ozZaaktype?.id || isFetching.value) {
    detBesluittypen.value = [];
    mappings.value = [];
    emit("update:complete", false);
    return;
  }

  isFetching.value = true;
  isLoading.value = true;
  try {
    const detBesluittypeData = await detService.getAllBesluittypen();
    detBesluittypen.value = detBesluittypeData;

    const existingMappings = props.mappingId
      ? await get<BesluittypeMappingsResponse[]>(`/api/mappings/${props.mappingId}/besluittypen`)
      : [];

    mappings.value = detBesluittypeData.map((detBesluittype) => {
      const existingMapping = existingMappings.find(
        (m) => m.detBesluittypeNaam === detBesluittype.naam
      );
      return existingMapping
        ? {
            detBesluittypeNaam: existingMapping.detBesluittypeNaam,
            ozBesluittypeId: existingMapping.ozBesluittypeId
          }
        : {
            detBesluittypeNaam: detBesluittype.naam,
            ozBesluittypeId: null
          };
    });

    const isComplete =
      mappings.value.length > 0 && mappings.value.every((m) => m.ozBesluittypeId !== null);
    emit("update:complete", isComplete);

    if (isComplete && existingMappings.length > 0) {
      isEditing.value = false;
      emit("update:editing", false);
    }
  } catch (error) {
    detBesluittypen.value = [];
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
    const mappingsToSave = mappings.value.filter((m) => m.ozBesluittypeId !== null);

    await post(`/api/mappings/${props.mappingId}/besluittypen`, {
      mappings: mappingsToSave
    } as SaveBesluittypeMappingsRequest);

    toast.add({ text: "De besluittype mappings zijn succesvol opgeslagen." });

    const isComplete =
      mappings.value.length > 0 && mappings.value.every((m) => m.ozBesluittypeId !== null);

    emit("update:complete", isComplete);
    isEditing.value = false;
    emit("update:editing", false);
  } catch (error) {
    toast.add({ text: `Fout bij opslaan van de besluittype mappings - ${error}`, type: "error" });
    throw error;
  } finally {
    isLoading.value = false;
  }
};

watch(
  () => props.ozZaaktype?.id,
  () => {
    fetchMappings();
  },
  { immediate: true }
);

defineExpose({
  fetchMappings,
  setEditing: (value: boolean) => {
    isEditing.value = value;
    emit("update:editing", value);
  }
});
</script>
