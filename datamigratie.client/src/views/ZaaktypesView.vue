<template>
  <h1>e-Suite zaaktypes</h1>

  <form @submit.prevent>
    <div class="form-group">
      <label for="filter">Filter</label>

      <input type="text" id="filter" v-model.trim="search" />
    </div>
  </form>

  <simple-spinner v-if="loading" />

  <alert-inline v-else-if="error">{{ error }}</alert-inline>

  <p v-else-if="!filteredZaaktypes.length">Geen zaaktypes gevonden voor "{{ search }}".</p>

  <ul v-else class="reset">
    <li
      v-for="{ naam, functioneleIdentificatie: detZaaktypeId } in filteredZaaktypes"
      :key="detZaaktypeId"
    >
      <router-link
        :to="{
          name: 'detZaaktype',
          params: { detZaaktypeId },
          ...(search && { query: { search } })
        }"
        class="button button-secondary"
        >{{ naam }} <span>&gt;</span></router-link
      >
    </li>
  </ul>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import { detService, type DETZaaktype } from "@/services/detService";

const route = useRoute();

const search = ref("");
const detZaaktypes = ref<DETZaaktype[]>([]);

const filteredZaaktypes = computed(() => {
  const query = search.value.toLowerCase();

  if (!query) return detZaaktypes.value;

  return detZaaktypes.value.filter((zaaktype) => zaaktype.naam.toLowerCase().includes(query));
});

const loading = ref(false);
const error = ref("");

const fetchDETZaaktypes = async () => {
  loading.value = true;
  error.value = "";

  try {
    detZaaktypes.value = await detService.getAllZaaktypes();
  } catch (err: unknown) {
    error.value = `Fout bij ophalen zaaktypes - ${err}`;
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  search.value = String(route.query.search || "");

  fetchDETZaaktypes();
});
</script>

<style lang="scss" scoped>
form {
  margin-block-end: var(--spacing-default);
}

ul {
  display: flex;
  flex-direction: column;
  row-gap: var(--spacing-small);
}

.button {
  display: flex;
  justify-content: space-between;
  margin-block-end: 0;
}
</style>
