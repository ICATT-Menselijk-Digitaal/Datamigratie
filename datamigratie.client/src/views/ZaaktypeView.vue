<template>
  <h1>e-Suite zaaktype</h1>

  <simple-spinner v-if="loading" />

  <form v-else @submit.prevent="submit">
    <alert-inline v-if="errors.length"
      >Fout(en) bij ophalen gegevens zaaktype - {{ errors.join(" | ") }}</alert-inline
    >

    <dl v-else-if="detZaaktype">
      <dt>Naam:</dt>
      <dd>{{ detZaaktype.naam }}</dd>

      <dt>Omschrijving:</dt>
      <dd>{{ detZaaktype.omschrijving }}</dd>

      <dt>Actief:</dt>
      <dd>{{ detZaaktype.actief ? "Ja" : "Nee" }}</dd>

      <dt>Aantal gesloten zaken:</dt>
      <dd>{{ detZaaktype?.closedZaken }}</dd>

      <dt id="mapping">Koppeling OZ zaaktype:</dt>
      <dd>
        <select name="ozUuid" aria-labelledby="mapping" v-model="mapping.ozUuid" required>
          <option v-if="!mapping.ozUuid" value="">Kies Open Zaak zaaktype</option>
          <option v-for="{ uuid, naam } in ozZaaktypes" :value="uuid" :key="uuid">
            {{ naam }}
          </option>
        </select>
      </dd>
    </dl>

    <menu class="reset">
      <li>
        <router-link
          :to="{ name: 'detZaaktypes', ...(search && { query: { search } }) }"
          class="button button-secondary"
          >Annuleren</router-link
        >
      </li>

      <li v-if="!errors.length">
        <button type="submit">Opslaan</button>
      </li>
    </menu>
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import toast from "@/components/toast/toast";
import { detService, type DETZaaktype } from "@/services/detService";
import { ozService, type OZZaaktype } from "@/services/ozService";
import { datamigratieService, type Mapping } from "@/services/datamigratieService";
import { knownErrorMessages } from "@/utils/fetchWrapper";

const { functioneleIdentificatie } = defineProps<{ functioneleIdentificatie: string }>();

const route = useRoute();
const search = computed(() => String(route.query.search || "").trim());

const detZaaktype = ref<DETZaaktype>();
const ozZaaktypes = ref<OZZaaktype[]>();
const mapping = ref<Mapping>({ ozUuid: "" });

const loading = ref(false);
const errors = ref<unknown[]>([]);

const fetchMappingData = async () => {
  loading.value = true;
  errors.value = [];

  try {
    const services = [
      {
        service: detService.getZaaktypeByFunctioneleIdentificatie(functioneleIdentificatie),
        target: detZaaktype
      },
      { service: ozService.getAllZaaktypes(), target: ozZaaktypes },
      {
        service:
          datamigratieService.getMappingByDETFunctioneleIdentificatie(functioneleIdentificatie),
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

        if (
          ignore404 &&
          reason instanceof Error &&
          reason.message === knownErrorMessages.notFound
        ) {
          return;
        }

        errors.value.push(reason);
      }
    });
  } catch (err: unknown) {
    errors.value.push(err);
  } finally {
    loading.value = false;
  }
};

const submit = async () => {
  loading.value = true;

  try {
    if (!mapping.value.detFunctioneleIdentificatie) {
      mapping.value = {
        ...mapping.value,
        detFunctioneleIdentificatie: functioneleIdentificatie
      };

      await datamigratieService.createMapping(mapping.value);
    } else {
      await datamigratieService.updateMapping(mapping.value);
    }

    toast.add({ text: "De mapping is succesvol opgeslagen." });
  } catch (err: unknown) {
    toast.add({ text: `Fout bij opslaan van de mapping - ${err}`, type: "error" });
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
  gap: var(--spacing-default);
  justify-content: space-between;
}
</style>
