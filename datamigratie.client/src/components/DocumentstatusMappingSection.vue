<template>
  <mapping-grid
    v-model="mappingsModel"
    title="Documentstatussen"
    description="Koppel de e-Suite documentstatussen aan de Open Zaak documentstatussen."
    source-label="e-Suite Documentstatus"
    target-label="Open Zaak Documentstatus"
    :source-items="sourceItems"
    :target-items="targetItems"
    :all-mapped="allMapped"
    :is-editing="isInEditMode"
    :disabled="disabled"
    :loading="loading"
    empty-message="Er zijn geen documentstatussen beschikbaar."
    target-placeholder="- Kies een documentstatus -"
    save-button-text="Mapping opslaan"
    cancel-button-text="Annuleren"
    edit-button-text="Mapping aanpassen"
    :show-edit-button="true"
    :show-warning="false"
    :collapsible="true"
    :show-collapse-warning="!allMapped"
    @save="handleSave"
    @cancel="handleCancel"
    @edit="forceEdit = true"
  />
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import type { DetDocumentstatus } from "@/services/detService";
import type { DocumentstatusMappingItem } from "@/types/datamigratie";

interface Props {
  detDocumentstatussen: DetDocumentstatus[];
  documentstatusMappings: DocumentstatusMappingItem[];
  allMapped: boolean;
  disabled?: boolean;
  loading?: boolean;
  showWarning?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false,
  loading: false,
  showWarning: true
});

const emit = defineEmits<{
  (e: "save", mappings: DocumentstatusMappingItem[]): void;
}>();

const forceEdit = ref(false);
const localMappings = ref<DocumentstatusMappingItem[]>([]);

const ozDocumentstatussen = [
  { id: "in_bewerking", name: "In bewerking" },
  { id: "ter_vaststelling", name: "Ter vaststelling" },
  { id: "definitief", name: "Definitief" },
  { id: "gearchiveerd", name: "Gearchiveerd" }
];

const isInEditMode = computed(() => {
  return !props.allMapped || forceEdit.value;
});

const sourceItems = computed<MappingItem[]>(() => {
  return props.detDocumentstatussen.map((status) => ({
    id: status.naam,
    name: status.naam
  }));
});

const targetItems = computed<MappingItem[]>(() => {
  return ozDocumentstatussen.map((status) => ({
    id: status.id,
    name: status.name,
    description: undefined
  }));
});

const mappingsModel = computed<Mapping[]>({
  get: () => {
    return localMappings.value.map((m) => ({
      sourceId: m.detDocumentstatus,
      targetId: m.ozDocumentstatus
    }));
  },
  set: (newMappings: Mapping[]) => {
    localMappings.value = newMappings.map((m) => ({
      detDocumentstatus: m.sourceId,
      ozDocumentstatus: m.targetId
    }));
  }
});

const handleSave = () => {
  emit("save", localMappings.value);
  forceEdit.value = false;
};

const handleCancel = () => {
  // server state
  localMappings.value = JSON.parse(JSON.stringify(props.documentstatusMappings));
  forceEdit.value = false;
};

watch(
  () => props.documentstatusMappings,
  (newMappings) => {
    localMappings.value = JSON.parse(JSON.stringify(newMappings));
  },
  { immediate: true, deep: true }
);
</script>
