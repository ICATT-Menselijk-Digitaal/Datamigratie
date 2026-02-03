<template>
  <section v-if="history.length > 0" class="migration-history">
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
          v-for="item in history"
          :key="item.id"
          class="clickable-row"
          @click="handleRowClick(item.id)"
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
import type { MigrationHistoryItem } from "@/services/datamigratieService";
import { formatDateTime } from "@/utils/dateFormatter";

defineProps<{
  history: MigrationHistoryItem[];
}>();

const emit = defineEmits<{
  rowClick: [migrationId: number];
}>();

const handleRowClick = (migrationId: number) => {
  emit("rowClick", migrationId);
};
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
