import type { Migration } from "@/types/datamigratie";
import { MigrationStatus } from "@/types/datamigratie";
import { get } from "@/utils/fetchWrapper";
import { readonly, ref } from "vue";

const POLL_INTERVAL_MS = 5_000;

const migration = ref<Migration>();
const error = ref("");
const migrationJustCompleted = ref(false);

let pollTimer: ReturnType<typeof setTimeout> | undefined;

const stopPolling = () => {
  if (pollTimer !== undefined) {
    clearTimeout(pollTimer);
    pollTimer = undefined;
  }
};

const scheduleNextPoll = () => {
  stopPolling();
  pollTimer = setTimeout(() => fetchMigration(), POLL_INTERVAL_MS);
};

const fetchMigration = async () => {
  try {
    const previousStatus = migration.value?.status;
    const newStatus = await get<Migration>(`/api/migration`);

    if (
      newStatus.detZaaktypeId === migration.value?.detZaaktypeId &&
      newStatus.status === migration.value?.status
    ) {
      scheduleNextPoll();
      return;
    }

    migration.value = newStatus;

    if (
      previousStatus === MigrationStatus.inProgress &&
      newStatus.status !== MigrationStatus.inProgress
    ) {
      migrationJustCompleted.value = true;
      stopPolling();
    } else if (newStatus.status === MigrationStatus.inProgress) {
      migrationJustCompleted.value = false;
      scheduleNextPoll();
    }
  } catch (err: unknown) {
    error.value = `Fout bij ophalen van de migratie status - ${err}`;
  }
};

const dismissCompletedAlert = () => {
  migrationJustCompleted.value = false;
};

export const useMigration = () => ({
  migration: readonly(migration),
  error: readonly(error),
  migrationJustCompleted: readonly(migrationJustCompleted),
  fetchMigration,
  stopPolling,
  dismissCompletedAlert
});
