import type { Migration } from "@/types/datamigratie";
import { MigrationStatus } from "@/types/datamigratie";
import { get } from "@/utils/fetchWrapper";
import { readonly, ref } from "vue";

const POLL_INTERVAL_MS = 5_000;

const migration = ref<Migration>();
const isLoading = ref(false);
const error = ref("");
const previousMigratedZaaktype = ref<string | undefined>();

let pollTimer: ReturnType<typeof setInterval> | undefined;

const stopPolling = () => {
  if (pollTimer !== undefined) {
    clearInterval(pollTimer);
    pollTimer = undefined;
  }
};

const fetchMigration = async (showLoading = true) => {
  if (showLoading) isLoading.value = true;

  try {
    // get the current migration status before fetching the new one, to determine if we need to start or stop polling
    const currentMigration = migration.value;

    // fetch the new migration status
    migration.value = await get<Migration>(`/api/migration`);

    if (
      currentMigration?.status === MigrationStatus.inProgress &&
      migration.value.status !== MigrationStatus.inProgress
    ) {
      previousMigratedZaaktype.value = currentMigration.detZaaktypeId;
      stopPolling();
    } else if (migration.value.status === MigrationStatus.inProgress && pollTimer === undefined) {
      pollTimer = setInterval(() => fetchMigration(false), POLL_INTERVAL_MS);
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
