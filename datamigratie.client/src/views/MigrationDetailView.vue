<template>
  <h1>Migratie details</h1>

  <simple-spinner v-if="loading" />

  <alert-inline v-else-if="error">{{ error }}</alert-inline>

  <div v-else>
    <menu class="reset nav-menu">
      <li>
        <router-link
          :to="{
            name: 'detZaaktype',
            params: { detZaaktypeId },
            ...(search && { query: { search } })
          }"
          class="button button-secondary"
          >&lt; Terug naar zaaktype</router-link
        >
      </li>
    </menu>

    <section v-if="failedRecords.length > 0" class="records-section">
      <h2>Mislukte zaken ({{ failedRecords.length }})</h2>
      <p>Hieronder ziet u een overzicht van alle mislukte zaken in deze migratie.</p>

      <table>
        <thead>
          <tr>
            <th>DET Zaaknummer</th>
            <th>Fout titel</th>
            <th>Fout details</th>
            <th>Status code</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="record in failedRecords" :key="record.id">
            <td>{{ record.detZaaknummer }}</td>
            <td>{{ record.errorTitle || "-" }}</td>
            <td class="error-details">{{ record.errorDetails || "-" }}</td>
            <td>{{ record.statusCode || "-" }}</td>
          </tr>
        </tbody>
      </table>
    </section>

    <section v-if="successfulRecords.length > 0" class="records-section">
      <h2>Geslaagde zaken ({{ successfulRecords.length }})</h2>
      <p>Hieronder ziet u een overzicht van alle succesvol gemigreerde zaken.</p>

      <table>
        <thead>
          <tr>
            <th>DET Zaaknummer</th>
            <th>OpenZaak Zaaknummer</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="record in successfulRecords" :key="record.id">
            <td>{{ record.detZaaknummer }}</td>
            <td>{{ record.ozZaaknummer || "-" }}</td>
          </tr>
        </tbody>
      </table>
    </section>

    <p v-if="failedRecords.length === 0 && successfulRecords.length === 0">
      Geen records gevonden voor deze migratie.
    </p>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import { get } from "@/utils/fetchWrapper";
import type { MigrationRecordItem } from "@/types/datamigratie";

const { migrationId, detZaaktypeId } = defineProps<{
  migrationId: string;
  detZaaktypeId: string;
}>();

const route = useRoute();
const search = computed(() => String(route.query.search || "").trim());

const loading = ref(false);
const error = ref("");
const records = ref<MigrationRecordItem[]>([]);

const failedRecords = computed(() => records.value.filter((r) => !r.isSuccessful));
const successfulRecords = computed(() => records.value.filter((r) => r.isSuccessful));

const fetchMigrationRecords = async () => {
  loading.value = true;
  error.value = "";

  try {
    records.value = await get<MigrationRecordItem[]>(
      `/api/migration/${Number(migrationId)}/records`
    );
  } catch (err: unknown) {
    error.value = `Fout bij ophalen migratie records - ${err}`;
  } finally {
    loading.value = false;
  }
};

onMounted(() => fetchMigrationRecords());
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

.nav-menu {
  display: flex;
  margin-block-end: var(--spacing-large);

  li > * {
    text-align: center;
  }
}

.records-section {
  margin-block-end: var(--spacing-extra-large);

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

    .error-details {
      word-break: break-word;
      max-width: 400px;
    }
  }
}
</style>
