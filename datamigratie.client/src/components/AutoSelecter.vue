<template>
  <button type="button" @click="openAndSelect">
    for testing: check to autofill all mappings with random selections
  </button>
</template>

<script setup lang="ts">
import { nextTick } from "vue";

async function openAndSelect() {
  openAllSections();
  await nextTick();
  fillInAllSelectElements();
}

function openAllSections() {
  document
    .querySelectorAll("button[data-edit-grid-open]")
    .forEach((button) => (button as HTMLButtonElement).click());
}

function fillInAllSelectElements() {
  document.querySelectorAll("select").forEach((select) => {
    // ignore if it already has a value
    if (select.value) return;
    // find the first option that has an actual value (not a placeholder)
    const firstOption = [...select.options].find(({ value }) => !!value);
    // if there are no options, ignore
    if (!firstOption) return;
    select.value = firstOption.value;
  });
}
</script>
