<template>
  <simple-spinner v-if="loading" />

  <form v-else @submit.prevent="saveConfiguration">
    <label for="rsin">RSIN</label>
    <input
      type="text"
      id="rsin"
      ref="rsinInput"
      v-model="rsin"
      maxlength="9"
      pattern="[0-9]{9}"
      @input="validateRsin"
    />

    <menu class="reset">
      <li>
        <button type="submit">Opslaan</button>
      </li>
    </menu>
  </form>
</template>

<script setup lang="ts">
import { ref, onMounted, useTemplateRef } from "vue";
import { datamigratieService, type GlobalConfiguration } from "@/services/datamigratieService";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import toast from "@/components/toast/toast";

const loading = ref(true);
const rsin = ref("");
const configuration = ref<GlobalConfiguration>({});
const rsinInput = useTemplateRef<HTMLInputElement>("rsinInput");

function validateRsin() {

  if (!rsinInput.value ) {
    return;
  }

  // Clear any previous custom validation message
  rsinInput.value.setCustomValidity("");

  if (!rsin.value) {
    //empty is allowed
    return;
  }
 
  if (rsin.value && rsin.value.length !== 9) {
    rsinInput.value.setCustomValidity("De RSIN moet uit 9 cijfers bestaan.");
    return;
  }

  // Apply the 11-test (elfproef)
  let sum = 0;
  for (let i = 0; i < 9; i++) {
    const digit = parseInt(rsin.value[i]);
    const multiplier = i === 8 ? -1 : 9 - i;
    sum += digit * multiplier;
  }

  const message = sum % 11 !== 0 ? "RSIN is niet geldig volgens de 11-proef." : "";
  rsinInput.value.setCustomValidity(message);

  // Trigger the browser to show the validation message
  rsinInput.value.reportValidity();
}

async function loadConfiguration() {
  loading.value = true;

  try {
    configuration.value = await datamigratieService.getGlobalConfiguration();
    rsin.value = configuration.value.rsin || "";
    if (rsin.value) {
      validateRsin();
    }
  } catch (error: unknown) {
    toast.add({ text: `Fout bij laden van de configuratie - ${error}`, type: "error" });
  } finally {
    loading.value = false;
  }
}

async function saveConfiguration() {
  loading.value = true;

  try {
    const updated = await datamigratieService.updateGlobalConfiguration({
      rsin: rsin.value || undefined
    });
    configuration.value = updated;
    toast.add({ text: "Configuratie succesvol opgeslagen." });
  } catch (error: unknown) {
    toast.add({ text: `Fout bij opslaan van de mapping - ${error}`, type: "error" });
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  loadConfiguration();
});
</script>
