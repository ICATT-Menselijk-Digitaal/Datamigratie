<template>
  <section class="mapping-section">
    <h2>Informatieobjecttype voor gegenereerde PDF</h2>
    <p>
      Voor elke zaak wordt er een PDF document gegenereerd waar niet-mapbare zaakgegevens in staan
      die wel gemigreerd moeten worden. Selecteer welk OZ informatieobjecttype deze PDF met
      zaakgegevens krijgt:
    </p>

    <simple-spinner v-if="isLoading" />

    <template v-else>
      <select
        v-model="selectedInformatieobjecttypeId"
        :disabled="(!isEditing && isMapped) || disabled"
      >
        <option value="">Kies een informatieobjecttype</option>
        <option v-for="iot in informatieobjecttypen" :key="iot.id" :value="iot.id">
          {{ iot.omschrijving }}
        </option>
      </select>

      <div v-if="(!isMapped || isEditing) && !disabled" class="mapping-actions">
        <button type="button" @click="saveMapping">Informatieobjecttype opslaan</button>
      </div>

      <div v-if="isMapped && !isEditing && !disabled" class="mapping-actions">
        <button type="button" class="secondary" @click="isEditing = true">
          Informatieobjecttype aanpassen
        </button>
      </div>

      <alert-inline v-if="!isMapped" type="warning">
        Er is nog geen informatieobjecttype geselecteerd voor de gegenereerde PDF. Migratie kan niet
        worden gestart.
      </alert-inline>
    </template>
  </section>
</template>

<script setup lang="ts">
import { ref, computed, watch } from "vue";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import toast from "@/components/toast/toast";
import { get, post } from "@/utils/fetchWrapper";
import type { OZZaaktype } from "@/services/ozService";

type PdfInformatieobjecttypeMappingResponse = {
  ozInformatieobjecttypeId: string;
};

interface Props {
  mappingId: string;
  ozZaaktype: OZZaaktype;
  disabled: boolean;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  (e: "update:complete", value: boolean): void;
}>();

const informatieobjecttypen = computed(() => props.ozZaaktype.informatieobjecttypen ?? []);

const isLoading = ref(false);
const isEditing = ref(false);
const selectedInformatieobjecttypeId = ref("");

const isMapped = computed(() => selectedInformatieobjecttypeId.value !== "");

const fetchMapping = async () => {
  isLoading.value = true;
  try {
    const response = await get<PdfInformatieobjecttypeMappingResponse | null>(
      `/api/mappings/${props.mappingId}/informatieobjecttype`
    );

    selectedInformatieobjecttypeId.value = response?.ozInformatieobjecttypeId ?? "";
  } catch (error) {
    toast.add({
      text: `Fout bij ophalen van de PDF informatieobjecttype mapping - ${error}`,
      type: "error"
    });
    throw error;
  } finally {
    isLoading.value = false;
  }
};

const saveMapping = async () => {
  isLoading.value = true;
  try {
    await post(`/api/mappings/${props.mappingId}/informatieobjecttype`, {
      ozInformatieobjecttypeId: selectedInformatieobjecttypeId.value || null
    });

    toast.add({
      text: "Het informatieobjecttype voor de gegenereerde PDF is succesvol opgeslagen."
    });

    await fetchMapping();

    isEditing.value = !isMapped.value;
  } catch (error) {
    toast.add({
      text: `Fout bij opslaan van het informatieobjecttype voor de gegenereerde PDF - ${error}`,
      type: "error"
    });
    throw error;
  } finally {
    isLoading.value = false;
  }
};

watch(
  () => props.mappingId,
  async () => {
    await fetchMapping();
    isEditing.value = !isMapped.value;
  },
  { immediate: true }
);

watch(isMapped, (value) => {
  emit("update:complete", value && !isEditing.value);
});

watch(isEditing, (value) => {
  emit("update:complete", isMapped.value && !value);
});
</script>

<style lang="scss" scoped>
.mapping-section {
  margin-block-end: var(--spacing-large);

  h2 {
    font-size: 1.5rem;
    margin-block-end: var(--spacing-small);
  }

  p {
    margin-block-end: var(--spacing-default);
  }

  select {
    width: 100%;
    margin-block-end: 0;
  }

  .mapping-actions {
    display: flex;
    justify-content: flex-end;
    margin-block-start: var(--spacing-default);
  }
}
</style>
