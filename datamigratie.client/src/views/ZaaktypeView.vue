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
      <dd v-if="!isEditingZaaktypeMapping && mapping.detZaaktypeId" class="mapping-display">
        {{ ozZaaktypes?.find((type) => type.id == mapping.ozZaaktypeId)?.identificatie }}
        <button 
          type="button" 
          class="secondary mapping-edit-button"
          @click="setEditingZaaktypeMapping(true)"
          v-if="!isThisMigrationRunning"
        >
          Mapping aanpassen
        </button>
      </dd>
      <dd v-else class="mapping-controls">
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
        <button type="submit" class="mapping-save-button">Mapping opslaan</button>
      </dd>
    </dl>

    <!-- Status Mapping Section -->
    <section v-if="showStatusMapping" class="status-mapping">
      <h2>Status mapping</h2>
      <p>Koppel de e-Suite statussen aan de Open Zaak statustypes.</p>

      <simple-spinner v-if="!statusMappingData" />

      <div v-else-if="statusMappingData.detStatuses.length === 0">
        <p>Er zijn geen statussen beschikbaar voor dit zaaktype.</p>
      </div>

      <div v-else class="status-mapping-grid">
        <div class="mapping-header">
          <div>e-Suite Status</div>
          <div>Open Zaak Statustype</div>
        </div>

        <div
          v-for="detStatus in statusMappingData.detStatuses"
          :key="detStatus.naam"
          class="mapping-row"
        >
          <div class="det-status">
            <strong>{{ detStatus.naam }}</strong>
            <span class="status-description">{{ detStatus.omschrijving }}</span>
          </div>

          <div class="oz-status">
            <select
              v-model="statusMappings.find((m) => m.detStatusNaam === detStatus.naam)!.ozStatustypeId"
              :disabled="!isEditingStatusMapping && statusMappingsComplete || isThisMigrationRunning"
              required
            >
              <option :value="null">Kies een statustype</option>
              <option
                v-for="ozStatus in statusMappingData.ozStatustypes"
                :key="ozStatus.uuid"
                :value="ozStatus.uuid"
              >
                {{ ozStatus.omschrijving }}
              </option>
            </select>
          </div>
        </div>

        <div v-if="(!statusMappingsComplete || isEditingStatusMapping) && !isThisMigrationRunning" class="mapping-actions">
          <button type="button" @click="saveStatusMappings">Statusmappings opslaan</button>
        </div>

        <alert-inline v-if="!statusMappingsComplete && mapping.detZaaktypeId" type="warning">
          Niet alle statussen zijn gekoppeld. Migratie kan niet worden gestart.
        </alert-inline>
      </div>
    </section>

    <menu class="reset">
      <li>
        <router-link
          :to="{ name: 'detZaaktypes', ...(search && { query: { search } }) }"
          class="button button-secondary"
          >&lt; Terug</router-link
        >
      </li>

      <template v-if="!errors.length && !isThisMigrationRunning">
        <li v-if="canStartMigration && statusMappingsComplete">
          <button type="button" class="secondary" @click="setEditingStatusMapping(true)">
            Statusmappings aanpassen
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
      :dialog="confirmOzZaaktypeChangeDialog"
      cancel-text="Annuleren"
      confirm-text="Ja, wijzig zaaktype"
    >
      <h2>Open Zaak zaaktype wijzigen</h2>

      <p>
        <strong>Let op:</strong> Als je het Open Zaak zaaktype wijzigt, worden alle bestaande statusmappings verwijderd.
      </p>
      <p>
        Je moet de statusmappings opnieuw configureren voor het nieuwe zaaktype.
      </p>
      <p>
        Weet je zeker dat je wilt doorgaan?
      </p>
    </prompt-modal>

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
            <td>{{ item.totalRecords ?? "-" }}</td>
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
  type StatusMappingsResponse,
  type StatusMappingItem
} from "@/services/datamigratieService";
import { knownErrorMessages } from "@/utils/fetchWrapper";
import { useMigration } from "@/composables/use-migration-status";

const { detZaaktypeId } = defineProps<{ detZaaktypeId: string }>();

const route = useRoute();
const router = useRouter();
const search = computed(() => String(route.query.search || "").trim());

const detZaaktype = ref<DETZaaktype>();
const ozZaaktypes = ref<OZZaaktype[]>();
const mapping = ref({ ozZaaktypeId: "" } as ZaaktypeMapping);
const migrationHistory = ref<MigrationHistoryItem[]>([]);

const statusMappingData = ref<StatusMappingsResponse | null>(null);
const statusMappings = ref<StatusMappingItem[]>([]);
const statusMappingsComplete = ref(false);
const showStatusMapping = computed(() => mapping.value.detZaaktypeId && mapping.value.ozZaaktypeId);

const { migration, fetchMigration } = useMigration();

const isEditingZaaktypeMapping = ref(false);
const setEditingZaaktypeMapping = (value: boolean) => (isEditingZaaktypeMapping.value = value);

const isEditingStatusMapping = ref(false);
const setEditingStatusMapping = (value: boolean) => (isEditingStatusMapping.value = value);

const previousOzZaaktypeId = ref<string>("");

const canStartMigration = computed(
  () =>
    mapping.value.detZaaktypeId &&
    mapping.value.ozZaaktypeId &&
    migration.value?.status !== MigrationStatus.inProgress &&
    !isEditingZaaktypeMapping.value &&
    !isEditingStatusMapping.value &&
    statusMappingsComplete.value
);

const isThisMigrationRunning = computed(
  () =>
    migration.value?.status === MigrationStatus.inProgress &&
    migration.value.detZaaktypeId === mapping.value.detZaaktypeId
);

const loading = ref(false);
const errors = ref<unknown[]>([]);

const confirmDialog = useConfirmDialog();
const confirmOzZaaktypeChangeDialog = useConfirmDialog();

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

const fetchStatusMappings = async () => {
  if (!mapping.value.ozZaaktypeId) {
    statusMappingData.value = null;
    statusMappings.value = [];
    statusMappingsComplete.value = false;
    return;
  }

  try {
    const data = await datamigratieService.getStatusMappings(detZaaktypeId, mapping.value.ozZaaktypeId);
    statusMappingData.value = data;
    
    statusMappings.value = data.detStatuses.map(detStatus => {
      const existingMapping = data.existingMappings.find(m => m.detStatusNaam === detStatus.naam);
      return existingMapping || {
        detStatusNaam: detStatus.naam,
        ozStatustypeId: null
      };
    });

    const validation = await datamigratieService.validateStatusMappings(detZaaktypeId);
    statusMappingsComplete.value = validation.allStatusesMapped;
  } catch (err: unknown) {
    console.error("Error fetching status mappings:", err);
    statusMappingData.value = null;
    statusMappings.value = [];
    statusMappingsComplete.value = false;
  }
};

const saveStatusMappings = async () => {
  loading.value = true;

  try {
    const mappingsToSave = statusMappings.value.filter(m => m.ozStatustypeId !== null);

    await datamigratieService.saveStatusMappings({
      detZaaktypeId,
      mappings: mappingsToSave
    });

    toast.add({ text: "De statusmappings zijn succesvol opgeslagen." });

    const validation = await datamigratieService.validateStatusMappings(detZaaktypeId);
    statusMappingsComplete.value = validation.allStatusesMapped;

    // exiting edit mode after successful save
    setEditingStatusMapping(false);
  } catch (err: unknown) {
    toast.add({ text: `Fout bij opslaan van de statusmappings - ${err}`, type: "error" });
  } finally {
    loading.value = false;
  }
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

  if (mapping.value.ozZaaktypeId) {
    previousOzZaaktypeId.value = mapping.value.ozZaaktypeId;
  }

  if (mapping.value.ozZaaktypeId) {
    await fetchStatusMappings();
  }
};

const submitMapping = async () => {
  const hasZaaktypeChanged = mapping.value.ozZaaktypeId !== previousOzZaaktypeId.value;

  const hasStatusMappings = 
    (statusMappingData.value?.existingMappings?.length || 0) > 0 ||
    statusMappings.value.some(m => m.ozStatustypeId !== null);

  if (
    mapping.value.detZaaktypeId &&
    previousOzZaaktypeId.value &&
    hasZaaktypeChanged &&
    hasStatusMappings
  ) {
    const result = await confirmOzZaaktypeChangeDialog.reveal();
    
    if (result.isCanceled) {
      // revert to previous
      mapping.value.ozZaaktypeId = previousOzZaaktypeId.value;
      return;
    }
  }

  loading.value = true;

  setEditingZaaktypeMapping(false);

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

    previousOzZaaktypeId.value = mapping.value.ozZaaktypeId;

    toast.add({ text: "De mapping is succesvol opgeslagen." });

    if (hasZaaktypeChanged) {
      await fetchStatusMappings();
    }
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

// Watch for migration status changes and refresh history when migration completes
watch(
  () => migration.value?.status,
  (newStatus, oldStatus) => {
    // When migration changes from inProgress to none, refresh the history (migration completed)
    if (oldStatus === MigrationStatus.inProgress && newStatus === MigrationStatus.none) {
      fetchMappingData();
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

    &.mapping-display {
      display: flex;
      gap: var(--spacing-default);
      align-items: center;
      flex-wrap: wrap;

      .mapping-edit-button {
        white-space: nowrap;
        margin-block-end: 0;
      }
    }

    &.mapping-controls {
      display: flex;
      gap: var(--spacing-default);
      align-items: center;
      flex-wrap: wrap;

      select {
        flex: 1;
        min-width: 200px;
      }

      .mapping-save-button {
        white-space: nowrap;
        margin-block-end: 0;
      }
    }
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

.status-mapping {
  margin-block-end: var(--spacing-large);

  h2 {
    font-size: 1.5rem;
    margin-block-end: var(--spacing-small);
  }

  p {
    margin-block-end: var(--spacing-default);
  }

  .status-mapping-grid {
    display: flex;
    flex-direction: column;
    gap: var(--spacing-default);
  }

  .mapping-header {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: var(--spacing-default);
    padding: var(--spacing-small);
    background-color: var(--background-secondary);
    font-weight: 600;
    border-radius: 4px;

    @media (max-width: variables.$breakpoint-md) {
      display: none;
    }
  }

  .mapping-row {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: var(--spacing-default);
    padding: var(--spacing-small);
    border: 1px solid var(--border);
    border-radius: 4px;
    align-items: center;

    @media (max-width: variables.$breakpoint-md) {
      grid-template-columns: 1fr;
    }
  }

  .det-status {
    display: flex;
    flex-direction: column;
    gap: var(--spacing-xs, 0.25rem);

    strong {
      font-size: 1rem;
    }

    .status-description {
      font-size: 0.875rem;
      color: var(--text-secondary, #666);
    }
  }

  .oz-status {
    select {
      width: 100%;
      margin-block-end: 0;
    }
  }

  .mapping-actions {
    display: flex;
    justify-content: flex-end;
    margin-block-start: var(--spacing-default);
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
