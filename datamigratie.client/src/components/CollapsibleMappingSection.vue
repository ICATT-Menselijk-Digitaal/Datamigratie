<template>
  <section class="collapsible-mapping-section">
    <button
      type="button"
      class="section-header"
      @click="toggleExpanded"
      :aria-expanded="isExpanded"
    >
      <h2>{{ title }}</h2>
      <img
        v-if="showWarning"
        src="@/assets/bi-exclamation-circle-fill.svg"
        alt="Niet compleet"
        class="warning-icon"
      />
      <img
        src="@/assets/arrow-drop-down.svg"
        alt="Toggle"
        class="toggle-icon"
        :class="{ rotated: isExpanded }"
      />
    </button>

    <div v-show="isExpanded" class="section-content">
      <p v-if="description" class="section-description">{{ description }}</p>
      <slot></slot>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from "vue";

interface Props {
  title: string;
  description?: string;
  showWarning?: boolean;
  initiallyExpanded?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  description: "",
  showWarning: false,
  initiallyExpanded: true
});

const isExpanded = ref(props.initiallyExpanded);

const toggleExpanded = () => {
  isExpanded.value = !isExpanded.value;
};
</script>

<style scoped lang="scss">
.collapsible-mapping-section {
  display: flex;
  min-width: 300px;
  padding: 12px;
  flex-direction: column;
  justify-content: center;
  align-items: flex-start;
  align-self: stretch;
  border-radius: 5px;
  border: 1px solid var(--Border, #898ea4);
  background: #fff;
  margin-block-end: 8px;

  .section-header {
    display: flex;
    align-items: center;
    width: 100%;
    padding: 0;
    border: none;
    background: transparent;
    cursor: pointer;
    text-align: left;
    gap: 8px;
    margin-bottom: 2px;

    &:hover {
      opacity: 0.8;
    }

    h2 {
      margin: 0;
      color: var(--Zwart, #212121);
      /* Lopende tekst bold */
      font-family: Avenir;
      font-size: 16px;
      font-style: normal;
      font-weight: 800;
      line-height: 20px; /* 125% */
    }

    .warning-icon {
      width: 16px;
      height: 16px;
    }

    .toggle-icon {
      width: 24px;
      height: 24px;
      margin-left: auto;
      transition: transform 0.3s ease;

      &.rotated {
        transform: rotate(180deg);
      }
    }
  }

  .section-description {
    align-self: stretch;
    margin: 0 0 12px 0;
    color: var(--Zwart, #212121);
    /* Lopende tekst */
    font-family: Avenir;
    font-size: 16px;
    font-style: normal;
    font-weight: 400;
    line-height: 20px; /* 125% */
  }

  .section-content {
    width: 100%;

    // overriding MappingGrid styles to fit within collapsible section
    :deep(.mapping-section) {
      margin-block-end: 0;

      h2 {
        display: none;
      }

      p {
        display: none;
      }

      .mapping-row {
        border: none;
        border-radius: 0;
      }

      .mapping-header {
        background-color: transparent;
        border-radius: 0;
        padding: 8px;
      }
    }
  }
}
</style>
