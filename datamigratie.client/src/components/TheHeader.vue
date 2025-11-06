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

<style lang="scss" scoped>
  header {
    nav ul {
      display: flex;
      align-items: center;
      gap: var(--spacing-default);
      list-style: none;
      padding: 0;
      margin: 0;

      .heading-1 {
        margin-block: 0;
        margin-right: auto;
      }

      li {
        display: flex;
        align-items: center;
        margin: 0;
      }

      a {
        margin: 0;
        padding: 0.5rem 1rem;
      }
    }
  }
</style>
