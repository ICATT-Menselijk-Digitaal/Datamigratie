<template>
  <mapping-grid
    v-if="showMapping"
    v-model="mappingsModel"
    title="Besluittype mapping"
    description="Koppel de e-Suite besluittypen aan de Open Zaak besluittypen."
    source-label="e-Suite Besluittype"
    target-label="Open Zaak Besluittype"
    :source-items="sourceBesluittypeItems"
    :target-items="targetBesluittypeItems"
    :all-mapped="allMapped"
    :is-editing="isEditing"
    :disabled="disabled"
    :loading="loading"
    empty-message="Er zijn geen besluittypen beschikbaar voor dit zaaktype."
    target-placeholder="Kies een besluittype"
    save-button-text="Besluittypemappings opslaan"
    :show-warning="showWarning"
    warning-message="Niet alle besluittypen zijn gekoppeld. Migratie kan niet worden gestart."
    @save="handleSave"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import type { DetBesluittype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";
import type { BesluittypeMappingItem } from "@/services/datamigratieService";

interface Props {
  detBesluittypen?: DetBesluittype[];
  ozZaaktype?: OZZaaktype;
  besluittypeMappings: BesluittypeMappingItem[];
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
  (e: "update:besluittypeMappings", value: BesluittypeMappingItem[]): void;
  (e: "save"): void;
}>();

// Computed properties for MappingGrid component
const sourceBesluittypeItems = computed<MappingItem[]>(() => {
  if (!props.detBesluittypen) return [];
  return props.detBesluittypen
    .filter((besluittype) => besluittype.actief)
    .map((besluittype) => ({
      id: besluittype.naam,
      name: besluittype.naam,
      description: besluittype.omschrijving
    }));
});

const targetBesluittypeItems = computed<MappingItem[]>(() => {
  if (!props.ozZaaktype?.besluittypen) return [];
  return props.ozZaaktype.besluittypen.map((besluittype) => ({
    id: besluittype.id,
    name: besluittype.omschrijving,
    description: undefined
  }));
});

const mappingsModel = computed<Mapping[]>({
  get: () => {
    return props.besluittypeMappings.map((m) => ({
      sourceId: m.detBesluittypeNaam,
      targetId: m.ozBesluittypeId
    }));
  },
  set: (newMappings: Mapping[]) => {
    const updated = newMappings.map((m) => ({
      detBesluittypeNaam: m.sourceId,
      ozBesluittypeId: m.targetId
    }));
    emit("update:besluittypeMappings", updated);
  }
});

const handleSave = () => {
  emit("save");
};
</script>
