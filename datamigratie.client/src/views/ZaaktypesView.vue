<template>
  <h1>Zaaktypes</h1>

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
      v-for="{ naam, functioneleIdentificatie } in filteredZaaktypes"
      :key="functioneleIdentificatie"
    >
      <router-link
        :to="{
          name: 'zaaktype',
          params: { functioneleIdentificatie },
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
import type { Zaaktype } from "@/types/zaaktype";
import { zaaktypeService } from "@/services/zaaktypeService";

const route = useRoute();

const search = ref("");
const zaaktypes = ref<Zaaktype[]>([]);

const filteredZaaktypes = computed(() => {
  let result = zaaktypes.value;

  const query = search.value.toLowerCase();

  if (query) {
    result = zaaktypes.value.filter((zaaktype) => zaaktype.naam.toLowerCase().includes(query));
  }

  return result.sort((a, b) => a.naam.toLowerCase().localeCompare(b.naam.toLowerCase()));
});

const loading = ref(false);
const error = ref("");

const fetchZaaktypes = async () => {
  loading.value = true;
  error.value = "";

  try {
    zaaktypes.value = await zaaktypeService.getAllZaaktypes();
  } catch (err: unknown) {
    error.value = `Fout bij ophalen zaaktypes: ${err}`;
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  search.value = String(route.query.search || "");

  fetchZaaktypes();
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
