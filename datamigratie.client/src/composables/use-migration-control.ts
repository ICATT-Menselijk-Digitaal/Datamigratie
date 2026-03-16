import { computed, readonly, ref, toValue, type MaybeRefOrGetter } from "vue";
import { useConfirmDialog } from "@vueuse/core";
import toast from "@/components/toast/toast";
import { post } from "@/utils/fetchWrapper";
import { MigrationStatus, type StartMigration, type StartPartialMigration } from "@/types/datamigratie";
import { useMigration } from "./migration-store";

export function useMigrationControl(detZaaktypeIdGetter: MaybeRefOrGetter<string>) {
  const { migration, fetchMigration } = useMigration();
  const isLoading = ref(false); // Can be extended if needed
  const confirmDialog = useConfirmDialog();
  const partialConfirmDialog = useConfirmDialog();

  /**
   * Checks if this specific migration is currently running
   */
  const isThisMigrationRunning = computed(
    () =>
      migration.value?.status === MigrationStatus.inProgress &&
      migration.value.detZaaktypeId === toValue(detZaaktypeIdGetter)
  );

  /**
   * Starts the migration process with user confirmation
   * @param fetchMigration - Callback to refresh migration status
   */
  const startMigration = async () => {
    if ((await confirmDialog.reveal()).isCanceled) return;

    try {
      await post(`/api/migration/start`, {
        detZaaktypeId: toValue(detZaaktypeIdGetter)
      } as StartMigration);
      fetchMigration();
    } catch (err: unknown) {
      toast.add({ text: `Fout bij starten van de migratie - ${err}`, type: "error" });
    }
  };

  const startPartialMigration = async () => {
    if ((await partialConfirmDialog.reveal()).isCanceled) return;

    try {
      await post(`/api/migration/start-partial`, {
        detZaaktypeId: toValue(detZaaktypeIdGetter)
      } as StartPartialMigration);
      fetchMigration();
    } catch (err: unknown) {
      toast.add({ text: `Fout bij starten van de gedeeltelijke hermigratie - ${err}`, type: "error" });
    }
  };

  return {
    // State
    isLoading: readonly(isLoading),
    isThisMigrationRunning: readonly(isThisMigrationRunning),
    confirmDialog,
    partialConfirmDialog,
    migration: readonly(migration),

    // Methods
    startMigration,
    startPartialMigration
  };
}
