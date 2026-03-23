<template>
  <mapping-grid
    v-model="mappingsModel"
    title="Publicatieniveau"
    description="Koppel de e-Suite publicatieniveaus voor documenten aan de Open Zaak vertrouwelijkheidaanduiding."
    source-label="e-Suite Publicatieniveau"
    target-label="Open Zaak Vertrouwelijkheidaanduiding"
    :source-items="sourceItems"
    :target-items="targetItems"
    :all-mapped="allMapped"
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

type PublicatieNiveauMappingItem = {
  detPublicatieNiveau: string;
  ozVertrouwelijkheidaanduiding: string | null;
};

type PublicatieNiveauMappingResponse = {
  detPublicatieNiveau: string;
  ozVertrouwelijkheidaanduiding: string;
};

type SavePublicatieNiveauMappingsRequest = {
  mappings: PublicatieNiveauMappingItem[];
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

const mappingsFromServer = ref<PublicatieNiveauMappingResponse[]>([]);

const validMappings = computed(() =>
  (props.detZaaktype.publicatieNiveauOptions ?? []).map((option) => ({
    sourceId: option.id,
    targetId:
      mappingsFromServer.value.find((m) => m.detPublicatieNiveau === option.id)
        ?.ozVertrouwelijkheidaanduiding || null
  }))
);

const isLoading = ref(false);

const allMapped = computed(
  () =>
    validMappings.value.length > 0 && validMappings.value.every(({ targetId }) => targetId)
);

const sourceItems = computed<MappingItem[]>(
  () => props.detZaaktype.publicatieNiveauOptions ?? []
);

const targetItems = computed<MappingItem[]>(
  () => props.ozZaaktype.ozDocumentVertrouwelijkheidaanduidingen ?? []
);

const mappingsModel = ref<Mapping[]>([]);

const fetchMappings = async () => {
  isLoading.value = true;
  try {
    mappingsFromServer.value = await get<PublicatieNiveauMappingResponse[]>(
      `/api/mappings/${props.mappingId}/publicatieniveaus`
    );
  } catch (error) {
    toast.add({ text: `Fout bij ophalen van de publicatieniveau mappings - ${error}`, type: "error" });
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
        detPublicatieNiveau: sourceId,
        ozVertrouwelijkheidaanduiding: targetId
      }));

    await post(`/api/mappings/${props.mappingId}/publicatieniveaus`, {
      mappings: mappingsToSave
    } as SavePublicatieNiveauMappingsRequest);

    toast.add({ text: "De publicatieniveau mappings zijn succesvol opgeslagen." });

    await fetchMappings();
  } catch (error) {
    toast.add({
      text: `Fout bij opslaan van de publicatieniveau mappings - ${error}`,
      type: "error"
    });
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
