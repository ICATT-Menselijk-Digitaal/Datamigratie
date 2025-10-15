import { readonly, ref } from "vue";
import { datamigratieService, type Migration } from "@/services/datamigratieService";

const migration = ref<Migration>();

const loading = ref(false);
const error = ref("");

export const useMigration = () => {
  const fetchMigration = async () => {
    loading.value = true;

    try {
      migration.value = await datamigratieService.getMigration();
    } catch (err: unknown) {
      error.value = `Fout bij ophalen van de migratie status - ${err}`;
    } finally {
      loading.value = false;
    }
  };

  return {
    migration: readonly(migration),
    loading: readonly(loading),
    error: readonly(error),
    fetchMigration
  };
};
