import type { Migration } from "@/types/datamigratie";
import { MigrationStatus } from "@/types/datamigratie";
import { get } from "@/utils/fetchWrapper";
import { readonly, ref } from "vue";

const POLL_INTERVAL_MS = 5_000;

const migration = ref<Migration>();
const isLoading = ref(false);
const error = ref("");
const previousMigratedZaaktype = ref<string | undefined>();

let pollTimer: ReturnType<typeof setTimeout> | undefined;

const stopPolling = () => {
  if (pollTimer !== undefined) {
    clearTimeout(pollTimer);
    pollTimer = undefined;
  }
};

const fetchMigration = async (showLoading = true) => {
  if (showLoading) isLoading.value = true;

  try {
    const currentMigration = migration.value;

    migration.value = await get<Migration>(`/api/migration`);
    const newMigrationStatus = migration.value.status;

    if (
      currentMigration?.status === MigrationStatus.inProgress &&
      newMigrationStatus !== MigrationStatus.inProgress
    ) {
      previousMigratedZaaktype.value = currentMigration.detZaaktypeId;
      stopPolling();
    } else if (newMigrationStatus === MigrationStatus.inProgress) {
      previousMigratedZaaktype.value = undefined;
      pollTimer = setTimeout(() => fetchMigration(false), POLL_INTERVAL_MS);
    }
  } catch (err: unknown) {
    error.value = `Fout bij ophalen van de migratie status - ${err}`;
  } finally {
    isLoading.value = false;
  }
};

const dismissFinished = () => {
  previousMigratedZaaktype.value = undefined;
};

export const useMigration = () => ({
  migration: readonly(migration),
  loading: readonly(isLoading),
  error: readonly(error),
  previousMigratedZaaktype: readonly(previousMigratedZaaktype),
  fetchMigration,
  stopPolling,
  dismissFinished
});
