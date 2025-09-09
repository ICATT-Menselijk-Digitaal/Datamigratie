<template>
  <h1>Zaaktype</h1>

  <p>{{ functioneleIdentificatie }}</p>

  <p>
    <router-link :to="{ name: 'home', ...(search && { query: { search } }) }" class="button"
      >&lt; Terug</router-link
    >
  </p>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useRoute } from "vue-router";

defineProps<{ functioneleIdentificatie: string }>();

const route = useRoute();

const search = computed(() => String(route.query.search || "").trim());
</script>

<style lang="scss" scoped></style>
