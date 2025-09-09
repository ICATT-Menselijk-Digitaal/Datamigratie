<template>
  <h1>Zaaktype</h1>

  <p>{{ functioneleIdentificatie }}</p>

  <p>
    <router-link :to="{ name: 'home', query: { search: route.query.search } }" class="button"
      >&lt; Terug</router-link
    >
  </p>
</template>

<script setup lang="ts">
import { useRoute } from "vue-router";

defineProps<{ functioneleIdentificatie: string }>();

const route = useRoute();
</script>

<style lang="scss" scoped></style>
