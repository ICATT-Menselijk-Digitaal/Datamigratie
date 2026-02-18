<template>
  <mapping-grid
    v-model="mappingsModel"
    title="Besluittype"
    description="Koppel de e-Suite besluittypen aan de Open Zaak besluittypen."
    source-label="e-Suite Besluittype"
    target-label="Open Zaak Besluittype"
    :source-items="sourceBesluittypeItems"
    :target-items="targetBesluittypeItems"
    :all-mapped="allMapped"
    :is-editing="isInEditMode"
    :disabled="disabled"
    :loading="isLoading"
    empty-message="Er zijn geen besluittypen beschikbaar voor dit zaaktype."
    target-placeholder="- Kies een besluittype -"
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
import type { DetBesluittype, DETZaaktype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";
import { get, post } from "@/utils/fetchWrapper";

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
  mappingId: string;
  detZaaktype: DETZaaktype;
  ozZaaktype: OZZaaktype;
  disabled: boolean;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  (e: "update:complete", value: boolean): void;
}>();

const mappingsFromServer = ref<BesluittypeMappingItem[]>([]);
const detBesluittypen = computed<DetBesluittype[]>(() => props.detZaaktype?.besluittypen ?? []);

const validMappings = computed(() =>
  detBesluittypen.value.map(({ naam }) => ({
    sourceId: naam,
    targetId:
      mappingsFromServer.value.find((s) => s.detBesluittypeNaam === naam)?.ozBesluittypeId || null
  }))
);
const isLoading = ref(false);
const forceEdit = ref(false);
const isInEditMode = computed(() => forceEdit.value || !allMapped.value);

const allMapped = computed(() => {
  if (detBesluittypen.value.length === 0) return true;
  return validMappings.value.length > 0 && validMappings.value.every((m) => m.targetId !== null);
});

const isComplete = computed(() => !isInEditMode.value);

const sourceBesluittypeItems = computed<MappingItem[]>(() => {
  return detBesluittypen.value.map((besluittype) => ({
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

const mappingsModel = ref<Mapping[]>([]);

const fetchMappings = async () => {
  isLoading.value = true;
  try {
    mappingsFromServer.value = await get<BesluittypeMappingsResponse[]>(
      `/api/mappings/${props.mappingId}/besluittypen`
    );
  } catch (error) {
    toast.add({ text: `Fout bij ophalen van de besluittype mappings - ${error}`, type: "error" });
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
        detBesluittypeNaam: sourceId,
        ozBesluittypeId: targetId
      }));

    await post(`/api/mappings/${props.mappingId}/besluittypen`, {
      mappings: mappingsToSave
    } as SaveBesluittypeMappingsRequest);

    toast.add({ text: "De besluittype mappings zijn succesvol opgeslagen." });
    // re-fetch mappings to check for completeness
    await fetchMappings();
    forceEdit.value = false;
  } catch (error) {
    toast.add({ text: `Fout bij opslaan van de besluittype mappings - ${error}`, type: "error" });
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
watch(validMappings, (v) => {
  mappingsModel.value = v;
});

watch(isComplete, (v) => emit("update:complete", v), { immediate: true });
</script>
