<template>
  <button type="button" @click="startMigration">Start migratie</button>

  <prompt-modal
    :dialog="confirmDialog"
    cancel-text="Nee, niet migreren"
    confirm-text="Ja, start migratie"
  >
    <h2>Migratie starten</h2>
    <p>
      Weet je zeker dat je de migratie van zaken van het e-Suite zaaktype
      <em>{{ zaaktypeNaam }}</em> wilt starten?
    </p>
  </prompt-modal>
</template>

<script setup lang="ts">
import { useConfirmDialog } from "@vueuse/core";
import PromptModal from "@/components/PromptModal.vue";
import { useMigration } from "@/composables/migration-store";
import { post } from "@/utils/fetchWrapper";
import toast from "@/components/toast/toast";
import type { StartMigration } from "@/types/datamigratie";

const { detZaaktypeId, zaaktypeNaam } = defineProps<{
  detZaaktypeId: string;
  zaaktypeNaam: string;
}>();

const { fetchMigration } = useMigration();
const confirmDialog = useConfirmDialog();

const startMigration = async () => {
  if ((await confirmDialog.reveal()).isCanceled) return;

  try {
    await post(`/api/migration/start`, { detZaaktypeId } as StartMigration);
    fetchMigration();
  } catch (err: unknown) {
    toast.add({ text: `Fout bij starten van de migratie - ${err}`, type: "error" });
  }
};
</script>

<style lang="scss" scoped></style>
