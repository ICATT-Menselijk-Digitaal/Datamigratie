<template>
  <button type="button" @click="openDialog">Migreren</button>

  <dialog ref="dialogRef">
    <form class="form-submit" @submit.prevent="onSubmit">
      <h2>Migreren</h2>
      Kies hieronder welke zaken van het e-Suite zaaktype "{{ zaaktypeNaam }}" je wilt migreren.

      <label class="radio-option">
        <input type="radio" name="optie" value="partial" v-model="selectedOption" />
        Alle nog niet succesvol gemigreerde zaken
      </label>

      <label class="radio-option">
        <input type="radio" name="optie" value="full" v-model="selectedOption" />
        Alle zaken
      </label>

      <label class="radio-option">
        <input type="radio" name="optie" value="single" v-model="selectedOption" />
        Eén zaak
      </label>

      <div v-if="selectedOption === 'single'" class="zaaknummer-input">
        <label for="zaaknummer-input"><b>Voer het zaaknummer in</b></label>
        <input
          id="zaaknummer-input"
          v-model="zaaknummer"
          type="text"
          autocomplete="off"
          @input="errorMessage = ''"
        />
      </div>

      <p v-if="errorMessage" class="error-message" role="alert">⚠ {{ errorMessage }}</p>

      <menu class="reset">
        <li>
          <button type="submit" :disabled="!selectedOption || isLoading">Migratie starten</button>
        </li>
        <li>
          <button type="button" class="secondary" @click="closeDialog">Annuleren</button>
        </li>
      </menu>
    </form>
  </dialog>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useMigration } from "@/composables/migration-store";
import { post } from "@/utils/fetchWrapper";
import toast from "@/components/toast/toast";

const { detZaaktypeId } = defineProps<{
  detZaaktypeId: string;
  zaaktypeNaam: string;
}>();

const { fetchMigration } = useMigration();

const dialogRef = ref<HTMLDialogElement>();
const selectedOption = ref<"partial" | "full" | "single" | "">("");
const zaaknummer = ref("");
const isLoading = ref(false);
const errorMessage = ref("");

const openDialog = () => {
  selectedOption.value = "";
  zaaknummer.value = "";
  errorMessage.value = "";
  dialogRef.value?.showModal();
};

const closeDialog = () => {
  dialogRef.value?.close();
};

const onSubmit = async () => {
  isLoading.value = true;
  errorMessage.value = "";

  try {
    if (selectedOption.value === "partial") {
      await post(`/api/migration/startpartial`, { detZaaktypeId });
      closeDialog();
      fetchMigration();
    } else if (selectedOption.value === "full") {
      await post(`/api/migration/start`, { detZaaktypeId });
      closeDialog();
      fetchMigration();
    } else if (selectedOption.value === "single") {
      const trimmed = zaaknummer.value.trim();
      await post(`/api/migration/startsingle`, { detZaaktypeId, zaaknummer: trimmed });
      closeDialog();
      fetchMigration();
      toast.add({ text: `Migratie gestart voor zaak '${trimmed}'.`, type: "confirm" });
    }
  } catch (err: unknown) {
    if (selectedOption.value === "single") {
      errorMessage.value =
        err instanceof Error ? err.message : "Er is een onbekende fout opgetreden.";
    } else {
      toast.add({ text: `Fout bij starten van de migratie - ${err}`, type: "error" });
    }
  } finally {
    isLoading.value = false;
  }
};
</script>

<style lang="scss" scoped>
form {
  text-align: start;
  display: flex;
  flex-direction: column;
  gap: var(--spacing-default);

  .radio-option {
    display: flex;
    align-items: center;
    gap: var(--spacing-small);
    cursor: pointer;

    input[type="radio"] {
      appearance: auto;
      flex-shrink: 0;
      background-color: unset;
      width: min-content;
      position: static;
    }
  }
}

.error-message {
  color: var(--danger);
}

.zaaknummer-input {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-small);
  background-color: var(--accent-bg);
  padding: var(--spacing-default);
  border-radius: var(--radius-default);
}
</style>
