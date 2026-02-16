<template>
  <simple-spinner v-if="loading" />

  <template v-else>
    <h2>Algemeen</h2>

    <div class="global-configuration">
      <details class="rsin-collapsible-section" open>
        <summary class="section-header">
          <h2>RSIN</h2>
          <img
            v-if="!rsin"
            src="@/assets/bi-exclamation-circle-fill.svg"
            alt="Niet compleet"
            class="warning-icon"
          />
          <img src="@/assets/arrow-drop-down.svg" alt="Toggle" class="toggle-icon" />
        </summary>

        <div class="section-content">
          <p>Voer hieronder de RSIN in</p>

          <div class="rsin-section">
            <div class="rsin-row">
              <div class="rsin-label">RSIN:</div>
              <div class="rsin-value">
                <input
                  v-if="isEditingRsin"
                  type="text"
                  id="rsin"
                  ref="rsinInput"
                  v-model="rsin"
                  maxlength="9"
                  pattern="[0-9]{9}"
                  @input="validateRsin"
                />
                <div v-else class="rsin-display">
                  <template v-if="rsin">{{ rsin }}</template>
                  <template v-else>
                    <span>Geen</span>
                    <img
                      src="@/assets/bi-exclamation-circle-fill.svg"
                      alt="Geen RSIN"
                      class="warning-icon-inline"
                    />
                  </template>
                </div>
              </div>
            </div>

            <div class="form-actions">
              <template v-if="isEditingRsin">
                <button type="button" class="primary-button" @click="saveRsinConfiguration">
                  Opslaan
                </button>
                <button type="button" class="cancel-button" @click="cancelRsinEdit">
                  Annuleren
                </button>
              </template>
              <template v-else>
                <button type="button" class="edit-button" @click="isEditingRsin = true">
                  RSIN aanpassen
                </button>
              </template>
            </div>
          </div>
        </div>
      </details>

      <documentstatus-mapping-section
        :det-documentstatussen="detDocumentstatussen"
        :documentstatus-mappings="documentstatusMappings"
        :all-mapped="allDocumentstatusesMapped"
        :is-editing="!allDocumentstatusesMapped"
        :loading="documentstatusLoading"
        :show-warning="!allDocumentstatusesMapped"
        @update:documentstatus-mappings="documentstatusMappings = $event"
        @save="saveDocumentstatusMappings"
        @fetch-mappings="fetchDocumentstatusMappings"
      />
    </div>
  </template>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, useTemplateRef } from "vue";
import { get, put } from "@/utils/fetchWrapper";
import type {
  RsinConfiguration,
  UpdateRsinConfiguration,
  DocumentstatusMappingItem,
  DocumentstatusMappingResponse,
  SaveDocumentstatusMappingsRequest
} from "@/types/datamigratie";
import { detService, type DetDocumentstatus } from "@/services/detService";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import DocumentstatusMappingSection from "@/components/DocumentstatusMappingSection.vue";
import toast from "@/components/toast/toast";

const loading = ref(true);
const rsin = ref("");
const originalRsin = ref("");
const isEditingRsin = ref(false);
const rsinConfiguration = ref<RsinConfiguration>({});
const rsinInput = useTemplateRef<HTMLInputElement>("rsinInput");

const detDocumentstatussen = ref<DetDocumentstatus[]>([]);
const documentstatusMappings = ref<DocumentstatusMappingItem[]>([]);
const documentstatusLoading = ref(false);

const allDocumentstatusesMapped = computed(() => {
  if (detDocumentstatussen.value.length === 0) return true;
  return detDocumentstatussen.value.every((status) => {
    const mapping = documentstatusMappings.value.find((m) => m.detDocumentstatus === status.naam);
    return mapping && mapping.ozDocumentstatus;
  });
});

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

async function loadConfiguration() {
  loading.value = true;

  try {
    const [rsinConfig, detStatuses, savedMappings] = await Promise.all([
      get<RsinConfiguration>(`/api/globalmapping/rsin`),
      detService.getAllDocumentstatussen(),
      get<DocumentstatusMappingResponse[]>(`/api/globalmapping/documentstatuses`)
    ]);

    rsinConfiguration.value = rsinConfig;
    rsin.value = rsinConfig.rsin || "";
    originalRsin.value = rsinConfig.rsin || "";
    isEditingRsin.value = !rsinConfig.rsin; // Start in edit mode if no RSIN is set
    if (rsin.value) {
      validateRsin();
    }

    detDocumentstatussen.value = detStatuses;

    // Initialize mappings from saved data
    documentstatusMappings.value = detStatuses.map((status: DetDocumentstatus) => {
      const existingMapping = savedMappings.find((m) => m.detDocumentstatus === status.naam);
      return {
        detDocumentstatus: status.naam,
        ozDocumentstatus: existingMapping?.ozDocumentstatus || null
      };
    });
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

  loading.value = true;

  try {
    const updated = await put<RsinConfiguration>(`/api/globalmapping/rsin`, {
      rsin: rsin.value || undefined
    } as UpdateRsinConfiguration);
    rsinConfiguration.value = updated;
    originalRsin.value = updated.rsin || "";
    isEditingRsin.value = false;
    toast.add({ text: "RSIN configuratie succesvol opgeslagen." });
  } catch (error: unknown) {
    toast.add({ text: `Fout bij opslaan van de RSIN - ${error}`, type: "error" });
  } finally {
    loading.value = false;
  }
}

async function saveDocumentstatusMappings() {
  documentstatusLoading.value = true;

  try {
    const mappingsToSave = documentstatusMappings.value
      .filter((m) => m.ozDocumentstatus)
      .map((m) => ({
        detDocumentstatus: m.detDocumentstatus,
        ozDocumentstatus: m.ozDocumentstatus as string
      }));

    await put<DocumentstatusMappingResponse[]>(`/api/globalmapping/documentstatuses`, {
      mappings: mappingsToSave
    } as SaveDocumentstatusMappingsRequest);
    toast.add({ text: "Documentstatus mappings succesvol opgeslagen." });
  } catch (error: unknown) {
    toast.add({
      text: `Fout bij opslaan van de documentstatus mappings - ${error}`,
      type: "error"
    });
  } finally {
    documentstatusLoading.value = false;
  }
}

async function fetchDocumentstatusMappings() {
  documentstatusLoading.value = true;

  try {
    const [detStatuses, savedMappings] = await Promise.all([
      get<DetDocumentstatus[]>(`/api/det/documentstatussen`),
      get<DocumentstatusMappingResponse[]>(`/api/globalmapping/documentstatuses`)
    ]);

    detDocumentstatussen.value = detStatuses;

    // initialize mappings from saved data
    documentstatusMappings.value = detStatuses.map((status: DetDocumentstatus) => {
      const existingMapping = savedMappings.find((m) => m.detDocumentstatus === status.naam);
      return {
        detDocumentstatus: status.naam,
        ozDocumentstatus: existingMapping?.ozDocumentstatus || null
      };
    });
  } catch (error: unknown) {
    toast.add({
      text: `Fout bij laden van documentstatus mappings - ${error}`,
      type: "error"
    });
  } finally {
    documentstatusLoading.value = false;
  }
}

onMounted(() => {
  loadConfiguration();
});
</script>

<style scoped lang="scss">
.global-configuration {
  display: flex;
  max-width: 75rem;
  flex-direction: column;
  justify-content: center;
  align-items: flex-start;
  gap: var(--spacing-small);
  align-self: stretch;
  width: 100%;
}

.rsin-collapsible-section {
  display: flex;
  padding: var(--spacing-default);
  flex-direction: column;
  align-items: flex-start;
  align-self: stretch;
  border-radius: var(--standard-border-radius);
  border: 1px solid var(--border);
  background: var(--bg);
  margin-block-end: var(--spacing-small);

  .section-header {
    display: flex;
    align-items: center;
    width: 100%;
    padding: var(--spacing-extrasmall) var(--spacing-small);
    cursor: pointer;
    text-align: left;
    gap: var(--spacing-small);
    margin-bottom: 0.125rem;
    list-style: none;

    &::-webkit-details-marker {
      display: none;
    }

    &::marker {
      display: none;
    }

    &:hover {
      opacity: 0.8;
    }

    h2 {
      margin: 0;
      color: var(--text);
      font-family: var(--sans-font);
      font-size: var(--font-medium);
      font-weight: 800;
    }

    .warning-icon {
      width: 1em;
      height: 1em;
    }

    .toggle-icon {
      width: 1.5em;
      height: 1.5em;
      margin-left: auto;
      transition: transform 0.3s ease;
    }
  }

  &[open] .section-header .toggle-icon {
    transform: rotate(180deg);
  }

  .section-content {
    width: 100%;

    p {
      align-self: stretch;
      margin: 0 0 var(--spacing-default) 0;
      color: var(--text);
      font-family: var(--sans-font);
      font-size: var(--font-medium);
      font-weight: 400;
    }
  }
}

.rsin-section {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-default);
}

.rsin-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--spacing-default);
  align-items: center;
  min-height: 3.25rem;
  padding: 0 var(--spacing-default);
  background: var(--accent-bg);
}

.rsin-label {
  color: var(--text);
  font-family: var(--sans-font);
  font-size: var(--font-medium);
  font-weight: 800;
  white-space: nowrap;
}

.rsin-value {
  input {
    width: 100%;
    margin-block-end: 0;
    padding: var(--spacing-small);
    border: 1px solid var(--border);
    border-radius: var(--radius-default);
    font-family: var(--sans-font);
    font-size: var(--font-medium);
    font-weight: 400;
  }
}

.rsin-display {
  color: var(--text);
  font-family: var(--sans-font);
  font-size: var(--font-medium);
  font-weight: 400;
  display: flex;
  align-items: center;
  gap: var(--spacing-small);

  .warning-icon-inline {
    width: 1em;
    height: 1em;
    flex-shrink: 0;
    aspect-ratio: 1/1;
  }
}

.form-actions {
  display: flex;
  gap: var(--spacing-small);
  margin-top: 0;

  button {
    margin-block-end: 0;
  }

  // Button styles are defined in main.scss
}
</style>
