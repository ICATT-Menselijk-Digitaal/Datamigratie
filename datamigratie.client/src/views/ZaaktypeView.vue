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
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { useConfirmDialog } from "@vueuse/core";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import PromptModal from "@/components/PromptModal.vue";
import toast from "@/components/toast/toast";
import { detService, type DETZaaktype } from "@/services/detService";
import { ozService, type OZZaaktype } from "@/services/ozService";
import {
  datamigratieService,
  type ZaaktypeMapping,
  type UpdateZaaktypeMapping
} from "@/services/datamigratieService";
import { knownErrorMessages } from "@/utils/fetchWrapper";
import { useMigrationStatus } from "@/composables/use-migration-status";

const { detZaaktypeId } = defineProps<{ detZaaktypeId: string }>();

const route = useRoute();
const search = computed(() => String(route.query.search || "").trim());

const detZaaktype = ref<DETZaaktype>();
const ozZaaktypes = ref<OZZaaktype[]>();
const mapping = ref({ ozZaaktypeId: "" } as ZaaktypeMapping);

const { migrationStatus, fetchMigrationStatus } = useMigrationStatus();

const isEditMode = ref(false);
const setEditMode = (value: boolean) => (isEditMode.value = value);

const canStartMigration = computed(
  () =>
    !isEditMode.value &&
    !migrationStatus.value?.isRunning &&
    mapping.value.detZaaktypeId &&
    mapping.value.ozZaaktypeId
);

const isThisMigrationRunning = computed(
  () =>
    migrationStatus.value?.isRunning &&
    migrationStatus.value.detZaaktypeId === mapping.value.detZaaktypeId
);

const loading = ref(false);
const errors = ref<unknown[]>([]);

const confirmDialog = useConfirmDialog();

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
    await datamigratieService.startMigration({
      detZaaktypeId,
      isRunning: true
    });

    fetchMigrationStatus();
  } catch (err: unknown) {
    toast.add({ text: `Fout bij starten van de migratie - ${err}`, type: "error" });
  } finally {
    loading.value = false;
  }
};

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
</style>
