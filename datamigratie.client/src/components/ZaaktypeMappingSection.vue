<template>
  <div class="zaaktype-mapping-section">
    <simple-spinner v-if="isLoading" />
    <table v-else-if="detZaaktype" class="zaaktype-properties">
      <tbody>
        <tr>
          <th>Naam:</th>
          <td>{{ detZaaktype.naam }}</td>
        </tr>

        <tr>
          <th>Omschrijving:</th>
          <td>{{ detZaaktype.omschrijving }}</td>
        </tr>

        <tr>
          <th>Functionele identificatie:</th>
          <td>{{ detZaaktype.functioneleIdentificatie }}</td>
        </tr>

        <tr>
          <th>Actief:</th>
          <td>{{ detZaaktype.actief ? "Ja" : "Nee" }}</td>
        </tr>

        <tr>
          <th>Aantal gesloten zaken:</th>
          <td>{{ detZaaktype?.closedZakenCount }}</td>
        </tr>

        <tr class="mapping-row">
          <th id="mapping">Koppeling OZ zaaktype:</th>
          <td v-if="!isEditing && !!model?.ozZaaktype" class="mapping-display">
            {{ model.ozZaaktype.identificatie }}
          </td>
          <td v-else class="mapping-controls">
            <select
              name="ozZaaktypeId"
              aria-labelledby="mapping"
              v-model="selectedZaaktypeId"
              required
            >
              <option v-if="!selectedZaaktypeId" value="">- Kies een zaaktype -</option>
              <option
                v-for="{ id, identificatie, omschrijving } in ozZaaktypes"
                :value="id"
                :key="id"
              >
                {{ identificatie }} – {{ omschrijving }}
              </option>
            </select>
          </td>
        </tr>
      </tbody>
    </table>

    <div class="koppeling-actions">
      <template v-if="!isEditing && model?.ozZaaktype">
        <button type="button" class="edit-button" @click="isEditing = true" :disabled="disabled">
          Koppeling aanpassen
        </button>
      </template>
      <template v-else>
        <button type="button" class="primary-button" @click="submit">Koppeling opslaan</button>
        <button type="button" class="cancel-button" @click="handleCancel">Annuleren</button>
      </template>
    </div>

    <zaaktype-change-confirmation-modal
      :dialog="confirmOzZaaktypeChangeDialog"
      warning-text="Als je het Open Zaak zaaktype wijzigt, worden alle bestaande mappings verwijderd."
      description-text="Je moet de mappings opnieuw configureren voor het nieuwe zaaktype."
    />
  </div>
</template>

<script setup lang="ts">
import { detService, type DETZaaktype } from "@/services/detService";
import { ozService, type OZZaaktype } from "@/services/ozService";
import type { ZaaktypeMapping } from "@/types/datamigratie";
import { get, post, put, swallow404 } from "@/utils/fetchWrapper";
import { onMounted, ref, watch } from "vue";
import SimpleSpinner from "./SimpleSpinner.vue";
import toast from "./toast/toast";
import ZaaktypeChangeConfirmationModal from "./ZaaktypeChangeConfirmationModal.vue";
import { useConfirmDialog } from "@vueuse/core";

export type ZaaktypeMappingModel = {
  id: string;
  detZaaktype: DETZaaktype;
  ozZaaktype: OZZaaktype;
};

const props = defineProps<{
  detZaaktypeId: string;
  disabled: boolean;
}>();

const model = defineModel<ZaaktypeMappingModel | undefined>("zaaktypeMapping", {
  required: true
});

const isLoading = ref(false);
const isEditing = ref(false);

const selectedZaaktypeId = ref("");

const zaaktypeMapping = ref<ZaaktypeMapping>();
const ozZaaktypes = ref<OZZaaktype[]>();
const detZaaktype = ref<DETZaaktype>();
const confirmOzZaaktypeChangeDialog = useConfirmDialog();

const fetchMapping = (id: string) => get<ZaaktypeMapping>(`/api/mapping/${id}`).catch(swallow404);

function handleCancel() {
  // Reset to the saved value
  if (zaaktypeMapping.value?.ozZaaktypeId) {
    selectedZaaktypeId.value = zaaktypeMapping.value.ozZaaktypeId;
  }
  isEditing.value = false;
}

async function submit() {
  // no selected value, shouldn't happen because it is required in the form
  if (!selectedZaaktypeId.value) {
    return;
  }
  // unchanged, no need to save, exit edit mode
  if (zaaktypeMapping.value?.ozZaaktypeId === selectedZaaktypeId.value) {
    isEditing.value = false;
    return;
  }
  // had a value, but changed, check for confirmation first
  if (zaaktypeMapping.value?.ozZaaktypeId) {
    const { isCanceled } = await confirmOzZaaktypeChangeDialog.reveal();
    if (isCanceled) {
      isEditing.value = false;
      return;
    }
  }
  isLoading.value = true;
  try {
    if (!zaaktypeMapping.value?.id) {
      await post(`/api/mapping/${props.detZaaktypeId}`, {
        ozZaaktypeId: selectedZaaktypeId.value
      });
    } else {
      await put(`/api/mapping/${props.detZaaktypeId}`, {
        updatedOzZaaktypeId: selectedZaaktypeId.value
      });
    }
    zaaktypeMapping.value = await fetchMapping(props.detZaaktypeId);
    isEditing.value = false;
  } catch (error) {
    toast.add({ text: `Fout bij opslaan van de zaaktype mapping - ${error}`, type: "error" });
  } finally {
    isLoading.value = false;
  }
}

watch(
  () => props.detZaaktypeId,
  async (id) => {
    isLoading.value = true;
    try {
      detZaaktype.value = await detService.getZaaktypeById(id);
      zaaktypeMapping.value = await fetchMapping(id);
    } catch (error) {
      toast.add({ text: `Fout bij ophalen van de zaaktype mapping - ${error}`, type: "error" });
    } finally {
      isLoading.value = false;
    }
  },
  { immediate: true }
);

watch(
  [zaaktypeMapping, detZaaktype],
  async ([{ id, ozZaaktypeId } = {}, detZaaktype]) => {
    if (id && detZaaktype && ozZaaktypeId) {
      selectedZaaktypeId.value = ozZaaktypeId;
      isLoading.value = true;
      try {
        model.value = {
          id,
          ozZaaktype: await ozService.getZaaktypeById(ozZaaktypeId),
          detZaaktype
        };
      } catch {
        // Silently handle errors when fetching OZ zaaktype
      } finally {
        isLoading.value = false;
      }
    }
  },
  { immediate: true }
);

onMounted(async () => {
  ozZaaktypes.value = await ozService.getAllZaaktypes();
});
</script>

<style scoped lang="scss">
@use "@/assets/variables";

.zaaktype-mapping-section {
  width: 100%;
}

.zaaktype-properties {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  margin-block-end: 0;

  tbody tr {
    background: white;

    &:not(:last-child) {
      border-bottom: 1px solid var(--border, #e0e0e0);
    }
  }

  th,
  td {
    padding: 8px;
    text-align: left;
    border: none;
    vertical-align: middle;
    background: inherit;
  }

  th {
    color: var(--Zwart, #212121);
    font-family: Avenir;
    font-size: 16px;
    font-style: normal;
    font-weight: 800;
    line-height: 20px;
    white-space: nowrap;
  }

  td {
    color: var(--Zwart, #212121);
    font-family: Avenir;
    font-size: 16px;
    font-style: normal;
    font-weight: 400;
    line-height: 20px;
  }

  // only the last row gets the colored background
  tbody tr.mapping-row {
    background: var(--Accent-background, #f5f7ff);

    th,
    td {
      height: 52px;
    }
  }

  .mapping-controls {
    select {
      flex: 1;
      min-width: 200px;
      max-width: 100%;
      margin-block-end: 0;
    }
  }
}

.koppeling-actions {
  display: flex;
  flex-direction: row;
  align-items: flex-start;
  gap: 8px;
  margin-top: 16px;

  button {
    margin-block-end: 0;
  }

  // Button styles are defined in main.scss
}
</style>
