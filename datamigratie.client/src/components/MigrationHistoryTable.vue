<template>
  <section v-if="migrationHistory.length > 0" class="migration-history">
    <h2>Migratie geschiedenis</h2>
    <p>Hieronder ziet u een overzicht van alle voltooide migraties voor dit zaaktype.</p>

    <table>
      <thead>
        <tr>
          <th>Status</th>
          <th>Gestart op</th>
          <th>Voltooid op</th>
          <th>Totaal</th>
          <th>Verwerkt</th>
          <th>Geslaagd</th>
          <th>Mislukt</th>
          <th>Foutmelding</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="item in migrationHistory"
          :key="item.id"
          class="clickable-row"
          @click="navigateToMigrationDetail(item.id)"
        >
          <td>{{ item.status }}</td>
          <td>{{ formatDateTime(item.startedAt ?? null) }}</td>
          <td>{{ formatDateTime(item.completedAt ?? null) }}</td>
          <td>{{ item.totalRecords ?? "-" }}</td>
          <td>{{ item.processedRecords }}</td>
          <td>{{ item.successfulRecords }}</td>
          <td>{{ item.failedRecords }}</td>
          <td>{{ item.errorMessage || "-" }}</td>
        </tr>
      </tbody>
    </table>
  </section>
</template>

<script setup lang="ts">
import { MigrationStatus, type MigrationHistoryItem } from "@/types/datamigratie";
import { formatDateTime } from "@/utils/dateFormatter";
import { computed, ref, watch } from "vue";
import toast from "./toast/toast";
import { useRoute, useRouter } from "vue-router";
import { get } from "@/utils/fetchWrapper";
import { useMigration } from "@/composables/migration-store";

const props = defineProps<{
  detZaaktypeId: string;
}>();

const migrationHistory = ref<MigrationHistoryItem[]>([]);
const isLoading = ref(true);
const route = useRoute();
const router = useRouter();
const search = computed(() => String(route.query.search || "").trim());
const { migration } = useMigration();

/**
 * Fetches all initial data: DET zaaktype, OZ zaaktypes, existing mapping, and migration history
 */
const fetchMigrationHistory = async () => {
  isLoading.value = true;
  // errors.value = [];

  try {
    migrationHistory.value = await get<MigrationHistoryItem[]>(
      `/api/migration/history/${props.detZaaktypeId}`
    );
  } catch (err: unknown) {
    toast.add({ text: `Fout bij ophalen van de migratiehistorie - ${err}`, type: "error" });
  } finally {
    isLoading.value = false;
  }
};

/**
 * Navigates to the migration detail view
 * @param migrationId - The ID of the migration to view
 * @param search - Optional search query parameter
 */
const navigateToMigrationDetail = (migrationId: number) => {
  router.push({
    name: "migrationDetail",
    params: { detZaaktypeId: props.detZaaktypeId, migrationId: migrationId.toString() },
    query: search.value ? { search: search.value } : undefined
  });
};

watch(() => props.detZaaktypeId, fetchMigrationHistory, { immediate: true });
watch(
  migration,
  async (_, old) => {
    if (old?.detZaaktypeId === props.detZaaktypeId) {
      await fetchMigrationHistory();
    }
  },
  { immediate: true }
);
</script>

<style lang="scss" scoped>
.migration-history {
  margin-block-start: var(--spacing-large);
  padding-block-start: var(--spacing-large);
  border-top: 1px solid var(--border);

  h2 {
    font-size: 1.5rem;
    margin-block-end: var(--spacing-small);
  }

  p {
    margin-block-end: var(--spacing-default);
  }

  table {
    width: 100%;
    border-collapse: collapse;
    font-size: 0.875rem;

    th,
    td {
      padding: var(--spacing-small);
      text-align: left;
      border-bottom: 1px solid var(--border);
    }

    th {
      background-color: var(--background-secondary);
      font-weight: 600;
      white-space: nowrap;
    }

    tbody tr:hover {
      background-color: var(--background-secondary);
    }

    .clickable-row {
      cursor: pointer;

      &:hover {
        background-color: var(--background-hover, #f0f0f0);
      }
    }

    td:last-child {
      word-break: break-word;
      max-width: 300px;
    }
  }
}
</style>
