<template>
  <mapping-grid
    title="Publicatieniveau"
    description="Koppel de e-Suite publicatieniveaus voor documenten aan de Open Zaak vertrouwelijkheidaanduiding."
    source-label="e-Suite Publicatieniveau"
    target-label="Open Zaak Vertrouwelijkheidaanduiding"
    :source-items="publicatieNiveauSourceItems"
    :target-items="vertrouwelijkheidaanduidingTargetItems"
    :mapping-id="mappingId"
    :det-zaaktype-id="detZaaktype.functioneleIdentificatie"
    mapping-property="publicatieniveau"
    empty-message="Er zijn geen publicatieniveaus beschikbaar."
    target-placeholder="- Kies een vertrouwelijkheidaanduiding -"
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

const publicatieNiveauSourceItems = computed<MappingItem[]>(
  () => props.detZaaktype.publicatieNiveauOptions ?? []
);

const vertrouwelijkheidaanduidingTargetItems = computed<MappingItem[]>(
  () => props.ozZaaktype.ozDocumentVertrouwelijkheidaanduidingen ?? []
);
</script>

<style lang="scss" scoped>
.document-property-mapping-section {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-small);
  align-self: stretch;
  width: 100%;

  // Remove bottom margin from nested collapsible sections since parent has gap
  :deep(.collapsible-mapping-section) {
    margin-block-end: 0;
  }

  // Add margin-bottom only to the last collapsible section
  // to maintain proper spacing with the next section (Vertrouwelijkheid)
  :deep(.collapsible-mapping-section:last-child) {
    margin-block-end: var(--spacing-small);
  }
}

.test-helper {
  padding: var(--spacing-default);
  background-color: var(--marked);
  border: 2px dashed var(--accent);
  border-radius: var(--radius-default);
  margin-bottom: var(--spacing-default);

  label {
    display: flex;
    align-items: center;
    gap: var(--spacing-small);
    cursor: pointer;
    margin: 0;

    input[type="checkbox"] {
      cursor: pointer;
    }

    span {
      color: var(--code);
      font-weight: var(--font-bold);
    }
  }
}
</style>
