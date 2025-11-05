<template>
  <header v-if="user.isLoggedIn">
    <nav>
      <ul>
        <template v-if="user.hasFunctioneelBeheerderAccess">
          <span class="heading-1">Datamigratie</span>
        </template>
        <li class="user-name">
          {{ user.name }}
        </li>
        <li>
          <a href="/api/logoff">Uitloggen</a>
        </li>
      </ul>
    </nav>
  </header>
</template>

<script lang="ts" setup>
  import { user } from "@/composables/use-user";
</script>

<style lang="scss" scoped></style>
