<template>
  <h1>Zaaktype</h1>

  <p>{{ functioneleIdentificatie }}</p>

  <p>
    <router-link :to="{ name: 'home' }" class="button">&lt; Terug</router-link>
  </p>
</template>

<script setup lang="ts">
// import type { Zaaktype } from "@/types/zaaktype";

defineProps<{ functioneleIdentificatie?: string }>();
</script>

<style lang="scss" scoped></style>
