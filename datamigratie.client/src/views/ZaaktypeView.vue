<template>
  <h1>DET zaaktype</h1>

  <simple-spinner v-if="loading" />

  <form v-else>
    <alert-inline v-if="error">{{ error }}</alert-inline>

    <dl v-else-if="detZaaktype">
      <dt>Naam:</dt>
      <dd>{{ detZaaktype.naam }}</dd>

      <dt>Omschrijving:</dt>
      <dd>{{ detZaaktype.omschrijving }}</dd>

      <dt>Actief:</dt>
      <dd>{{ detZaaktype.actief ? "Ja" : "Nee" }}</dd>

      <dt>Aantal gesloten zaken:</dt>
      <dd>{{ detZaaktype?.closedZaken }}</dd>
    </dl>

    <menu class="reset">
      <li>
        <router-link
          :to="{ name: 'detZaaktypes', ...(search && { query: { search } }) }"
          class="button button-secondary"
          >Annuleren</router-link
        >
      </li>
    </menu>
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import { detService, type DETZaaktype } from "@/services/detService";

const props = defineProps<{ functioneleIdentificatie: string }>();

const route = useRoute();
const search = computed(() => String(route.query.search || "").trim());

const detZaaktype = ref<DETZaaktype>();

const loading = ref(false);
const error = ref("");

const fetchZaaktypes = async () => {
  loading.value = true;
  error.value = "";

  try {
    const [_detZaaktype] = await Promise.all([
      detService.getZaaktypeByFunctioneleIdentificatie(props.functioneleIdentificatie)
    ]);

    detZaaktype.value = _detZaaktype;
  } catch (err: unknown) {
    error.value = `Fout bij ophalen zaaktypes - ${err}`;
  } finally {
    loading.value = false;
  }
};

onMounted(() => fetchZaaktypes());
</script>

<style lang="scss" scoped>
dl {
  display: grid;
  grid-template-columns: max-content 1fr;
  gap: var(--spacing-default);

  dt {
    grid-column: 1;
    font-weight: 600;

    &[id] {
      align-self: center;
    }
  }

  dd {
    grid-column: 2;
    margin-inline: 0;
  }

  select {
    min-inline-size: var(--section-width-small);
    margin-block-end: 0;
  }
}

menu {
  display: flex;
  gap: var(--spacing-default);
  justify-content: space-between;
}
</style>
