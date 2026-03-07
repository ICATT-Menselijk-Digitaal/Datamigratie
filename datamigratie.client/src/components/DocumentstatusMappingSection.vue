<template>
  <mapping-grid
    mapping-property="documentstatus"
    title="Documentstatussen"
    description="Koppel de e-Suite documentstatussen aan de Open Zaak documentstatussen."
    source-label="e-Suite Documentstatus"
    target-label="Open Zaak Documentstatus"
    :source-items="sourceItems"
    :target-items="targetItems"
    empty-message="Er zijn geen documentstatussen beschikbaar."
    target-placeholder="- Kies een documentstatus -"
  />
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import MappingGrid, { type MappingItem } from "@/components/MappingGrid.vue";
import { detService, type DetDocumentstatus } from "@/services/detService";

const detDocumentstatussen = ref<DetDocumentstatus[]>([]);
onMounted(async () => {
  detDocumentstatussen.value = await detService.getAllDocumentstatussen();
});

const ozDocumentstatussen = [
  { id: "in_bewerking", name: "In bewerking" },
  { id: "ter_vaststelling", name: "Ter vaststelling" },
  { id: "definitief", name: "Definitief" },
  { id: "gearchiveerd", name: "Gearchiveerd" }
];

const sourceItems = computed<MappingItem[]>(() => {
  return detDocumentstatussen.value.map((status) => ({
    id: status.naam,
    name: status.naam
  }));
});

const targetItems = computed<MappingItem[]>(() => {
  return ozDocumentstatussen.map((status) => ({
    id: status.id,
    name: status.name,
    description: undefined
  }));
});
</script>
