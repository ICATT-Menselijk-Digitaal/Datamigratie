<template>
  <simple-spinner v-if="loading" />

  <div v-else>
    <form @submit.prevent="saveRsinConfiguration">
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

      <div class="form-actions">
        <button type="submit">Opslaan</button>
      </div>
    </form>

    <documentstatus-mapping-section
      :det-documentstatussen="detDocumentstatussen"
      :documentstatus-mappings="documentstatusMappings"
      :all-mapped="allDocumentstatusesMapped"
      :is-editing="isEditingDocumentstatuses"
      :loading="documentstatusLoading"
      :show-warning="true"
      @update:documentstatus-mappings="documentstatusMappings = $event"
      @save="saveDocumentstatusMappings"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, useTemplateRef } from "vue";
import { datamigratieService, type RsinConfiguration, type DocumentstatusMappingItem } from "@/services/datamigratieService";
import { detService, type DetDocumentstatus } from "@/services/detService";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import DocumentstatusMappingSection from "@/components/DocumentstatusMappingSection.vue";
import toast from "@/components/toast/toast";

const loading = ref(true);
const rsin = ref("");
const rsinConfiguration = ref<RsinConfiguration>({});
const rsinInput = useTemplateRef<HTMLInputElement>("rsinInput");

const detDocumentstatussen = ref<DetDocumentstatus[]>([]);
const documentstatusMappings = ref<DocumentstatusMappingItem[]>([]);
const documentstatusLoading = ref(false);

const allDocumentstatusesMapped = computed(() => {
  const activeStatuses = detDocumentstatussen.value.filter(s => s.actief);
  if (activeStatuses.length === 0) return true;

  return activeStatuses.every(status => {
    const mapping = documentstatusMappings.value.find(m => m.detDocumentstatus === status.naam);
    return mapping && mapping.ozDocumentstatus;
  });
});

const isEditingDocumentstatuses = computed(() => {
  return !allDocumentstatusesMapped.value;
});

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
    const [rsinConfig, detStatuses, savedMappings] = await Promise.all([
      datamigratieService.getRsinConfiguration(),
      detService.getAllDocumentstatussen(),
      datamigratieService.getDocumentstatusMappings()
    ]);

    rsinConfiguration.value = rsinConfig;
    rsin.value = rsinConfig.rsin || "";
    if (rsin.value) {
      validateRsin();
    }

    detDocumentstatussen.value = detStatuses;

    // Initialize mappings from saved data
    documentstatusMappings.value = detStatuses
      .filter(s => s.actief)
      .map(status => {
        const existingMapping = savedMappings.find(m => m.detDocumentstatus === status.naam);
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
  loading.value = true;

  try {
    const updated = await datamigratieService.updateRsinConfiguration({
      rsin: rsin.value || undefined
    });
    rsinConfiguration.value = updated;
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
      .filter(m => m.ozDocumentstatus)
      .map(m => ({
        detDocumentstatus: m.detDocumentstatus,
        ozDocumentstatus: m.ozDocumentstatus as string
      }));

    await datamigratieService.saveDocumentstatusMappings({ mappings: mappingsToSave });
    toast.add({ text: "Documentstatus mappings succesvol opgeslagen." });
  } catch (error: unknown) {
    toast.add({ text: `Fout bij opslaan van de documentstatus mappings - ${error}`, type: "error" });
  } finally {
    documentstatusLoading.value = false;
  }
}

onMounted(() => {
  loadConfiguration();
});
</script>

<style scoped>
.form-actions {
  display: flex;
  justify-content: flex-end;
  margin-block-start: var(--spacing-default);
}
</style>
