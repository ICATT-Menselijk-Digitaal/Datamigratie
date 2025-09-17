import { readonly, ref } from "vue";
import { datamigratieService, type MigrationStatus } from "@/services/datamigratieService";

const migrationStatus = ref<MigrationStatus>();

const loading = ref(false);
const error = ref("");

export const useMigrationStatus = () => {
  const fetchMigrationStatus = async () => {
    loading.value = true;

    try {
      migrationStatus.value = await datamigratieService.getMigrationStatus();
    } catch (err: unknown) {
      error.value = `Fout bij ophalen van de migratie status - ${err}`;
    } finally {
      loading.value = false;
    }
  };

  return {
    migrationStatus: readonly(migrationStatus),
    loading: readonly(loading),
    error: readonly(error),
    fetchMigrationStatus
  };
};
