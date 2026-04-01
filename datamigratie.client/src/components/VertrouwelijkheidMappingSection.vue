<template>
  <mapping-grid
    title="Vertrouwelijkheid"
    description="Koppel de e-Suite vertrouwelijkheid waarden aan de Open Zaak vertrouwelijkheidaanduiding."
    :mapping-id="mappingId"
    mapping-property="vertrouwelijkheid"
    :det-zaaktype-id="detZaaktype.functioneleIdentificatie"
    source-label="e-Suite Vertrouwelijk"
    target-label="Open Zaak Vertrouwelijkheidaanduiding"
    :source-items="sourceItems"
    :target-items="targetItems"
    empty-message="Er zijn geen vertrouwelijkheid opties beschikbaar."
    target-placeholder="- Kies een vertrouwelijkheidaanduiding -"
    @update:complete="emit('update:complete', $event)"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import MappingGrid, { type MappingItem } from "@/components/MappingGrid.vue";
import type { OZZaaktype } from "@/services/ozService";
import type { DETZaaktype } from "@/services/detService";

interface Props {
  mappingId: string;
  detZaaktype: DETZaaktype;
  ozZaaktype: OZZaaktype;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  (e: "update:complete", value: boolean): void;
}>();

const sourceItems = computed<MappingItem[]>(
  () => props.detZaaktype.detVertrouwelijkheidOpties ?? []
);

const targetItems = computed<MappingItem[]>(
  () => props.ozZaaktype.ozZaakVertrouwelijkheidaanduidingen ?? []
);
</script>
