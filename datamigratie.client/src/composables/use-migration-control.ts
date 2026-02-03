import { computed, watch, type Ref } from "vue";
import { useRouter } from "vue-router";
import { useConfirmDialog } from "@vueuse/core";
import toast from "@/components/toast/toast";
import { datamigratieService, MigrationStatus, type ZaaktypeMapping, type Migration } from "@/services/datamigratieService";

export function useMigrationControl(
  detZaaktypeId: string,
  mapping: Ref<ZaaktypeMapping>,
  migration: Ref<Migration | undefined>,
  isEditingZaaktypeMapping: Ref<boolean>,
  isEditingStatusMapping: Ref<boolean>,
  statusMappingsComplete: Ref<boolean>,
  isEditingResultaattypeMapping: Ref<boolean>,
  resultaattypeMappingsComplete: Ref<boolean>,
  isEditingBesluittypeMapping: Ref<boolean>,
  besluittypeMappingsComplete: Ref<boolean>,
  isEditingPublicatieNiveauMapping: Ref<boolean>,
  isEditingDocumenttypeMapping: Ref<boolean>,
  documentPropertyMappingsComplete: Ref<boolean>,
  onMigrationCompleted: () => Promise<void>
) {
  const router = useRouter();
  const isLoading = computed(() => false); // Can be extended if needed
  const confirmDialog = useConfirmDialog();

  /**
   * Checks if this specific migration is currently running
   */
  const isThisMigrationRunning = computed(
    () =>
      migration.value?.status === MigrationStatus.inProgress &&
      migration.value.detZaaktypeId === mapping.value.detZaaktypeId
  );

  /**
   * Checks if migration can be started
   * All mappings must be complete and no editing should be in progress
   */
  const canStartMigration = computed(
    () =>
      mapping.value.detZaaktypeId &&
      mapping.value.ozZaaktypeId &&
      migration.value?.status !== MigrationStatus.inProgress &&
      !isEditingZaaktypeMapping.value &&
      !isEditingStatusMapping.value &&
      statusMappingsComplete.value &&
      !isEditingResultaattypeMapping.value &&
      resultaattypeMappingsComplete.value &&
      !isEditingBesluittypeMapping.value &&
      besluittypeMappingsComplete.value &&
      !isEditingPublicatieNiveauMapping.value &&
      !isEditingDocumenttypeMapping.value &&
      documentPropertyMappingsComplete.value
  );

  /**
   * Starts the migration process with user confirmation
   * @param fetchMigration - Callback to refresh migration status
   */
  const startMigration = async (fetchMigration: () => void) => {
    if ((await confirmDialog.reveal()).isCanceled) return;

    try {
      await datamigratieService.startMigration({ detZaaktypeId });
      fetchMigration();
    } catch (err: unknown) {
      toast.add({ text: `Fout bij starten van de migratie - ${err}`, type: "error" });
    }
  };

  /**
   * Navigates to the migration detail view
   * @param migrationId - The ID of the migration to view
   * @param search - Optional search query parameter
   */
  const navigateToMigrationDetail = (migrationId: number, search?: string) => {
    router.push({
      name: "migrationDetail",
      params: { detZaaktypeId, migrationId: migrationId.toString() },
      query: search ? { search } : undefined
    });
  };

  /**
   * Watch for migration status changes and trigger callback when completed
   */
  watch(
    () => migration.value?.status,
    (newStatus, oldStatus) => {
      // When migration changes from inProgress to none, refresh the data (migration completed)
      if (oldStatus === MigrationStatus.inProgress && newStatus === MigrationStatus.none) {
        onMigrationCompleted();
      }
    }
  );

  return {
    // State
    isLoading,
    isThisMigrationRunning,
    canStartMigration,
    confirmDialog,

    // Methods
    startMigration,
    navigateToMigrationDetail
  };
}

