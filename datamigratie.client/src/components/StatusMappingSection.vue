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
    :loading="loading"
    empty-message="Er zijn geen statussen beschikbaar voor dit zaaktype."
    target-placeholder="Kies een statustype"
    save-button-text="Statusmappings opslaan"
    :show-warning="showWarning"
    warning-message="Niet alle statussen zijn gekoppeld. Migratie kan niet worden gestart."
    @save="handleSave"
  />
</template>

<script setup lang="ts">
import { computed, watch } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import type { DETZaaktype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";
import type { StatusMappingItem } from "@/services/datamigratieService";

interface Props {
  detZaaktype?: DETZaaktype;
  ozZaaktype?: OZZaaktype;
  statusMappings: StatusMappingItem[];
  allMapped: boolean;
  isEditing: boolean;
  disabled?: boolean;
  loading?: boolean;
  showWarning?: boolean;
  showMapping: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false,
  loading: false,
  showWarning: true
});

const emit = defineEmits<{
  (e: "update:statusMappings", value: StatusMappingItem[]): void;
  (e: "save"): void;
}>();

// Computed properties for MappingGrid component
const sourceStatusItems = computed<MappingItem[]>(() => {
  if (!props.detZaaktype?.statuses) return [];
  return props.detZaaktype.statuses
    .filter((status) => status.actief)
    .map((status) => ({
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
    return props.statusMappings.map((m) => ({
      sourceId: m.detStatusNaam,
      targetId: m.ozStatustypeId
    }));
  },
  set: (newMappings: Mapping[]) => {
    const updated = newMappings.map((m) => ({
      detStatusNaam: m.sourceId,
      ozStatustypeId: m.targetId
    }));
    emit("update:statusMappings", updated);
  }
});

const handleSave = () => {
  emit("save");
};
</script>

<style lang="scss" scoped>
// Component-specific styles if needed
</style>
