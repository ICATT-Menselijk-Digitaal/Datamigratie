import type { Migration } from "@/types/datamigratie";
import { get } from "@/utils/fetchWrapper";
import { readonly, ref } from "vue";

const migration = ref<Migration>();
const isLoading = ref(false);
const error = ref("");

const fetchMigration = async () => {
  isLoading.value = true;

  try {
    migration.value = await get<Migration>(`/api/migration`);
  } catch (err: unknown) {
    error.value = `Fout bij ophalen van de migratie status - ${err}`;
  } finally {
    isLoading.value = false;
  }
};

export const useMigration = () => ({
  migration: readonly(migration),
  loading: readonly(isLoading),
  error: readonly(error),
  fetchMigration
});
