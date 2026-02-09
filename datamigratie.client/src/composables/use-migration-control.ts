import { computed, readonly, ref, toValue, type MaybeRefOrGetter } from "vue";
import { useConfirmDialog } from "@vueuse/core";
import toast from "@/components/toast/toast";
import { post } from "@/utils/fetchWrapper";
import { MigrationStatus, type StartMigration } from "@/types/datamigratie";
import { useMigration } from "./migration-store";

export function useMigrationControl(detZaaktypeIdGetter: MaybeRefOrGetter<string>) {
  const { migration, fetchMigration } = useMigration();
  const isLoading = ref(false); // Can be extended if needed
  const confirmDialog = useConfirmDialog();

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

  return {
    // State
    isLoading: readonly(isLoading),
    isThisMigrationRunning: readonly(isThisMigrationRunning),
    confirmDialog,
    migration: readonly(migration),

    // Methods
    startMigration
  };
}
