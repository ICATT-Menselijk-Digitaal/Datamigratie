<template>
  <h1>{{ functioneleIdentificatie }}</h1>

  <simple-spinner v-if="loading" />

  <alert-inline v-else-if="error">{{ error }}</alert-inline>

  <dl v-else>
    <dt>Aantal gesloten zaken:</dt>
    <dd>{{ zaaktype?.closedZaken }}</dd>
  </dl>

  <p v-if="!loading">
    <router-link :to="{ name: 'zaaktypes', ...(search && { query: { search } }) }" class="button"
      >&lt; Terug</router-link
    >
  </p>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import { zaaktypeService } from "@/services/zaaktypeService";
import type { Zaaktype } from "@/types/zaaktype";

const props = defineProps<{ functioneleIdentificatie: string }>();

const route = useRoute();
const search = computed(() => String(route.query.search || "").trim());

const zaaktype = ref<Zaaktype>();

const loading = ref(false);
const error = ref("");

const fetchZaaktypeByFunctioneleIdentificatie = async () => {
  loading.value = true;
  error.value = "";

  try {
    zaaktype.value = await zaaktypeService.getZaaktypeByFunctioneleIdentificatie(
      props.functioneleIdentificatie
    );
  } catch (err: unknown) {
    error.value = `Fout bij ophalen zaaktype: ${err}`;
  } finally {
    loading.value = false;
  }
};

onMounted(() => fetchZaaktypeByFunctioneleIdentificatie());
</script>

<style lang="scss" scoped>
dl {
  display: flex;
  column-gap: var(--spacing-default);

  dd {
    font-weight: 600;
    margin-inline: 0;
  }
}
</style>
