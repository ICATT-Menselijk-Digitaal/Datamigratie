<template>
  <mapping-grid
    v-model="mappingsModel"
    title="Roltype"
    description="Koppel de e-Suite rollen aan de OpenZaak roltypen. Kies 'Alleen PDF' als de rol niet naar OpenZaak gemigreerd wordt maar wel in de PDF opgenomen moet worden."
    source-label="e-Suite Rol"
    target-label="OpenZaak Roltype"
    :source-items="sourceItems"
    :target-items="targetItems"
    :all-mapped="allMapped"
    :disabled="disabled"
    :loading="isLoading"
    empty-message="Er zijn geen rollen beschikbaar."
    target-placeholder="- Kies een roltype -"
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

const ALLEEN_PDF_ID = "alleen_pdf";
const ALLEEN_PDF_ITEM: MappingItem = { id: ALLEEN_PDF_ID, name: "Alleen PDF" };

type RoltypeMappingItem = {
  detRol: string;
  alleenPdf: boolean;
  ozRoltypeUrl: string | null;
};

type RoltypeMappingResponse = {
  detRol: string;
  alleenPdf: boolean;
  ozRoltypeUrl: string | null;
};

type SaveRoltypeMappingsRequest = {
  mappings: RoltypeMappingItem[];
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

const mappingsFromServer = ref<RoltypeMappingResponse[]>([]);
const isLoading = ref(false);

const validMappings = computed(() =>
  (props.detZaaktype.detRolOpties ?? []).map((option) => {
    const serverMapping = mappingsFromServer.value.find((m) => m.detRol === option.id);
    let targetId: string | null = null;
    if (serverMapping) {
      targetId = serverMapping.alleenPdf ? ALLEEN_PDF_ID : serverMapping.ozRoltypeUrl;
    }
    return { sourceId: option.id, targetId };
  })
);

const allMapped = computed(
  () => validMappings.value.length > 0 && validMappings.value.every(({ targetId }) => targetId)
);

const sourceItems = computed<MappingItem[]>(() => props.detZaaktype.detRolOpties ?? []);

const targetItems = computed<MappingItem[]>(() => [
  ...(props.ozZaaktype.roltypen ?? []).map((r) => ({ id: r.url, name: r.omschrijving })),
  ALLEEN_PDF_ITEM
]);

const mappingsModel = ref<Mapping[]>([]);

const fetchMappings = async () => {
  isLoading.value = true;
  try {
    mappingsFromServer.value = await get<RoltypeMappingResponse[]>(
      `/api/mappings/${props.mappingId}/roltypen`
    );
  } catch (error) {
    toast.add({
      text: `Fout bij ophalen van de roltype mappings - ${error instanceof Error ? error.message : "onbekende fout"}`,
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
      .map(({ sourceId, targetId }) => {
        const alleenPdf = targetId === ALLEEN_PDF_ID;
        return {
          detRol: sourceId,
          alleenPdf,
          ozRoltypeUrl: alleenPdf ? null : targetId
        } as RoltypeMappingItem;
      });

    await post(`/api/mappings/${props.mappingId}/roltypen`, {
      mappings: mappingsToSave
    } as SaveRoltypeMappingsRequest);

    toast.add({ text: "De roltype mappings zijn succesvol opgeslagen." });
    await fetchMappings();
  } catch (error) {
    toast.add({
      text: `Fout bij opslaan van de roltype mappings - ${error instanceof Error ? error.message : "onbekende fout"}`,
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

watch(
  [() => props.mappingId, () => props.ozZaaktype],
  () => {
    fetchMappings();
  },
  { immediate: true }
);

watch(validMappings, (value) => {
  mappingsModel.value = value;
});

watch(allMapped, (v) => emit("update:complete", v));
</script>
