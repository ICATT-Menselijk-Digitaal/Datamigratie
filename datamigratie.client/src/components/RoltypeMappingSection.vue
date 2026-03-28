<template>
  <mapping-grid
    title="Roltype"
    mapping-property="roltype"
    :mapping-id="mappingId"
    :det-zaaktype-id="detZaaktype.functioneleIdentificatie"
    description="Koppel de e-Suite rollen aan de OpenZaak roltypen. Kies 'Alleen PDF' als de rol niet naar OpenZaak gemigreerd wordt maar wel in de PDF opgenomen moet worden."
    source-label="e-Suite Rol"
    target-label="OpenZaak Roltype"
    :source-items="sourceItems"
    :target-items="targetItems"
    empty-message="Er zijn geen rollen beschikbaar."
    target-placeholder="- Kies een roltype -"
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

const ALLEEN_PDF_ID = "alleen_pdf";
const ALLEEN_PDF_ITEM: MappingItem = { id: ALLEEN_PDF_ID, name: "Alleen PDF" };

interface Props {
  mappingId: string;
  detZaaktype: DETZaaktype;
  ozZaaktype: OZZaaktype;
  disabled: boolean;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  (e: "update:complete", value: boolean): void;
}>();

const sourceItems = computed<MappingItem[]>(() => props.detZaaktype.detRolOpties ?? []);

const targetItems = computed<MappingItem[]>(() => [
  ...(props.ozZaaktype.roltypen ?? []).map((r) => ({ id: r.url, name: r.omschrijving })),
  ALLEEN_PDF_ITEM
]);
</script>
