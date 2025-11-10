<template>
  <header v-if="user.isLoggedIn">
    <h1>Datamigratie</h1>
    <nav>
      <ul class="reset">
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

<style lang="scss" scoped>
ul {
  display: flex;
  align-items: center;
  gap: var(--spacing-default);
}

li,
a {
  margin: 0;
}

.user-name {
  margin-inline-start: auto;
}
</style>
