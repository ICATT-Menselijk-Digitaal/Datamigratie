<template>
  <mapping-grid
    title="Resultaattype"
    description="Koppel de e-Suite resultaattypen aan de Open Zaak resultaattypen."
    source-label="e-Suite Resultaattype"
    target-label="Open Zaak Resultaattype"
    :source-items="sourceResultaattypeItems"
    :target-items="targetResultaattypeItems"
    :det-zaaktype-id="detZaaktype.functioneleIdentificatie"
    :mapping-id="mappingId"
    mapping-property="resultaattype"
    empty-message="Er zijn geen resultaattypen beschikbaar voor dit zaaktype."
    target-placeholder="- Kies een resultaattype -"
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

const sourceResultaattypeItems = computed<MappingItem[]>(() => {
  if (!props.detZaaktype.resultaten) return [];
  return props.detZaaktype.resultaten.map((resultaattype) => ({
    id: resultaattype.resultaat.naam,
    name: resultaattype.resultaat.naam,
    description: resultaattype.resultaat.omschrijving
  }));
});

const targetResultaattypeItems = computed<MappingItem[]>(() => {
  if (!props.ozZaaktype.resultaattypen) return [];
  return props.ozZaaktype.resultaattypen.map((resultaattype) => ({
    id: resultaattype.id,
    name: resultaattype.omschrijving,
    description: undefined
  }));
});
</script>
