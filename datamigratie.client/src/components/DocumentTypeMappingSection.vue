<template>
  <mapping-grid
    mapping-property="documenttype"
    :mapping-id="mappingId"
    :det-zaaktype-id="detZaaktype.functioneleIdentificatie"
    title="Documenttype"
    description="Koppel de e-Suite documenttypes aan de Open Zaak informatieobjecttypes."
    source-label="e-Suite Documenttype"
    target-label="Open Zaak Informatieobjecttype"
    :source-items="documenttypeSourceItems"
    :target-items="informatieobjecttypeTargetItems"
    empty-message="Er zijn geen documenttypes beschikbaar."
    target-placeholder="- Kies een informatieobjecttype -"
    @update:complete="emit('update:complete', $event)"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import MappingGrid, { type MappingItem } from "@/components/MappingGrid.vue";
import type { DETZaaktype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";

const props = defineProps<{
  mappingId: string;
  detZaaktype: DETZaaktype;
  ozZaaktype: OZZaaktype;
}>();

const emit = defineEmits<{
  (e: "update:complete", value: boolean): void;
}>();

const documenttypeSourceItems = computed<MappingItem[]>(() => {
  if (!props.detZaaktype.documenttypen) return [];
  const fromServer = props.detZaaktype.documenttypen
    .filter((dt) => dt.actief)
    .map((dt) => ({
      id: dt.naam,
      name: dt.naam,
      description: dt.omschrijving
    }));
  fromServer.push({
    id: "export-pdf",
    name: "Informatieobjecttype voor gegenereerde PDF",
    description: `Voor elke zaak wordt er een PDF document gegenereerd waar niet-mapbare zaakgegevens in staan
      die wel gemigreerd moeten worden. Selecteer welk OZ informatieobjecttype deze PDF met
      zaakgegevens krijgt:`
  });
  return fromServer;
});

const informatieobjecttypeTargetItems = computed<MappingItem[]>(() => {
  if (!props.ozZaaktype?.informatieobjecttypen) return [];
  return props.ozZaaktype.informatieobjecttypen.map((iot) => ({
    id: iot.url,
    name: iot.omschrijving,
    description: undefined
  }));
});
</script>
