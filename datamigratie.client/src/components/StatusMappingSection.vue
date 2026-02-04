<template>
  <mapping-grid
    v-if="showMapping"
    v-model="mappingsModel"
    title="Status mapping"
    description="Koppel de e-Suite statussen aan de Open Zaak statustypes."
    source-label="e-Suite Status"
    target-label="Open Zaak Statustype"
    :source-items="sourceStatusItems"
    :target-items="targetStatusItems"
    :all-mapped="allMapped"
    :is-editing="isEditing"
    :disabled="disabled"
    :loading="showLoading"
    empty-message="Er zijn geen statussen beschikbaar voor dit zaaktype."
    target-placeholder="Kies een statustype"
    save-button-text="Statusmappings opslaan"
    :show-warning="showWarning"
    warning-message="Niet alle statussen zijn gekoppeld. Migratie kan niet worden gestart."
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

const mappings = ref<StatusMappingItem[]>([]);
const isEditing = ref(false);
const isLoading = ref(false);
const isFetching = ref(false);

const isDataLoaded = computed(() => {
  return !!(props.detZaaktype?.statuses && props.ozZaaktype?.statustypes);
});

const showLoading = computed(() => {
  return isLoading.value || !isDataLoaded.value;
});

const allMapped = computed(() => {
  return mappings.value.length > 0 && mappings.value.every((m) => m.ozStatustypeId !== null);
});

const sourceStatusItems = computed<MappingItem[]>(() => {
  if (!props.detZaaktype?.statuses) return [];
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

const mappingsModel = computed<Mapping[]>({
  get: () => {
    return mappings.value.map((m) => ({
      sourceId: m.detStatusNaam,
      targetId: m.ozStatustypeId
    }));
  },
  set: (newMappings: Mapping[]) => {
    const updated = newMappings.map((m) => ({
      detStatusNaam: m.sourceId,
      ozStatustypeId: m.targetId
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
      ? await get<StatusMappingsResponse[]>(`/api/mappings/${props.mappingId}/statuses`)
      : [];
    const detStatuses = props.detZaaktype?.statuses || [];

    mappings.value = detStatuses.map((detStatus) => {
      const existingMapping = existingMappings.find((m) => m.detStatusNaam === detStatus.naam);
      return {
        detStatusNaam: detStatus.naam,
        ozStatustypeId: existingMapping?.ozStatustypeId || null
      };
    });

    const isComplete =
      mappings.value.length > 0 && mappings.value.every((m) => m.ozStatustypeId !== null);
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
    const mappingsToSave = mappings.value.filter((m) => m.ozStatustypeId !== null);

    await post(`/api/mappings/${props.mappingId}/statuses`, {
      mappings: mappingsToSave
    } as SaveStatusMappingsRequest);

    toast.add({ text: "De status mappings zijn succesvol opgeslagen." });

    const isComplete =
      mappings.value.length > 0 && mappings.value.every((m) => m.ozStatustypeId !== null);

    emit("update:complete", isComplete);
    isEditing.value = false;
    emit("update:editing", false);
  } catch (error) {
    toast.add({ text: `Fout bij opslaan van de status mappings - ${error}`, type: "error" });
    throw error;
  } finally {
    isLoading.value = false;
  }
};

watch(
  () => [props.ozZaaktype?.id, props.detZaaktype?.statuses],
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
