<template>
  <mapping-grid
    v-model="mappingsModel"
    title="Status"
    description="Koppel de e-Suite statussen aan de Open Zaak statustypes."
    source-label="e-Suite Status"
    target-label="Open Zaak Statustype"
    :source-items="sourceStatusItems"
    :target-items="targetStatusItems"
    :all-mapped="allMapped"
    :disabled="disabled"
    :loading="isLoading"
    empty-message="Er zijn geen statussen beschikbaar voor dit zaaktype."
    target-placeholder="- Kies een statustype -"
    save-button-text="Mapping opslaan"
    cancel-button-text="Annuleren"
    :show-warning="false"
    :collapsible="true"
    :show-collapse-warning="!allMapped"
    @save="saveMappings"
    @cancel="handleCancel"
    edit-button-text="Mapping aanpassen"
    :show-edit-button="true"
  />
</template>

<script setup lang="ts">
import { ref, computed, watch } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import toast from "@/components/toast/toast";
import type { DETZaaktype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";
import { get, post } from "@/utils/fetchWrapper";

type StatusMappingItem = {
  detStatusNaam: string;
  ozStatustypeId: string | null;
};

type StatusMappingsResponse = {
  detStatusNaam: string;
  ozStatustypeId: string;
};

type SaveStatusMappingsRequest = {
  mappings: StatusMappingItem[];
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

const mappingsFromServer = ref<StatusMappingItem[]>([]);
const validMappings = computed(
  () =>
    props.detZaaktype.statuses?.map(({ naam }) => ({
      sourceId: naam,
      targetId:
        mappingsFromServer.value.find((s) => s.detStatusNaam === naam)?.ozStatustypeId || null
    })) || []
);

const isLoading = ref(false);

const allMapped = computed(() => {
  return validMappings.value.length > 0 && validMappings.value.every((m) => m.targetId);
});

const isComplete = computed(() => allMapped.value);

const sourceStatusItems = computed<MappingItem[]>(() => {
  if (!props.detZaaktype.statuses) return [];
  return props.detZaaktype.statuses.map((status) => ({
    id: status.naam,
    name: status.naam,
    description: status.omschrijving
  }));
});

const targetStatusItems = computed<MappingItem[]>(() => {
  if (!props.ozZaaktype?.statustypes) return [];
  return props.ozZaaktype.statustypes.map((statustype) => ({
    id: statustype.uuid,
    name: statustype.omschrijving,
    description: undefined
  }));
});

const mappingsModel = ref<Mapping[]>([]);

const fetchMappings = async () => {
  isLoading.value = true;
  try {
    mappingsFromServer.value = await get<StatusMappingsResponse[]>(
      `/api/mappings/${props.mappingId}/statuses`
    );
  } catch (error) {
    toast.add({ text: `Fout bij ophalen van de status mappings - ${error}`, type: "error" });
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
        detStatusNaam: sourceId,
        ozStatustypeId: targetId
      }));

    await post(`/api/mappings/${props.mappingId}/statuses`, {
      mappings: mappingsToSave
    } as SaveStatusMappingsRequest);

    toast.add({ text: "De status mappings zijn succesvol opgeslagen." });

    // re-fetch mappings to check for completeness
    await fetchMappings();
  } catch (error) {
    toast.add({ text: `Fout bij opslaan van de status mappings - ${error}`, type: "error" });
    throw error;
  } finally {
    isLoading.value = false;
  }
};

const handleCancel = () => {
  fetchMappings();
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
watch(
  validMappings,
  (value) => {
    mappingsModel.value = value;
  },
  { immediate: true }
);

watch(isComplete, (v) => emit("update:complete", v));
</script>
