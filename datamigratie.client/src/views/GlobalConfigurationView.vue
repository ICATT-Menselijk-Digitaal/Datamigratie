<template>
  <simple-spinner v-if="loading" />

  <template v-else>
    <h2>Algemeen</h2>

    <details>
      <summary>
        <span>RSIN</span>
        <img
          v-if="!rsin"
          src="@/assets/bi-exclamation-circle-fill.svg"
          alt="Niet compleet"
          class="warning-icon"
        />
      </summary>

      <p>Voer hieronder de RSIN in</p>

      <form v-if="isEditingRsin" @submit.prevent="saveRsinConfiguration">
        <div class="rsin-grid">
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
        </div>
        <div class="form-actions">
          <button type="submit" class="primary-button">Opslaan</button>
          <button type="button" class="secondary" @click="cancelRsinEdit">Annuleren</button>
        </div>
      </form>
      <template v-else>
        <dl class="rsin-grid">
          <dt>RSIN</dt>
          <dd>
            {{ rsin || "Geen"
            }}<img
              src="@/assets/bi-exclamation-circle-fill.svg"
              v-if="!rsin"
              alt="Geen RSIN"
              class="warning-icon-inline"
            />
          </dd>
        </dl>
        <button type="button" class="secondary" @click="isEditingRsin = true">
          RSIN aanpassen
        </button>
      </template>
    </details>

    <documentstatus-mapping-section />
  </template>
</template>

<script setup lang="ts">
import { ref, onMounted, useTemplateRef } from "vue";
import { get, put } from "@/utils/fetchWrapper";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import DocumentstatusMappingSection from "@/components/DocumentstatusMappingSection.vue";
import toast from "@/components/toast/toast";
import type { Mapping } from "@/components/MappingGrid.vue";

const loading = ref(true);
const rsin = ref("");
const originalRsin = ref("");
const isEditingRsin = ref(false);
const rsinInput = useTemplateRef<HTMLInputElement>("rsinInput");

function validateRsin() {
  if (!rsinInput.value) {
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

function cancelRsinEdit() {
  rsin.value = originalRsin.value;
  isEditingRsin.value = false;
}

const serverUrl = `/api/mappings/properties/rsin`;

async function loadConfiguration() {
  loading.value = true;

  try {
    const rsinConfig = await get<Mapping[]>(serverUrl);
    const rsinValue = rsinConfig[0]?.targetId || "";
    rsin.value = rsinValue;
    originalRsin.value = rsinValue;
    isEditingRsin.value = !rsinValue; // Start in edit mode if no RSIN is set
    if (rsin.value) {
      validateRsin();
    }
  } catch (error: unknown) {
    toast.add({ text: `Fout bij laden van de configuratie - ${error}`, type: "error" });
  } finally {
    loading.value = false;
  }
}

async function saveRsinConfiguration() {
  if (rsinInput.value) {
    validateRsin();
    if (!rsinInput.value.validity.valid) {
      return;
    }
  }

  try {
    originalRsin.value = rsin.value || "";
    isEditingRsin.value = false;
    await put(serverUrl, [
      {
        targetId: rsin.value || undefined
      }
    ]);
  } catch (error: unknown) {
    toast.add({ text: `Fout bij opslaan van de RSIN - ${error}`, type: "error" });
    isEditingRsin.value = true;
  }
}

onMounted(() => {
  loadConfiguration();
});
</script>

<style scoped lang="scss">
details {
  padding-block-end: var(--spacing-default);
}

.rsin-grid {
  margin-block: var(--spacing-default);
  display: flex;
  align-items: center;
  background-color: var(--accent-bg);
  gap: 8rem;

  dt,
  label {
    color: inherit;
    font-weight: bold;
    padding-inline-start: var(--spacing-small);
  }

  dd {
    display: flex;
    align-items: center;
    padding: var(--input-padding);
    border: 1px transparent solid;
  }

  * {
    margin: 0;
  }
}

.warning-icon-inline {
  margin-inline-start: 1ch;
}
</style>
