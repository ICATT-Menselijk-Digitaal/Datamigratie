<template>
  <mapping-grid
    v-if="showMapping"
    v-model="mappingsModel"
    title="Resultaattype mapping"
    description="Koppel de e-Suite resultaattypen aan de Open Zaak resultaattypen."
    source-label="e-Suite Resultaattype"
    target-label="Open Zaak Resultaattype"
    :source-items="sourceResultaattypeItems"
    :target-items="targetResultaattypeItems"
    :all-mapped="allMapped"
    :is-editing="isEditing"
    :disabled="disabled"
    :loading="loading"
    empty-message="Er zijn geen resultaattypen beschikbaar voor dit zaaktype."
    target-placeholder="Kies een resultaattype"
    save-button-text="Resultaattypen mappings opslaan"
    :show-warning="showWarning"
    warning-message="Niet alle resultaattypen zijn gekoppeld. Migratie kan niet worden gestart."
    @save="handleSave"
  />
</template>

<script setup lang="ts">
import { computed, watch } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import type { DETZaaktype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";
import type { ResultaattypeMappingItem } from "@/services/datamigratieService";

interface Props {
  detZaaktype?: DETZaaktype;
  ozZaaktype?: OZZaaktype;
  resultaattypeMappings: ResultaattypeMappingItem[];
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
  (e: "update:resultaattypeMappings", value: ResultaattypeMappingItem[]): void;
  (e: "save"): void;
}>();

// Computed properties for MappingGrid component
const sourceResultaattypeItems = computed<MappingItem[]>(() => {
  if (!props.detZaaktype?.resultaattypen) return [];
  return props.detZaaktype.resultaattypen
    .filter(resultaattype => resultaattype.actief)
    .map(resultaattype => ({
      id: resultaattype.naam,
      name: resultaattype.naam,
      description: resultaattype.omschrijving
    }));
});

const targetResultaattypeItems = computed<MappingItem[]>(() => {
  if (!props.ozZaaktype?.resultaattypen) return [];
  return props.ozZaaktype.resultaattypen.map(resultaattype => ({
    id: resultaattype.uuid,
    name: resultaattype.omschrijving,
    description: undefined
  }));
});

const mappingsModel = computed<Mapping[]>({
  get: () => {
    return props.resultaattypeMappings.map(m => ({
      sourceId: m.detResultaattypeNaam,
      targetId: m.ozResultaattypeId
    }));
  },
  set: (newMappings: Mapping[]) => {
    const updated = newMappings.map(m => ({
      detStatusNaam: m.sourceId,
      ozStatustypeId: m.targetId
    }));
    emit("update:resultaattypeMappings", updated);
  }
});

const handleSave = () => {
  emit("save");
};
</script>

<style lang="scss" scoped>
// Component-specific styles if needed
</style>
