<template>
  <h1>Globale configuratie</h1>

  <simple-spinner v-if="loading" />

  <div v-else>
    <alert-inline v-if="errorMessage" class="error">{{ errorMessage }}</alert-inline>
    <alert-inline v-if="successMessage" class="success">{{ successMessage }}</alert-inline>

    <form @submit.prevent="saveConfiguration">
      <p>
        Op deze pagina kunt u globale instellingen configureren die van toepassing zijn op alle
        zaaktypes.
      </p>

      <fieldset>
        <legend>RSIN configuratie</legend>

        <div class="form-group">
          <label for="rsin">RSIN van de gemeente</label>
          <input
            type="text"
            id="rsin"
            v-model="rsin"
            maxlength="9"
            pattern="[0-9]{9}"
            placeholder="Voer 9 cijfers in"
            @input="validateRsin"
          />
          <small class="help-text">
            Het RSIN (Rechtspersonen Samenwerkingsverbanden Informatienummer) moet precies 9
            cijfers bevatten en voldoen aan de 11-proef.
          </small>
          <div v-if="validationError" class="validation-error">{{ validationError }}</div>
          <div v-if="rsinValid" class="validation-success">RSIN is geldig</div>
        </div>

        <div v-if="configuration.updatedAt" class="last-updated">
          Laatst bijgewerkt: {{ formatDate(configuration.updatedAt) }}
        </div>
      </fieldset>

      <menu class="reset">
        <li>
          <router-link to="/" class="button button-secondary">&lt; Terug</router-link>
        </li>
        <li>
          <button type="submit" :disabled="!canSave">Opslaan</button>
        </li>
      </menu>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { datamigratieService, type GlobalConfiguration } from "@/services/datamigratieService";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import AlertInline from "@/components/AlertInline.vue";

const loading = ref(true);
const errorMessage = ref("");
const successMessage = ref("");
const rsin = ref("");
const validationError = ref("");
const rsinValid = ref(false);
const configuration = ref<GlobalConfiguration>({});

const canSave = computed(() => {
  return rsin.value.length === 9 && rsinValid.value && !validationError.value;
});

function validateRsin() {
  successMessage.value = "";
  validationError.value = "";
  rsinValid.value = false;

  if (!rsin.value) {
    return;
  }

  if (rsin.value.length !== 9) {
    validationError.value = "RSIN moet precies 9 cijfers bevatten.";
    return;
  }

  if (!/^\d{9}$/.test(rsin.value)) {
    validationError.value = "RSIN mag alleen cijfers bevatten.";
    return;
  }

  // Apply the 11-test (elfproef)
  let sum = 0;
  for (let i = 0; i < 9; i++) {
    const digit = parseInt(rsin.value[i]);
    const multiplier = i === 8 ? -1 : 9 - i;
    sum += digit * multiplier;
  }

  if (sum % 11 !== 0) {
    validationError.value = "RSIN is niet geldig volgens de 11-proef.";
    return;
  }

  rsinValid.value = true;
}

async function loadConfiguration() {
  loading.value = true;
  errorMessage.value = "";

  try {
    configuration.value = await datamigratieService.getGlobalConfiguration();
    rsin.value = configuration.value.rsin || "";
    if (rsin.value) {
      validateRsin();
    }
  } catch (error: any) {
    errorMessage.value = "Er is een fout opgetreden bij het laden van de configuratie.";
    console.error(error);
  } finally {
    loading.value = false;
  }
}

async function saveConfiguration() {
  errorMessage.value = "";
  successMessage.value = "";
  loading.value = true;

  try {
    const updated = await datamigratieService.updateGlobalConfiguration({
      rsin: rsin.value || undefined
    });
    configuration.value = updated;
    successMessage.value = "Configuratie succesvol opgeslagen.";
  } catch (error: any) {
    if (error.message) {
      errorMessage.value = error.message;
    } else {
      errorMessage.value = "Er is een fout opgetreden bij het opslaan van de configuratie.";
    }
    console.error(error);
  } finally {
    loading.value = false;
  }
}

function formatDate(dateString: string): string {
  const date = new Date(dateString);
  return date.toLocaleString("nl-NL", {
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit"
  });
}

onMounted(() => {
  loadConfiguration();
});
</script>

<style scoped>
.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: bold;
}

.form-group input[type="text"] {
  width: 100%;
  max-width: 300px;
  padding: 0.5rem;
  font-size: 1rem;
  border: 1px solid #ccc;
  border-radius: 4px;
}

.form-group input[type="text"]:focus {
  outline: none;
  border-color: #0066cc;
  box-shadow: 0 0 0 3px rgba(0, 102, 204, 0.1);
}

.help-text {
  display: block;
  margin-top: 0.5rem;
  color: #666;
  font-size: 0.875rem;
}

.validation-error {
  margin-top: 0.5rem;
  color: #d32f2f;
  font-size: 0.875rem;
}

.validation-success {
  margin-top: 0.5rem;
  color: #2e7d32;
  font-size: 0.875rem;
}

.last-updated {
  margin-top: 1rem;
  font-size: 0.875rem;
  color: #666;
}

fieldset {
  border: 1px solid #ddd;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  border-radius: 4px;
}

legend {
  padding: 0 0.5rem;
  font-weight: bold;
  font-size: 1.1rem;
}

.error {
  background-color: #ffebee;
  border-left: 4px solid #d32f2f;
  padding: 1rem;
  margin-bottom: 1rem;
}

.success {
  background-color: #e8f5e9;
  border-left: 4px solid #2e7d32;
  padding: 1rem;
  margin-bottom: 1rem;
}
</style>
