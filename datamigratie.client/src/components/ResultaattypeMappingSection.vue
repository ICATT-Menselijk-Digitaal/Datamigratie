<template>
  <mapping-grid
    v-model="mappingsModel"
    title="Resultaattype"
    description="Koppel de e-Suite resultaattypen aan de Open Zaak resultaattypen."
    source-label="e-Suite Resultaattype"
    target-label="Open Zaak Resultaattype"
    :source-items="sourceResultaattypeItems"
    :target-items="targetResultaattypeItems"
    :all-mapped="allMapped"
    :is-editing="isInEditMode"
    :disabled="disabled"
    :loading="isLoading"
    empty-message="Er zijn geen resultaattypen beschikbaar voor dit zaaktype."
    target-placeholder="- Kies een resultaattype -"
    save-button-text="Mapping opslaan"
    cancel-button-text="Annuleren"
    edit-button-text="Mapping aanpassen"
    :show-edit-button="true"
    :show-warning="false"
    :collapsible="true"
    :show-collapse-warning="!allMapped"
    @save="saveMappings"
    @cancel="handleCancel"
    @edit="forceEdit = true"
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
  mappingId: string;
  detZaaktype: DETZaaktype;
  ozZaaktype: OZZaaktype;
  disabled: boolean;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  (e: "update:complete", value: boolean): void;
}>();

const mappingsFromServer = ref<ResultaattypeMappingItem[]>([]);

const validMappings = computed(
  () =>
    props.detZaaktype.resultaten?.map(({ resultaat: { naam } }) => ({
      sourceId: naam,
      targetId:
        mappingsFromServer.value.find((s) => s.detResultaattypeNaam === naam)?.ozResultaattypeId ||
        null
    })) || []
);

const isLoading = ref(false);
const forceEdit = ref(false);
const isInEditMode = computed(() => forceEdit.value || !allMapped.value);

const allMapped = computed(() => {
  return validMappings.value.length > 0 && validMappings.value.every((m) => m.targetId);
});

const isComplete = computed(() => !isInEditMode.value);

const sourceResultaattypeItems = computed<MappingItem[]>(() => {
  if (!props.detZaaktype.resultaten) return [];
  return props.detZaaktype.resultaten.map((resultaattype) => ({
    id: resultaattype.resultaat.naam,
    name: resultaattype.resultaat.naam,
    description: resultaattype.resultaat.omschrijving
  }));
});

const targetResultaattypeItems = computed<MappingItem[]>(() => {
  if (!props.ozZaaktype.resultaattypen) return [];
  return props.ozZaaktype.resultaattypen.map((resultaattype) => ({
    id: resultaattype.id,
    name: resultaattype.omschrijving,
    description: undefined
  }));
});

const mappingsModel = ref<Mapping[]>([]);

const fetchMappings = async () => {
  isLoading.value = true;
  try {
    mappingsFromServer.value = await get<ResultaattypeMappingResponse[]>(
      `/api/mappings/${props.mappingId}/resultaattypen`
    );
  } catch (error) {
    toast.add({ text: `Fout bij ophalen van de resultaattype mappings - ${error}`, type: "error" });
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
        detResultaattypeNaam: sourceId,
        ozResultaattypeId: targetId
      }));

    await post(`/api/mappings/${props.mappingId}/resultaattypen`, {
      mappings: mappingsToSave
    } as SaveResultaattypeMappingsRequest);

    toast.add({ text: "De resultaattype mappings zijn succesvol opgeslagen." });

    // re-fetch mappings to check for completeness
    await fetchMappings();

    forceEdit.value = false;
  } catch (error) {
    toast.add({ text: `Fout bij opslaan van de resultaattype mappings - ${error}`, type: "error" });
    throw error;
  } finally {
    isLoading.value = false;
  }
};

const handleCancel = () => {
  fetchMappings();
  forceEdit.value = false;
};

// trigger fetching mappings whenever the mapping id or target zaaktype changes
watch(
  [() => props.mappingId, () => props.ozZaaktype],
  () => {
    fetchMappings();
  },
  { immediate: true }
);

// update the mapping model based on server data whenever it changes
watch(validMappings, (value) => {
  mappingsModel.value = value;
});

watch(isComplete, (v) => emit("update:complete", v));
</script>
