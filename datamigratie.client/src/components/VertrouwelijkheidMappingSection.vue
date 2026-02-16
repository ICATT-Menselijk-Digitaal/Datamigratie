<template>
  <mapping-grid
    v-model="mappingsModel"
    title="Vertrouwelijkheid"
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
    target-placeholder="- Kies een vertrouwelijkheidaanduiding -"
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
import { ref, computed, watch, watchEffect } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import toast from "@/components/toast/toast";
import { get, post } from "@/utils/fetchWrapper";
import type { OZZaaktype } from "@/services/ozService";
import type { DETZaaktype } from "@/services/detService";

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

const sourceItems = computed<MappingItem[]>(
  () => props.detZaaktype.detVertrouwelijkheidOpties ?? []
);

const targetItems = computed<MappingItem[]>(
  () => props.ozZaaktype.ozZaakVertrouwelijkheidaanduidingen ?? []
);

const isLoading = ref(false);
const forceEdit = ref(false);
const allMapped = ref<boolean>(false);
const mappingsModel = ref<Mapping[]>([]);

const isInEditMode = computed(() => {
  return !allMapped.value || forceEdit.value;
});

const fetchMappings = async () => {
  isLoading.value = true;
  try {
    mappingsModel.value = (
      await get<VertrouwelijkheidMappingResponse[]>(
        `/api/mappings/${props.mappingId}/vertrouwelijkheid`
      )
    ).map((m) => ({
      sourceId: m.detVertrouwelijkheid.toString(),
      targetId: m.ozVertrouwelijkheidaanduiding
    }));
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

const handleCancel = () => {
  fetchMappings();
  forceEdit.value = false;
};

// Trigger fetching mappings and set the form in edit/view mode whenever the mapping id changes
watch(
  () => props.mappingId,
  async () => {
    await fetchMappings();
    forceEdit.value = !allMapped.value;
  },
  { immediate: true }
);

watchEffect(() => {
  // the mapping is complete when all source items are present in the mapping and have a target value
  const isMappingComplete = sourceItems.value.every((m) => {
    const mappedSourceItem = mappingsModel.value.find(
      (mapping) => mapping.sourceId.toString() === m.id
    );

    const mappedSourceItemHasTarget = mappedSourceItem && mappedSourceItem.targetId;
    return mappedSourceItemHasTarget;
  });

  allMapped.value = isMappingComplete;
  emit("update:complete", isMappingComplete && !forceEdit.value);
});
</script>
