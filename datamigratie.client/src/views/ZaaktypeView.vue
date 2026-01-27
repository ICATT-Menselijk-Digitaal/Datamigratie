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

      <dt>Functionele identificatie:</dt>
      <dd>{{ detZaaktype.functioneleIdentificatie }}</dd>

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

          <option v-for="{ id, identificatie, omschrijving } in ozZaaktypes" :value="id" :key="id">
            {{ identificatie }} – {{ omschrijving }}
          </option>
        </select>
        <button type="submit" class="mapping-save-button">Mapping opslaan</button>
      </dd>
    </dl>

    <status-mapping-section
      :det-zaaktype="detZaaktype"
      :oz-zaaktype="ozZaaktype"
      :status-mappings="statusMappings"
      :all-mapped="statusMappingsComplete"
      :is-editing="isEditingStatusMapping"
      :disabled="isThisMigrationRunning"
      :loading="!ozZaaktype"
      :show-warning="!!mapping.detZaaktypeId"
      :show-mapping="showDetailMapping"
      @update:status-mappings="statusMappings = $event"
      @save="saveStatusMappings"
    />

    <menu class="reset edit-menu">
      <li v-if="canEditStatusMappings">
        <button type="button" class="secondary" @click="setEditingStatusMapping(true)">
          Statusmappings aanpassen
        </button>
      </li>
    </menu>

    <resultaattype-mapping-section
      :det-zaaktype="detZaaktype"
      :oz-zaaktype="ozZaaktype"
      :resultaattype-mappings="resultaattypeMappings"
      :all-mapped="resultaattypeMappingsComplete"
      :is-editing="isEditingResultaattypeMapping"
      :disabled="isThisMigrationRunning"
      :loading="!ozZaaktype"
      :show-warning="!!mapping.detZaaktypeId"
      :show-mapping="showDetailMapping"
      @update:resultaattype-mappings="resultaattypeMappings = $event"
      @save="saveResultaattypeMappings"
    />

    <menu class="reset edit-menu">
      <li v-if="canEditResultaattypeMappings">
        <button type="button" class="secondary" @click="setEditingResultaattypeMapping(true)">
          Resultaattypemappings aanpassen
        </button>
      </li>
    </menu>

    <besluittype-mapping-section
      :det-besluittypen="detBesluittypen"
      :oz-zaaktype="ozZaaktype"
      :besluittype-mappings="besluittypeMappings"
      :all-mapped="besluittypeMappingsComplete"
      :is-editing="isEditingBesluittypeMapping"
      :disabled="isThisMigrationRunning"
      :loading="!ozZaaktype"
      :show-warning="!!mapping.detZaaktypeId"
      :show-mapping="showDetailMapping"
      @update:besluittype-mappings="besluittypeMappings = $event"
      @save="saveBesluittypeMappings"
    />

    <menu class="reset edit-menu">
      <li v-if="canEditBesluittypeMappings">
        <button type="button" class="secondary" @click="setEditingBesluittypeMapping(true)">
          Besluittypemappings aanpassen
        </button>
      </li>
    </menu>

    <document-property-mapping-section
      :det-documenttypen="detDocumenttypen"
      :oz-zaaktype="ozZaaktype"
      :document-property-mappings="documentPropertyMappings"
      :is-editing-publicatieniveau="isEditingPublicatieNiveauMapping"
      :is-editing-documenttype="isEditingDocumenttypeMapping"
      :disabled="isThisMigrationRunning"
      :loading="!ozZaaktype"
      :show-warning="!!mapping.detZaaktypeId"
      :show-mapping="showDetailMapping"
      @update:document-property-mappings="documentPropertyMappings = $event"
      @save="saveDocumentPropertyMappings"
      @edit-publicatieniveau="setEditingPublicatieNiveauMapping(true)"
      @edit-documenttype="setEditingDocumenttypeMapping(true)"
    />

    <menu class="reset">
      <li>
        <router-link
          :to="{ name: 'detZaaktypes', ...(search && { query: { search } }) }"
          class="button button-secondary"
          >&lt; Terug</router-link
        >
      </li>

      <template v-if="!errors.length && !isThisMigrationRunning">
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

    <zaaktype-change-confirmation-modal
      :dialog="confirmOzZaaktypeChangeDialog"
      warning-text="Als je het Open Zaak zaaktype wijzigt, worden alle bestaande mappings verwijderd."
      description-text="Je moet de mappings opnieuw configureren voor het nieuwe zaaktype."
    />

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
import StatusMappingSection from "@/components/StatusMappingSection.vue";
import BesluittypeMappingSection from "@/components/BesluittypeMappingSection.vue";
import ZaaktypeChangeConfirmationModal from "@/components/ZaaktypeChangeConfirmationModal.vue";
import toast from "@/components/toast/toast";
import { detService, type DETZaaktype, type DetDocumenttype, type DetBesluittype } from "@/services/detService";
import { ozService, type OZZaaktype } from "@/services/ozService";
import {
  datamigratieService,
  MigrationStatus,
  type ZaaktypeMapping,
  type CreateZaaktypeMapping,
  type UpdateZaaktypeMapping,
  type MigrationHistoryItem,
  type StatusMappingItem,
  type ResultaattypeMappingItem,
  type DocumentPropertyMappingItem,
  type BesluittypeMappingItem
} from "@/services/datamigratieService";
import { knownErrorMessages } from "@/utils/fetchWrapper";
import { useMigration } from "@/composables/use-migration-status";
import ResultaattypeMappingSection from "@/components/ResultaattypeMappingSection.vue";
import DocumentPropertyMappingSection from "@/components/DocumentPropertyMappingSection.vue";

const { detZaaktypeId } = defineProps<{ detZaaktypeId: string }>();

const route = useRoute();
const router = useRouter();
const search = computed(() => String(route.query.search || "").trim());

const detZaaktype = ref<DETZaaktype>();
const ozZaaktype = ref<OZZaaktype>();
const ozZaaktypes = ref<OZZaaktype[]>();
const mapping = ref({ ozZaaktypeId: "" } as ZaaktypeMapping);
const migrationHistory = ref<MigrationHistoryItem[]>([]);

const statusMappings = ref<StatusMappingItem[]>([]);

const statusMappingsComplete = ref(false);

const showDetailMapping = computed(() => !!(mapping.value.detZaaktypeId && mapping.value.ozZaaktypeId));

const resultaattypeMappings = ref<ResultaattypeMappingItem[]>([]);

const resultaattypeMappingsComplete = ref(false);

const detBesluittypen = ref<DetBesluittype[]>([]);

const besluittypeMappings = ref<BesluittypeMappingItem[]>([]);

const besluittypeMappingsComplete = ref(false);

const detDocumenttypen = ref<DetDocumenttype[]>([]);

const documentPropertyMappings = ref<DocumentPropertyMappingItem[]>([]);

const documentPropertyMappingsComplete = ref(false);

// flag to prevent re-entrant calls
let isFetchingDocumentPropertyMappings = false;

const { migration, fetchMigration } = useMigration();

const isEditingZaaktypeMapping = ref(false);
const setEditingZaaktypeMapping = (value: boolean) => (isEditingZaaktypeMapping.value = value);

const isEditingStatusMapping = ref(false);
const setEditingStatusMapping = (value: boolean) => (isEditingStatusMapping.value = value);

const isEditingResultaattypeMapping = ref(false);
const setEditingResultaattypeMapping = (value: boolean) => (isEditingResultaattypeMapping.value = value);

const isEditingBesluittypeMapping = ref(false);
const setEditingBesluittypeMapping = (value: boolean) => (isEditingBesluittypeMapping.value = value);

const isEditingPublicatieNiveauMapping = ref(true);
const setEditingPublicatieNiveauMapping = (value: boolean) => (isEditingPublicatieNiveauMapping.value = value);

const isEditingDocumenttypeMapping = ref(true);
const setEditingDocumenttypeMapping = (value: boolean) => (isEditingDocumenttypeMapping.value = value);

const previousOzZaaktypeId = ref<string>("");

const canEditStatusMappings = computed(
  () =>
    !isThisMigrationRunning.value &&
    mapping.value.detZaaktypeId &&
    mapping.value.ozZaaktypeId &&
    !isEditingZaaktypeMapping.value &&
    !isEditingStatusMapping.value &&
    statusMappingsComplete.value
);

const canEditResultaattypeMappings = computed(
  () =>
    !isThisMigrationRunning.value &&
    mapping.value.detZaaktypeId &&
    mapping.value.ozZaaktypeId &&
    !isEditingZaaktypeMapping.value &&
    !isEditingResultaattypeMapping.value &&
    resultaattypeMappingsComplete.value
);

const canEditBesluittypeMappings = computed(
  () =>
    !isThisMigrationRunning.value &&
    mapping.value.detZaaktypeId &&
    mapping.value.ozZaaktypeId &&
    !isEditingZaaktypeMapping.value &&
    !isEditingBesluittypeMapping.value &&
    besluittypeMappingsComplete.value
);

const canEditDocumentPropertyMappings = computed(
  () =>
    !isThisMigrationRunning.value &&
    mapping.value.detZaaktypeId &&
    mapping.value.ozZaaktypeId &&
    !isEditingZaaktypeMapping.value &&
    (!isEditingPublicatieNiveauMapping.value || !isEditingDocumenttypeMapping.value) &&
    documentPropertyMappingsComplete.value
);

const canStartMigration = computed(
  () =>
    mapping.value.detZaaktypeId &&
    mapping.value.ozZaaktypeId &&
    migration.value?.status !== MigrationStatus.inProgress &&
    !isEditingZaaktypeMapping.value &&
    !isEditingStatusMapping.value &&
    statusMappingsComplete.value &&
    !isEditingResultaattypeMapping.value &&
    resultaattypeMappingsComplete.value &&
    !isEditingBesluittypeMapping.value &&
    besluittypeMappingsComplete.value &&
    !isEditingPublicatieNiveauMapping.value &&
    !isEditingDocumenttypeMapping.value &&
    documentPropertyMappingsComplete.value
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
    ozZaaktype.value = undefined;
    statusMappings.value = [];
    statusMappingsComplete.value = false;
    return;
  }

  try {
    // Fetch OZ zaaktype with statustypes
    ozZaaktype.value = await ozService.getZaaktypeById(mapping.value.ozZaaktypeId);
    
    // Fetch existing mappings -> if mapping has been already saved
    let mappingsData: StatusMappingItem[] = [];
    if (mapping.value.id) {
      mappingsData = await datamigratieService.getStatusMappings(mapping.value.id);
    }
    
    // Build complete mapping list from DET statuses
    const activeDetStatuses = detZaaktype.value?.statuses?.filter(s => s.actief) || [];
    statusMappings.value = activeDetStatuses.map(detStatus => {
      const existingMapping = mappingsData.find(m => m.detStatusNaam === detStatus.naam);
      return existingMapping || {
        detStatusNaam: detStatus.naam,
        ozStatustypeId: null
      };
    });

    // Check if all statuses are mapped
    statusMappingsComplete.value = statusMappings.value.length > 0 && 
      statusMappings.value.every(m => m.ozStatustypeId !== null);
  } catch (err: unknown) {
    console.error("Error fetching status mappings:", err);
    ozZaaktype.value = undefined;
    statusMappings.value = [];
    statusMappingsComplete.value = false;
  }
};

const fetchResultaattypenMappings = async () => {
  if (!mapping.value.ozZaaktypeId) {
    ozZaaktype.value = undefined;
    resultaattypeMappings.value = [];
    statusMappingsComplete.value = false;
    return;
  }

  try {
    // Fetch OZ zaaktype with statustypes
    if (!ozZaaktype.value) {
        ozZaaktype.value = await ozService.getZaaktypeById(mapping.value.ozZaaktypeId);
    }
    
    // Fetch existing mappings -> if mapping has been already saved
    let mappingsData: ResultaattypeMappingItem[] = [];
    if (mapping.value.id) {
      mappingsData = await datamigratieService.getResultaattypeMappings(mapping.value.id);
    }
    
    // Build complete mapping list from DET resultaattypen
    const activeDetResultaattypen = detZaaktype.value?.resultaten?.filter(s => s.resultaat.actief) || [];
    resultaattypeMappings.value = activeDetResultaattypen.map(detResultaattypen => {
      const existingMapping = mappingsData.find(m => m.detResultaattypeNaam === detResultaattypen.resultaat.naam);
      return existingMapping || {
        detResultaattypeNaam: detResultaattypen.resultaat.naam,
        ozResultaattypeId: null
      };
    });

    // Check if all resultaattypen are mapped
    resultaattypeMappingsComplete.value = resultaattypeMappings.value.length > 0 && 
      resultaattypeMappings.value.every(m => m.ozResultaattypeId !== null);
  } catch (err: unknown) {
    console.error("Error fetching resultaattypen mappings:", err);
    ozZaaktype.value = undefined;
    resultaattypeMappings.value = [];
    resultaattypeMappingsComplete.value = false;
  }
};

const saveResultaattypeMappings = async () => {
  loading.value = true;

  try {    
    const mappingsToSave = resultaattypeMappings.value.filter(m => m.ozResultaattypeId !== null);
  
    await datamigratieService.saveResultaattypeMappings(mapping.value.id, {
      mappings: mappingsToSave
    });

    toast.add({ text: "De resultaattype mappings zijn succesvol opgeslagen." });

    // Recalculate completion status
    resultaattypeMappingsComplete.value = resultaattypeMappings.value.length > 0 && 
      resultaattypeMappings.value.every(m => m.ozResultaattypeId !== null);

    // exiting edit mode after successful save
    setEditingResultaattypeMapping(false);
  } catch (err: unknown) {
    toast.add({ text: `Fout bij opslaan van de resultaattype mappings - ${err}`, type: "error" });
  } finally {
    loading.value = false;
  }
};

const fetchBesluittypeMappings = async () => {
  if (!mapping.value.ozZaaktypeId) {
    detBesluittypen.value = [];
    besluittypeMappings.value = [];
    besluittypeMappingsComplete.value = false;
    return;
  }

  try {
    // Fetch OZ zaaktype with besluittypen
    if (!ozZaaktype.value) {
      ozZaaktype.value = await ozService.getZaaktypeById(mapping.value.ozZaaktypeId);
    }

    // Fetch all DET besluittypen
    const detBesluittypeData = await detService.getAllBesluittypen();
    detBesluittypen.value = detBesluittypeData;
    
    // Fetch existing mappings -> if mapping has been already saved
    let mappingsData: BesluittypeMappingItem[] = [];
    if (mapping.value.id) {
      const apiResponse = await datamigratieService.getBesluittypeMappings(mapping.value.id);
      mappingsData = apiResponse.map(m => ({
        detBesluittypeNaam: m.detBesluittypeNaam,
        ozBesluittypeId: m.ozBesluittypeId
      }));
    }
    
    // Build complete mapping list from DET besluittypen
    const activeDetBesluittypen = detBesluittypeData.filter(b => b.actief);
    besluittypeMappings.value = activeDetBesluittypen.map(detBesluittype => {
      const existingMapping = mappingsData.find(m => m.detBesluittypeNaam === detBesluittype.naam);
      return existingMapping || {
        detBesluittypeNaam: detBesluittype.naam,
        ozBesluittypeId: null
      };
    });

    // Check if all besluittypen are mapped
    besluittypeMappingsComplete.value = besluittypeMappings.value.length > 0 && 
      besluittypeMappings.value.every(m => m.ozBesluittypeId !== null);
  } catch (err: unknown) {
    console.error("Error fetching besluittype mappings:", err);
    detBesluittypen.value = [];
    besluittypeMappings.value = [];
    besluittypeMappingsComplete.value = false;
  }
};

const saveBesluittypeMappings = async () => {
  loading.value = true;

  try {    
    const mappingsToSave = besluittypeMappings.value.filter(m => m.ozBesluittypeId !== null);
  
    await datamigratieService.saveBesluittypeMappings(mapping.value.id, {
      mappings: mappingsToSave
    });

    toast.add({ text: "De besluittype mappings zijn succesvol opgeslagen." });

    // Recalculate completion status
    besluittypeMappingsComplete.value = besluittypeMappings.value.length > 0 && 
      besluittypeMappings.value.every(m => m.ozBesluittypeId !== null);

    // exiting edit mode after successful save
    setEditingBesluittypeMapping(false);
  } catch (err: unknown) {
    toast.add({ text: `Fout bij opslaan van de besluittype mappings - ${err}`, type: "error" });
  } finally {
    loading.value = false;
  }
};

const fetchDocumentPropertyMappings = async () => {
  if (isFetchingDocumentPropertyMappings) {
    return;
  }

  isFetchingDocumentPropertyMappings = true;

  if (!mapping.value.ozZaaktypeId) {
    documentPropertyMappings.value = [];
    documentPropertyMappingsComplete.value = false;
    isFetchingDocumentPropertyMappings = false;
    return;
  }

  try {
    let ozZaaktypeData = ozZaaktype.value;
    if (!ozZaaktypeData) {
      ozZaaktypeData = await ozService.getZaaktypeById(mapping.value.ozZaaktypeId);
    }

    const detDocumenttypenData = await detService.getAllDocumenttypen();
    const publicatieNiveauValuesData = await datamigratieService.getPublicatieNiveauOptions();

    let mappingsData: DocumentPropertyMappingItem[] = [];
    if (mapping.value.id) {
      const apiResponse = await datamigratieService.getDocumentPropertyMappings(mapping.value.id);
      mappingsData = apiResponse as DocumentPropertyMappingItem[];
    }

    const publicatieNiveauMappings = publicatieNiveauValuesData.map((val: string) => {
      const existingMapping = mappingsData.find(m => m.detPropertyName === "publicatieniveau" && m.detValue === val);
      return existingMapping || {
        detPropertyName: "publicatieniveau",
        detValue: val,
        ozValue: null
      };
    });

    const activeDocumenttypen = detDocumenttypenData.filter(dt => dt.actief);
    const documenttypeMappings = activeDocumenttypen.map(dt => {
      const existingMapping = mappingsData.find(m => m.detPropertyName === "documenttype" && m.detValue === dt.naam);
      return existingMapping || {
        detPropertyName: "documenttype",
        detValue: dt.naam,
        ozValue: null
      };
    });

    const publicatieNiveauMapped = publicatieNiveauMappings.every(m => m.ozValue !== null);
    const documenttypeMapped = documenttypeMappings.length === 0 || documenttypeMappings.every(m => m.ozValue !== null);

    const savedPublicatieNiveauMappings = mappingsData.filter(m => m.detPropertyName === "publicatieniveau");
    const savedDocumenttypeMappings = mappingsData.filter(m => m.detPropertyName === "documenttype");
    
    const shouldEditPublicatieniveau = !(publicatieNiveauMapped && savedPublicatieNiveauMappings.length > 0);
    const shouldEditDocumenttype = !(documenttypeMapped && savedDocumenttypeMappings.length > 0);

    if (!ozZaaktype.value) {
      ozZaaktype.value = ozZaaktypeData;
    }
    detDocumenttypen.value = detDocumenttypenData;
    documentPropertyMappings.value = [...publicatieNiveauMappings, ...documenttypeMappings];
    documentPropertyMappingsComplete.value = publicatieNiveauMapped && documenttypeMapped;
    
    // Only update editing states if they need to change
    if (isEditingPublicatieNiveauMapping.value !== shouldEditPublicatieniveau) {
      setEditingPublicatieNiveauMapping(shouldEditPublicatieniveau);
    }
    if (isEditingDocumenttypeMapping.value !== shouldEditDocumenttype) {
      setEditingDocumenttypeMapping(shouldEditDocumenttype);
    }
  } catch (err: unknown) {
    console.error("Error fetching document property mappings:", err);
    documentPropertyMappings.value = [];
    documentPropertyMappingsComplete.value = false;
  } finally {
    isFetchingDocumentPropertyMappings = false;
  }
};

const saveDocumentPropertyMappings = async () => {
  loading.value = true;

  try {
    const mappingsToSave = documentPropertyMappings.value.filter(m => m.ozValue !== null && m.ozValue !== "");

    await datamigratieService.saveDocumentPropertyMappings(mapping.value.id, {
      mappings: mappingsToSave
    });

    toast.add({ text: "De documentproperty mappings zijn succesvol opgeslagen." });

    await fetchDocumentPropertyMappings();
  } catch (err: unknown) {
    toast.add({ text: `Fout bij opslaan van de documentproperty mappings - ${err}`, type: "error" });
  } finally {
    loading.value = false;
  }
};

const saveStatusMappings = async () => {
  loading.value = true;

  try {
    const mappingsToSave = statusMappings.value.filter(m => m.ozStatustypeId !== null);
  
    await datamigratieService.saveStatusMappings(mapping.value.id, {
      mappings: mappingsToSave
    });

    toast.add({ text: "De statusmappings zijn succesvol opgeslagen." });

    // Recalculate completion status
    statusMappingsComplete.value = statusMappings.value.length > 0 && 
      statusMappings.value.every(m => m.ozStatustypeId !== null);

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
    await fetchResultaattypenMappings();
    await fetchBesluittypeMappings();
    await fetchDocumentPropertyMappings();
  }
};

const submitMapping = async () => {
  const hasZaaktypeChanged = mapping.value.ozZaaktypeId !== previousOzZaaktypeId.value;

  const hasStatusMappings = 
    statusMappings.value.some(m => m.ozStatustypeId !== null);

  const hasResultaattypeMappings = 
    resultaattypeMappings.value.some(m => m.ozResultaattypeId !== null);

  const hasBesluittypeMappings = 
    besluittypeMappings.value.some(m => m.ozBesluittypeId !== null);

  const hasDocumentPropertyMappings = 
    documentPropertyMappings.value.some(m => m.ozValue !== null);

  if (
    mapping.value.detZaaktypeId &&
    previousOzZaaktypeId.value &&
    hasZaaktypeChanged &&
    (hasStatusMappings ||
    hasResultaattypeMappings ||
    hasBesluittypeMappings ||
    hasDocumentPropertyMappings)
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

      const createMapping: CreateZaaktypeMapping = {
        detZaaktypeId: mapping.value.detZaaktypeId,
        ozZaaktypeId: mapping.value.ozZaaktypeId
      };

      await datamigratieService.createMapping(createMapping);
      
      // getting ID of the created mapping
      const createdMapping = await datamigratieService.getMappingByDETZaaktypeId(detZaaktypeId);
      mapping.value = createdMapping;
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
      await fetchResultaattypenMappings();
      await fetchBesluittypeMappings();
      await fetchDocumentPropertyMappings();
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
        max-width: 100%;

        option {
          white-space: normal;
          overflow-wrap: break-word;
          word-wrap: break-word;
          padding: var(--spacing-small);
        }
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

  &.edit-menu {
    @media (min-width: variables.$breakpoint-md) {
      li:only-of-type {
        margin-inline-start: auto;
        margin-inline-end: 0;
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
