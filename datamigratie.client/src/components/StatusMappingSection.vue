<template>
  <mapping-grid
    :mapping-id="mappingId"
    mapping-property="status"
    :det-zaaktype-id="detZaaktype.functioneleIdentificatie"
    title="Status"
    description="Koppel de e-Suite statussen aan de Open Zaak statustypes."
    source-label="e-Suite Status"
    target-label="Open Zaak Statustype"
    :source-items="sourceStatusItems"
    :target-items="targetStatusItems"
    empty-message="Er zijn geen statussen beschikbaar voor dit zaaktype."
    target-placeholder="- Kies een statustype -"
    @update:complete="emit('update:complete', $event)"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import MappingGrid, { type MappingItem } from "@/components/MappingGrid.vue";
import type { DETZaaktype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";

interface Props {
  mappingId: string;
  detZaaktype: DETZaaktype;
  ozZaaktype: OZZaaktype;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  (e: "update:complete", value: boolean): void;
}>();

const sourceStatusItems = computed<MappingItem[]>(() => {
  if (!props.detZaaktype.statuses) return [];
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
</script>
