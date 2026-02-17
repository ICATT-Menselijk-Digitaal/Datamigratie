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
