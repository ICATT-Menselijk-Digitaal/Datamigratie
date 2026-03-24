<template>
  <button type="button" class="secondary outline" @click="openDialog">Migreer één zaak</button>

  <dialog ref="dialogRef">
    <form method="dialog" @submit.prevent="onSubmit">
      <h2>Één zaak migreren</h2>
      <p>
        Voer het zaaknummer in van de zaak die je wilt migreren voor het e-Suite zaaktype
        <em>{{ zaaktypeNaam }}</em
        >.
      </p>

      <label for="zaaknummer-input">Zaaknummer</label>
      <input
        id="zaaknummer-input"
        v-model="zaaknummer"
        type="text"
        placeholder="Vul het zaaknummer in"
        required
        autocomplete="off"
      />

      <p v-if="errorMessage" class="error-message" role="alert">{{ errorMessage }}</p>

      <menu class="reset">
        <li>
          <button type="button" class="button secondary" @click="closeDialog">Annuleren</button>
        </li>
        <li>
          <button type="submit" :disabled="!zaaknummer.trim() || isLoading">Migreer zaak</button>
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

const { detZaaktypeId, zaaktypeNaam } = defineProps<{
  detZaaktypeId: string;
  zaaktypeNaam: string;
}>();

const { fetchMigration } = useMigration();

const dialogRef = ref<HTMLDialogElement>();
const zaaknummer = ref("");
const isLoading = ref(false);
const errorMessage = ref("");

const openDialog = () => {
  zaaknummer.value = "";
  errorMessage.value = "";
  dialogRef.value?.showModal();
};

const closeDialog = () => {
  dialogRef.value?.close();
};

const onSubmit = async () => {
  const trimmed = zaaknummer.value.trim();
  if (!trimmed) return;

  isLoading.value = true;
  errorMessage.value = "";
  try {
    await post(`/api/migration/startsingle`, { detZaaktypeId, zaaknummer: trimmed });
    closeDialog();
    fetchMigration();
    toast.add({ text: `Migratie gestart voor zaak '${trimmed}'.`, type: "confirm" });
  } catch (err: unknown) {
    errorMessage.value = `Fout bij starten van de enkelvoudige migratie - ${err}`;
  } finally {
    isLoading.value = false;
  }
};
</script>

<style lang="scss" scoped>
dialog {
  --backdrop-color: rgb(0 0 0 / 60%);

  min-width: 33%;
  padding: var(--spacing-large);
  border: var(--outline-width) solid var(--border);
  border-radius: var(--radius-default);
  box-shadow: var(--shadow-default);
  background-color: var(--bg);
  color: var(--text);
}

::backdrop {
  background-color: var(--backdrop-color);
}

form {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-default);

  > * {
    margin-block: 0;
  }
}

menu {
  display: flex;
  gap: var(--spacing-default);
  justify-content: flex-end;
}

label {
  font-weight: var(--font-bold);
}

.error-message {
  color: var(--danger);
}
</style>
