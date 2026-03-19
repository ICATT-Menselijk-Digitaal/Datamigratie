<template>
  <button type="button" class="secondary" @click="startPartialMigration">
    Gedeeltelijke hermigratie
  </button>

  <prompt-modal
    :dialog="confirmDialog"
    cancel-text="Nee, annuleren"
    confirm-text="Ja, start gedeeltelijke hermigratie"
  >
    <h2>Gedeeltelijke hermigratie starten</h2>
    <p>
      Weet je zeker dat je een gedeeltelijke hermigratie wilt starten voor het e-Suite zaaktype
      <em>{{ zaaktypeNaam }}</em
      >? Alleen zaken met fouten uit eerdere migraties en nieuw gesloten zaken worden gemigreerd.
    </p>
  </prompt-modal>
</template>

<script setup lang="ts">
import { useConfirmDialog } from "@vueuse/core";
import PromptModal from "@/components/PromptModal.vue";
import { useMigration } from "@/composables/migration-store";
import { post } from "@/utils/fetchWrapper";
import toast from "@/components/toast/toast";

const { detZaaktypeId, zaaktypeNaam } = defineProps<{
  detZaaktypeId: string;
  zaaktypeNaam: string;
}>();

const { fetchMigration } = useMigration();
const confirmDialog = useConfirmDialog();

const startPartialMigration = async () => {
  if ((await confirmDialog.reveal()).isCanceled) return;

  try {
    await post(`/api/migration/startpartial`, { detZaaktypeId });
    fetchMigration();
  } catch (err: unknown) {
    toast.add({
      text: `Fout bij starten van de gedeeltelijke hermigratie - ${err}`,
      type: "error"
    });
  }
};
</script>

<style lang="scss" scoped></style>
