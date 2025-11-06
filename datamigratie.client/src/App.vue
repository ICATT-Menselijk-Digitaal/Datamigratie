<script setup lang="ts">
  import { RouterView, useRoute } from "vue-router";
  import ToastSection from "./components/toast/ToastSection.vue";
  import TheHeader from "./components/TheHeader.vue";
  import TheFooter from "./components/TheFooter.vue";
  import MigrationAlert from "@/components/MigrationAlert.vue";
  import { user } from "@/composables/use-user";
</script>



<template>
  <toast-section />

  <the-header />

  <main id="main">

    <migration-alert v-if="user.hasFunctioneelBeheerderAccess" />

    <router-view />
  </main>

  <the-footer />
</template>
