<template>
  <section class="notice" :class="type && `notice--${type}`" aria-live="polite" role="alert">
    <slot></slot>
  </section>
</template>

<script setup lang="ts">
defineProps<{ type?: "warning" | "info" }>();
</script>

<style lang="scss" scoped></style>
