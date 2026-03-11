<template>
  <mapping-grid
    v-model="mappingsModel"
    title="Documenttype"
    description="Koppel de e-Suite documenttypes aan de Open Zaak informatieobjecttypes."
    source-label="e-Suite Documenttype"
    target-label="Open Zaak Informatieobjecttype"
    :source-items="sourceItems"
    :target-items="targetItems"
    :all-mapped="allMapped"
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
    :show-collapse-warning="!allMapped"
    @save="saveMappings"
    @cancel="handleCancel"
  />
</template>

<script setup lang="ts">
import { ref, computed, watch } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import toast from "@/components/toast/toast";
import type { DETZaaktype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";
import { get, post } from "@/utils/fetchWrapper";

type DocumenttypeMappingItem = {
  detDocumenttypeNaam: string;
  ozInformatieobjecttypeUrl: string | null;
};

type DocumenttypeMappingResponse = {
  detDocumenttypeNaam: string;
  ozInformatieobjecttypeUrl: string;
};

type SaveDocumenttypeMappingsRequest = {
  mappings: DocumenttypeMappingItem[];
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

const mappingsFromServer = ref<DocumenttypeMappingResponse[]>([]);

const validMappings = computed(
  () =>
    props.detZaaktype.documenttypen?.map(({ naam }) => ({
      sourceId: naam,
      targetId:
        mappingsFromServer.value.find((m) => m.detDocumenttypeNaam === naam)
          ?.ozInformatieobjecttypeUrl || null
    })) || []
);

const isLoading = ref(false);

const allMapped = computed(
  () =>
    validMappings.value.length > 0 && validMappings.value.every(({ targetId }) => targetId)
);

const sourceItems = computed<MappingItem[]>(() => {
  if (!props.detZaaktype.documenttypen) return [];
  return props.detZaaktype.documenttypen
    .filter((dt) => dt.actief)
    .map((dt) => ({
      id: dt.naam,
      name: dt.naam,
      description: dt.omschrijving
    }));
});

const targetItems = computed<MappingItem[]>(() => {
  if (!props.ozZaaktype?.informatieobjecttypen) return [];
  return props.ozZaaktype.informatieobjecttypen.map((iot) => ({
    id: iot.url,
    name: iot.omschrijving,
    description: undefined
  }));
});

const mappingsModel = ref<Mapping[]>([]);

const fetchMappings = async () => {
  isLoading.value = true;
  try {
    mappingsFromServer.value = await get<DocumenttypeMappingResponse[]>(
      `/api/mappings/${props.mappingId}/documenttypen`
    );
  } catch (error) {
    toast.add({ text: `Fout bij ophalen van de documenttype mappings - ${error}`, type: "error" });
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
        detDocumenttypeNaam: sourceId,
        ozInformatieobjecttypeUrl: targetId
      }));

    await post(`/api/mappings/${props.mappingId}/documenttypen`, {
      mappings: mappingsToSave
    } as SaveDocumenttypeMappingsRequest);

    toast.add({ text: "De documenttype mappings zijn succesvol opgeslagen." });

    await fetchMappings();
  } catch (error) {
    toast.add({ text: `Fout bij opslaan van de documenttype mappings - ${error}`, type: "error" });
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
watch(validMappings, (value) => {
  mappingsModel.value = value;
});

watch(allMapped, (v) => emit("update:complete", v));
</script>
