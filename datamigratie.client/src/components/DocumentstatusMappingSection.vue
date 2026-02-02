<template>
  <mapping-grid
    v-model="mappingsModel"
    title="Documentstatus mapping"
    description="Koppel de e-Suite documentstatussen aan de Open Zaak documentstatussen."
    source-label="e-Suite Documentstatus"
    target-label="Open Zaak Documentstatus"
    :source-items="sourceItems"
    :target-items="targetItems"
    :all-mapped="allMapped"
    :is-editing="isEditing"
    :disabled="disabled"
    :loading="loading"
    empty-message="Er zijn geen documentstatussen beschikbaar."
    target-placeholder="Kies een documentstatus"
    save-button-text="Documentstatus mappings opslaan"
    :show-warning="showWarning"
    warning-message="Niet alle documentstatussen zijn gekoppeld."
    @save="handleSave"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import type { DetDocumentstatus } from "@/services/detService";
import type { DocumentstatusMappingItem } from "@/services/datamigratieService";

interface Props {
  detDocumentstatussen: DetDocumentstatus[];
  documentstatusMappings: DocumentstatusMappingItem[];
  allMapped: boolean;
  isEditing: boolean;
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
  (e: "update:documentstatusMappings", value: DocumentstatusMappingItem[]): void;
  (e: "save"): void;
}>();

const ozDocumentstatussen = [
  { id: "in_bewerking", name: "In bewerking" },
  { id: "ter_vaststelling", name: "Ter vaststelling" },
  { id: "definitief", name: "Definitief" },
  { id: "gearchiveerd", name: "Gearchiveerd" }
];

const sourceItems = computed<MappingItem[]>(() => {
  return props.detDocumentstatussen
    .map(status => ({
      id: status.naam,
      name: status.naam,
      description: status.omschrijving
    }));
});

const targetItems = computed<MappingItem[]>(() => {
  return ozDocumentstatussen.map(status => ({
    id: status.id,
    name: status.name,
    description: undefined
  }));
});

const mappingsModel = computed<Mapping[]>({
  get: () => {
    return props.documentstatusMappings.map(m => ({
      sourceId: m.detDocumentstatus,
      targetId: m.ozDocumentstatus
    }));
  },
  set: (newMappings: Mapping[]) => {
    const updated = newMappings.map(m => ({
      detDocumentstatus: m.sourceId,
      ozDocumentstatus: m.targetId
    }));
    emit("update:documentstatusMappings", updated);
  }
});

const handleSave = () => {
  emit("save");
};
</script>
