<template>
  <button type="button" @click="openAndSelect">
    Voor testen: vul alle mappings automatisch met willekeurige keuzes
  </button>
</template>

<script setup lang="ts">
const wait = (ms?: number) => new Promise((r) => setTimeout(r, ms ?? 50));

async function openAndSelect() {
  for (const details of document.querySelectorAll("details")) {
    details.open = true;
    await wait();
    const editButton = details.querySelector("button[data-edit-grid-open]") as
      | HTMLButtonElement
      | undefined;
    editButton?.click();
    await wait();
    await selectRandomOptions(details);
    const submitbutton = details.querySelector("button[type=submit]") as
      | HTMLButtonElement
      | undefined;
    if (submitbutton) {
      submitbutton.click();
      await waitForSave(submitbutton);
    }
  }
}

async function waitForSave(submitButton: HTMLButtonElement) {
  await new Promise<void>((resolve) => {
    const observer = new MutationObserver(() => {
      if (!document.contains(submitButton) || submitButton.closest("form") === null) {
        observer.disconnect();
        resolve();
      }
    });
    observer.observe(document.body, { childList: true, subtree: true });
    setTimeout(() => {
      observer.disconnect();
      resolve();
    }, 5000);
  });
}

async function selectRandomOptions(details: HTMLDetailsElement) {
  for (const select of details.querySelectorAll("select")) {
    if (select.value) continue;
    const selectableOptions = [...select.options].filter(({ value }) => !!value);
    if (!selectableOptions.length) continue;
    const randomChoice = Math.floor(Math.random() * selectableOptions.length);
    const option = selectableOptions[randomChoice];
    select.value = option.value;
    select.dispatchEvent(new Event("change"));
    await wait();
  }
}
</script>

<style lang="scss" scoped>
button {
  margin-block-end: var(--spacing-default);
}
</style>
