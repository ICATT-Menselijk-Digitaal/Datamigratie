<template>
  <mapping-grid
    title="Besluittype"
    description="Koppel de e-Suite besluittypen aan de Open Zaak besluittypen."
    :det-zaaktype-id="detZaaktype.functioneleIdentificatie"
    :mapping-id="mappingId"
    mapping-property="besluittype"
    source-label="e-Suite Besluittype"
    target-label="Open Zaak Besluittype"
    :source-items="sourceBesluittypeItems"
    :target-items="targetBesluittypeItems"
    empty-message="Er zijn geen besluittypen beschikbaar voor dit zaaktype."
    target-placeholder="- Kies een besluittype -"
    save-button-text="Mapping opslaan"
    cancel-button-text="Annuleren"
    edit-button-text="Mapping aanpassen"
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

const sourceBesluittypeItems = computed<MappingItem[]>(() => {
  if (!props.detZaaktype?.besluittypen) return [];
  return props.detZaaktype.besluittypen.map((besluittype) => ({
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
</script>
