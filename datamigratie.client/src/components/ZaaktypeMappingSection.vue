<template>
  <simple-spinner v-if="isLoading" />
  <form @submit.prevent="submit" v-else-if="detZaaktype">
    <dl>
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

      <dt id="mapping" class="accentuate">Koppeling OZ zaaktype:</dt>
      <dd v-if="!isEditing" class="mapping-display accentuate">
        {{ model?.ozZaaktype.identificatie }}
      </dd>
      <dd v-else class="mapping-controls accentuate">
        <select name="ozZaaktypeId" aria-labelledby="mapping" v-model="selectedZaaktypeId" required>
          <option v-if="!selectedZaaktypeId" value="">Kies Open Zaak zaaktype</option>
          <option v-for="{ id, identificatie, omschrijving } in ozZaaktypes" :value="id" :key="id">
            {{ identificatie }} – {{ omschrijving }}
          </option>
        </select>
      </dd>
    </dl>
    <ul class="reset form-buttons" v-if="!disabled && isEditing">
      <li><button type="submit" class="mapping-save-button">Koppeling opslaan</button></li>
      <li>
        <button @click="handleCancel" type="button" class="secondary">Annuleren</button>
      </li>
    </ul>
  </form>
  <button
    type="button"
    class="secondary mapping-edit-button"
    @click="isEditing = true"
    v-if="!disabled && !isEditing"
  >
    Koppeling aanpassen
  </button>
  <zaaktype-change-confirmation-modal
    :dialog="confirmOzZaaktypeChangeDialog"
    warning-text="Als je het Open Zaak zaaktype wijzigt, worden alle bestaande mappings verwijderd."
    description-text="Je moet de mappings opnieuw configureren voor het nieuwe zaaktype."
  />
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
      } catch (error) {
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

function handleCancel() {
  isEditing.value = false;
  selectedZaaktypeId.value = zaaktypeMapping.value?.ozZaaktypeId ?? "";
}
</script>

<style scoped lang="scss">
@use "@/assets/variables";
dl {
  display: grid;
  margin-block-end: var(--spacing-large);

  dt,
  dd {
    padding-block-end: var(--spacing-default);
  }

  dt {
    padding-inline-start: var(--spacing-small);
    padding-inline-end: var(--spacing-large);
    color: var(--text);
    font-weight: 600;
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

  .accentuate {
    background-color: var(--accent-bg);
    padding-block: 0;
    display: flex;
    align-items: center;
    min-height: 3rem;
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

.form-buttons {
  display: flex;
  gap: var(--spacing-small);
}
</style>
