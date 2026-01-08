<template>
  <h1>e-Suite zaaktype</h1>

  <simple-spinner v-if="loading" />

  <form v-else @submit.prevent="submitMapping">
    <alert-inline v-if="errors.length"
      >Fout(en) bij ophalen gegevens - {{ errors.join(" | ") }}</alert-inline
    >

    <dl v-else-if="detZaaktype">
      <dt>Naam:</dt>
      <dd>{{ detZaaktype.naam }}</dd>

      <dt>Omschrijving:</dt>
      <dd>{{ detZaaktype.omschrijving }}</dd>

      <dt>Actief:</dt>
      <dd>{{ detZaaktype.actief ? "Ja" : "Nee" }}</dd>

      <dt>Aantal gesloten zaken:</dt>
      <dd>{{ detZaaktype?.closedZakenCount }}</dd>

      <dt id="mapping">Koppeling OZ zaaktype:</dt>
      <dd v-if="canStartMigration || isThisMigrationRunning">
        {{ ozZaaktypes?.find((type) => type.id == mapping.ozZaaktypeId)?.identificatie }}
      </dd>
      <dd v-else>
        <select
          name="ozZaaktypeId"
          aria-labelledby="mapping"
          v-model="mapping.ozZaaktypeId"
          required
        >
          <option v-if="!mapping.ozZaaktypeId" value="">Kies Open Zaak zaaktype</option>

          <option v-for="{ id, identificatie } in ozZaaktypes" :value="id" :key="id">
            {{ identificatie }}
          </option>
        </select>
      </dd>
    </dl>

    <menu class="reset">
      <li>
        <router-link
          :to="{ name: 'detZaaktypes', ...(search && { query: { search } }) }"
          class="button button-secondary"
          >&lt; Terug</router-link
        >
      </li>

      <template v-if="!errors.length && !isThisMigrationRunning">
        <li v-if="!canStartMigration">
          <button type="submit">Mapping opslaan</button>
        </li>

        <li v-else>
          <button type="button" class="secondary" @click="setEditMode(true)">
            Mapping aanpassen
          </button>
        </li>

        <li v-if="canStartMigration">
          <button type="button" @click="startMigration">Start migratie</button>
        </li>
      </template>
    </menu>

    <prompt-modal
      :dialog="confirmDialog"
      cancel-text="Nee, niet migreren"
      confirm-text="Ja, start migratie"
    >
      <h2>Migratie starten</h2>

      <p>
        Weet je zeker dat je de migratie van zaken van het e-Suite zaaktype
        <em>{{ detZaaktype?.naam }}</em> wilt starten?
      </p>
    </prompt-modal>

    <prompt-modal
      :dialog="changeZaaktypeDialog"
      cancel-text="Annuleren"
      confirm-text="Ja, wijzig zaaktype"
    >
      <h2>OpenZaak zaaktype wijzigen</h2>

      <p>
        <strong>Let op:</strong> Als je het OpenZaak zaaktype wijzigt, worden alle bestaande
        resultaattype mappings verwijderd. Je moet deze opnieuw instellen voor het nieuwe zaaktype.
      </p>

      <p>
        Weet je zeker dat je het zaaktype wilt wijzigen?
      </p>
    </prompt-modal>

    <!-- Resultaattype Mapping Section -->
    <section v-if="mapping.detZaaktypeId && detZaaktype?.resultaten && selectedOzZaaktype?.resultaattypen">
      <h2>Resultaattype Mapping</h2>
      <table>
        <thead>
          <tr>
            <th>e-Suite Resultaat</th>
            <th>OpenZaak Resultaattype</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="detResultaattype in sortedDetResultaten" :key="detResultaattype.resultaat.naam">
            <td>{{ detResultaattype.resultaat.naam }}</td>
            <td>
              <select v-model="resultaattypeMappings[detResultaattype.resultaat.naam]" :disabled="isEditMode">
                <option
                  v-for="ozResultaattype in selectedOzZaaktype.resultaattypen"
                  :key="ozResultaattype.id"
                  :value="ozResultaattype.id"
                >
                  {{ ozResultaattype.omschrijving }}
                </option>
              </select>
            </td>
          </tr>
        </tbody>
      </table>
      <button type="button" @click="saveResultaattypeMappings" :disabled="isEditMode">Resultaattype mappings opslaan</button>
    </section>

    <section v-if="!errors.length && migrationHistory.length > 0" class="migration-history">
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
            <td>{{ item.totalRecords }}</td>
            <td>{{ item.processedRecords }}</td>
            <td>{{ item.successfulRecords }}</td>
            <td>{{ item.failedRecords }}</td>
            <td>{{ item.errorMessage || "-" }}</td>
          </tr>
        </tbody>
      </table>
    </section>
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useConfirmDialog } from "@vueuse/core";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import PromptModal from "@/components/PromptModal.vue";
import toast from "@/components/toast/toast";
import { detService, type DETZaaktype } from "@/services/detService";
import { ozService, type OZZaaktype } from "@/services/ozService";
import {
  datamigratieService,
  MigrationStatus,
  type ZaaktypeMapping,
  type UpdateZaaktypeMapping,
  type MigrationHistoryItem,
  type ResultaattypeMappingRequest
} from "@/services/datamigratieService";
import { knownErrorMessages } from "@/utils/fetchWrapper";
import { useMigration } from "@/composables/use-migration-status";

const { detZaaktypeId } = defineProps<{ detZaaktypeId: string }>();

const route = useRoute();
const router = useRouter();
const search = computed(() => String(route.query.search || "").trim());

const detZaaktype = ref<DETZaaktype>();
const ozZaaktypes = ref<OZZaaktype[]>();
const selectedOzZaaktype = ref<OZZaaktype>();
const mapping = ref({ ozZaaktypeId: "" } as ZaaktypeMapping);
const migrationHistory = ref<MigrationHistoryItem[]>([]);
const resultaattypeMappings = ref<Record<string, string>>({});

const { migration, fetchMigration } = useMigration();

const isEditMode = ref(false);
const setEditMode = (value: boolean) => (isEditMode.value = value);

const sortedDetResultaten = computed(() => {
  if (!detZaaktype.value?.resultaten) return [];
  return [...detZaaktype.value.resultaten].sort((a, b) =>
    a.resultaat.naam.localeCompare(b.resultaat.naam)
  );
});

const canStartMigration = computed(
  () =>
    mapping.value.detZaaktypeId &&
    mapping.value.ozZaaktypeId &&
    migration.value?.status !== MigrationStatus.inProgress &&
    !isEditMode.value
);

const isThisMigrationRunning = computed(
  () =>
    migration.value?.status === MigrationStatus.inProgress &&
    migration.value.detZaaktypeId === mapping.value.detZaaktypeId
);

const loading = ref(false);
const errors = ref<unknown[]>([]);

const confirmDialog = useConfirmDialog();
const changeZaaktypeDialog = useConfirmDialog();
const previousOzZaaktypeId = ref<string>("");

const formatDateTime = (dateString: string | null): string => {
  if (!dateString) return "-";
  const date = new Date(dateString);
  return date.toLocaleString("nl-NL", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit"
  });
};

const fetchMappingData = async () => {
  loading.value = true;
  errors.value = [];

  try {
    const services = [
      {
        service: detService.getZaaktypeById(detZaaktypeId),
        target: detZaaktype
      },
      { service: ozService.getAllZaaktypes(), target: ozZaaktypes },
      {
        service: datamigratieService.getMappingByDETZaaktypeId(detZaaktypeId),
        target: mapping,
        ignore404: true
      },
      {
        service: datamigratieService.getMigrationHistory(detZaaktypeId),
        target: migrationHistory,
        ignore404: false
      }
    ];

    const results = await Promise.allSettled(services.map((s) => s.service));

    results.forEach((result, index) => {
      const { target, ignore404 } = services[index];

      if (result.status === "fulfilled") {
        target.value = result.value;
      } else {
        const { reason } = result;

        if (ignore404 && reason instanceof Error && reason.message === knownErrorMessages.notFound)
          return;

        errors.value.push(reason);
      }
    });
  } catch (err: unknown) {
    errors.value.push(err);
  } finally {
    loading.value = false;
  }
};

const submitMapping = async () => {
  // Check if OZ zaaktype is changing and there are filled resultaattype mappings
  if (mapping.value.detZaaktypeId &&
      previousOzZaaktypeId.value &&
      previousOzZaaktypeId.value !== mapping.value.ozZaaktypeId) {
    const hasFilledMappings = Object.values(resultaattypeMappings.value).some(value => value !== "");

    if (hasFilledMappings) {
      // Show confirmation dialog
      const result = await changeZaaktypeDialog.reveal();

      if (result.isCanceled) {
        // Revert the change
        mapping.value.ozZaaktypeId = previousOzZaaktypeId.value;
        return;
      }

      // Clear resultaattype mappings since they will be deleted on backend
      resultaattypeMappings.value = {};
    }
  }

  loading.value = true;

  setEditMode(false);

  try {
    if (!mapping.value.detZaaktypeId) {
      mapping.value = { ...mapping.value, detZaaktypeId };

      await datamigratieService.createMapping(mapping.value);
    } else {
      const updatedMapping: UpdateZaaktypeMapping = {
        detZaaktypeId: mapping.value.detZaaktypeId,
        updatedOzZaaktypeId: mapping.value.ozZaaktypeId
      };

      await datamigratieService.updateMapping(updatedMapping);
    }

    // Update the previous value after successful save
    if (mapping.value.ozZaaktypeId) {
      previousOzZaaktypeId.value = mapping.value.ozZaaktypeId;
    }

    // After saving, fetch the new OZ zaaktype details and reload mappings
    await fetchSelectedOzZaaktype();
    await loadExistingResultaattypeMappings();

    toast.add({ text: "De mapping is succesvol opgeslagen." });
  } catch (err: unknown) {
    toast.add({ text: `Fout bij opslaan van de mapping - ${err}`, type: "error" });
  } finally {
    loading.value = false;
  }
};

const startMigration = async () => {
  if ((await confirmDialog.reveal()).isCanceled) return;

  loading.value = true;

  try {
    await datamigratieService.startMigration({ detZaaktypeId });

    fetchMigration();
  } catch (err: unknown) {
    toast.add({ text: `Fout bij starten van de migratie - ${err}`, type: "error" });
  } finally {
    loading.value = false;
  }
};

const navigateToMigrationDetail = (migrationId: number) => {
  router.push({
    name: "migrationDetail",
    params: { detZaaktypeId, migrationId: migrationId.toString() },
    query: search.value ? { search: search.value } : undefined
  });
};

const fetchSelectedOzZaaktype = async () => {
  if (!mapping.value.ozZaaktypeId) {
    selectedOzZaaktype.value = undefined;
    return;
  }

  try {
    selectedOzZaaktype.value = await ozService.getZaaktypeById(mapping.value.ozZaaktypeId);
  } catch (error: unknown) {
    toast.add({ text: `Fout bij ophalen OZ Zaaktype: ${error}`, type: "error" });
  }
};

const loadExistingResultaattypeMappings = async () => {
  if (!mapping.value.detZaaktypeId) return;

  try {
    const existingMappings = await datamigratieService.getAllResultaattypeMappings(detZaaktypeId);
    if (existingMappings && existingMappings.length > 0) {
      existingMappings.forEach((mapping) => {
        resultaattypeMappings.value[mapping.detResultaattypeId] = mapping.ozResultaattypeId;
      });
    }
  } catch {
    // No existing mappings found
  }
};

const saveResultaattypeMappings = async () => {
  if (!mapping.value.ozZaaktypeId) {
    toast.add({ text: "Eerst een OpenZaak zaaktype selecteren", type: "error" });
    return;
  }

  try {
    // First, fetch existing mappings to know which ones to update vs create
    const existingMappings = await datamigratieService.getAllResultaattypeMappings(detZaaktypeId);
    const existingDetResultaattypeIds = new Set(existingMappings.map(m => m.detResultaattypeId));

    for (const [detResultaatNaam, ozResultaattypeId] of Object.entries(resultaattypeMappings.value)) {
      if (!ozResultaattypeId) continue;

      const payload: ResultaattypeMappingRequest = {
        ozZaaktypeId: mapping.value.ozZaaktypeId,
        ozResultaattypeId: ozResultaattypeId
      };

      if (existingDetResultaattypeIds.has(detResultaatNaam)) {
        await datamigratieService.updateResultaattypeMapping(detZaaktypeId, detResultaatNaam, payload);
      } else {
        await datamigratieService.createResultaattypeMapping(detZaaktypeId, detResultaatNaam, payload);
      }
    }

    toast.add({ text: "Resultaattype mappings succesvol opgeslagen" });
  } catch (error: unknown) {
    toast.add({ text: `Fout bij opslaan: ${error}`, type: "error" });
  }
};

watch(
  () => mapping.value.ozZaaktypeId,
  () => {
    // Only process if not in edit mode
    // In edit mode, changes shouldn't trigger any actions until saved
    if (isEditMode.value) {
      return;
    }

    // Fetch the new OZ zaaktype details
    fetchSelectedOzZaaktype();
  }
);

watch(
  () => mapping.value.detZaaktypeId,
  () => {
    if (mapping.value.detZaaktypeId) {
      fetchSelectedOzZaaktype();
      loadExistingResultaattypeMappings();
      // Store the initial OZ zaaktype ID
      if (mapping.value.ozZaaktypeId) {
        previousOzZaaktypeId.value = mapping.value.ozZaaktypeId;
      }
    }
  }
);

onMounted(() => fetchMappingData());
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

dl {
  display: grid;
  gap: var(--spacing-default);
  margin-block-end: var(--spacing-large);

  dt {
    color: var(--text);
    font-weight: 600;

    &[id] {
      align-self: center;
    }
  }

  dd {
    margin-inline: 0;
  }

  select {
    min-inline-size: var(--section-width-small);
    margin-block-end: 0;
  }

  @media (min-width: variables.$breakpoint-md) {
    & {
      grid-template-columns: max-content 1fr;

      dd {
        grid-column: 2;
      }
    }
  }
}

menu {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-default);

  li > * {
    text-align: center;
    inline-size: 100%;
  }

  @media (min-width: variables.$breakpoint-md) {
    & {
      flex-direction: row;

      li:first-of-type {
        margin-inline-end: auto;
      }
    }
  }
}

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
