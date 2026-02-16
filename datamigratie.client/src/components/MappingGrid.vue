<template>
  <form @submit.prevent="handleSave">
    <details class="mapping-section mapping-section--collapsible" :open="initiallyExpanded">
      <summary class="mapping-header-collapsible">
        <h2>{{ title }}</h2>
        <img
          v-if="showCollapseWarning"
          src="@/assets/bi-exclamation-circle-fill.svg"
          alt="Niet compleet"
          class="warning-icon"
        />
        <img src="@/assets/arrow-drop-down.svg" alt="Toggle" class="toggle-icon" />
      </summary>
      <div class="mapping-content">
        <p v-if="description">{{ description }}</p>
        <slot name="extra-content"></slot>
        <simple-spinner v-if="loading" />
        <div v-else-if="sourceItems.length === 0">
          <p>{{ emptyMessage }}</p>
        </div>
        <div v-else class="mapping-grid">
          <div class="mapping-header">
            <div>{{ sourceLabel }}</div>
            <div>{{ targetLabel }}</div>
          </div>
          <div v-for="sourceItem in sourceItems" :key="sourceItem.id" class="mapping-row">
            <div class="source-item">
              <strong>{{ sourceItem.name }}</strong>
              <span v-if="sourceItem.description" class="item-description">{{
                sourceItem.description
              }}</span>
            </div>
            <div class="target-item">
              <select
                v-if="isEditing || !allMapped"
                :value="getMappingForSource(sourceItem.id).targetId || ''"
                @change="updateMapping(sourceItem.id, ($event.target as HTMLSelectElement).value)"
                :disabled="disabled"
              >
                <option value="">{{ targetPlaceholder }}</option>
                <option
                  v-for="targetItem in targetItems"
                  :key="targetItem.id"
                  :value="targetItem.id"
                >
                  {{ targetItem.name }}
                </option>
              </select>
              <div v-else class="target-value">
                {{ getTargetName(getMappingForSource(sourceItem.id).targetId) }}
              </div>
            </div>
          </div>
          <div v-if="(!allMapped || isEditing) && !disabled" class="mapping-actions">
            <button type="submit" class="primary-button">
              {{ saveButtonText }}
            </button>
            <button type="button" class="cancel-button" @click="handleCancel">
              {{ cancelButtonText }}
            </button>
          </div>
          <div
            v-if="showEditButton && allMapped && !isEditing && !disabled"
            class="mapping-actions"
          >
            <button type="button" class="secondary" @click="handleEdit">
              {{ editButtonText }}
            </button>
          </div>
          <alert-inline v-if="!allMapped && showWarning" type="warning">
            {{ warningMessage }}
          </alert-inline>
        </div>
      </div>
    </details>
  </form>
</template>

<script setup lang="ts">
import AlertInline from "@/components/AlertInline.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";

export interface MappingItem {
  id: string;
  name: string;
  description?: string;
}

export interface Mapping {
  sourceId: string;
  targetId: string | null;
}

interface Props {
  title: string;
  description: string;
  sourceLabel: string;
  targetLabel: string;
  sourceItems: MappingItem[];
  targetItems: MappingItem[];
  modelValue: Mapping[];
  allMapped: boolean;
  isEditing: boolean;
  disabled?: boolean;
  loading?: boolean;
  emptyMessage?: string;
  targetPlaceholder?: string;
  saveButtonText?: string;
  cancelButtonText?: string;
  editButtonText?: string;
  showEditButton?: boolean;
  showWarning?: boolean;
  warningMessage?: string;
  initiallyExpanded?: boolean;
  showCollapseWarning?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false,
  loading: false,
  emptyMessage: "Er zijn geen items beschikbaar voor dit zaaktype.",
  targetPlaceholder: "Kies een item",
  saveButtonText: "Mappings opslaan",
  cancelButtonText: "Annuleren",
  editButtonText: "Mappings aanpassen",
  showEditButton: false,
  showWarning: true,
  warningMessage: "Niet alle items zijn gekoppeld. Migratie kan niet worden gestart.",
  initiallyExpanded: false,
  showCollapseWarning: false
});

const emit = defineEmits<{
  (e: "update:modelValue", value: Mapping[]): void;
  (e: "save"): void;
  (e: "cancel"): void;
  (e: "edit"): void;
}>();

const getMappingForSource = (sourceId: string): Mapping => {
  const existing = props.modelValue.find((m) => m.sourceId === sourceId);
  if (existing) return existing;

  // Create a new mapping and add it to the array
  const newMapping: Mapping = { sourceId, targetId: null };
  emit("update:modelValue", [...props.modelValue, newMapping]);
  return newMapping;
};

const getTargetName = (targetId: string | null | undefined): string => {
  if (!targetId) return "";
  const targetItem = props.targetItems.find((item) => item.id === targetId);
  return targetItem?.name || "";
};

const updateMapping = (sourceId: string, targetId: string) => {
  // Convert empty string to null
  const normalizedTargetId = targetId === "" || targetId === "null" ? null : targetId;

  // Find existing mapping or create new one
  const existingIndex = props.modelValue.findIndex((m) => m.sourceId === sourceId);

  if (existingIndex >= 0) {
    // Update existing mapping
    const updatedMappings = [...props.modelValue];
    updatedMappings[existingIndex] = {
      ...updatedMappings[existingIndex],
      targetId: normalizedTargetId
    };
    emit("update:modelValue", updatedMappings);
  } else {
    // Create new mapping
    const newMapping: Mapping = { sourceId, targetId: normalizedTargetId };
    emit("update:modelValue", [...props.modelValue, newMapping]);
  }
};

const handleSave = () => {
  emit("save");
};

const handleCancel = () => {
  emit("cancel");
};

const handleEdit = () => {
  emit("edit");
};
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

.mapping-section {
  margin-block-end: var(--spacing-large);

  h2 {
    font-size: 1.5rem;
    margin-block-end: var(--spacing-small);
  }

  p {
    margin-block-end: var(--spacing-default);
  }

  &--collapsible {
    display: flex;
    padding: var(--spacing-default);
    flex-direction: column;
    align-items: flex-start;
    align-self: stretch;
    border-radius: var(--standard-border-radius);
    border: 1px solid var(--border);
    background: var(--bg);
    margin-block-end: var(--spacing-small);

    h2 {
      margin: 0;
      color: var(--text);
      font-family: var(--sans-font);
      font-size: var(--font-medium);
      font-weight: 800;
      line-height: 1.25;
    }

    p {
      align-self: stretch;
      margin: 0 0 var(--spacing-default) 0;
      color: var(--text);
      font-family: var(--sans-font);
      font-size: var(--font-medium);
      font-weight: 400;
      line-height: 1.25;
    }

    &[open] .mapping-header-collapsible .toggle-icon {
      transform: rotate(180deg);
    }
  }

  .mapping-header-collapsible {
    display: flex;
    align-items: center;
    width: 100%;
    padding: var(--spacing-extrasmall) var(--spacing-small);
    cursor: pointer;
    text-align: left;
    gap: var(--spacing-small);
    margin-bottom: 0.125rem;
    list-style: none;

    &::-webkit-details-marker {
      display: none;
    }

    &::marker {
      display: none;
    }

    &:hover {
      opacity: 0.8;
    }

    .warning-icon {
      width: 1em;
      height: 1em;
    }

    .toggle-icon {
      width: 1.5em;
      height: 1.5em;
      margin-left: auto;
      transition: transform 0.3s ease;
    }
  }

  .mapping-content {
    width: 100%;
  }

  .mapping-grid {
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    gap: 0;
    align-self: stretch;
  }

  .mapping-header {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: var(--spacing-large);
    padding: var(--spacing-extrasmall);
    font-weight: 600;
    align-self: stretch;

    div {
      color: var(--text);
      font-family: var(--sans-font);
      font-size: var(--font-large);
      font-weight: 900;
      line-height: 1.25;
      white-space: nowrap;
    }

    @media (max-width: variables.$breakpoint-md) {
      display: none;
    }
  }

  .mapping-row {
    display: flex;
    padding: var(--spacing-extrasmall);
    align-items: center;
    gap: var(--spacing-large);
    align-self: stretch;
    min-height: 3.25rem;

    &:nth-child(even) {
      background: var(--accent-bg);
    }

    @media (min-width: variables.$breakpoint-md) {
      display: flex;
    }

    @media (max-width: variables.$breakpoint-md) {
      flex-direction: column;
      align-items: stretch;
      gap: var(--spacing-default);
      height: auto;
    }
  }

  .source-item {
    display: flex;
    flex: 1;
    flex-direction: column;
    justify-content: center;
    align-items: flex-start;
    gap: 0.125rem;

    strong {
      color: var(--text);
      font-family: var(--sans-font);
      font-size: var(--font-medium);
      font-weight: 800;
      line-height: 1.25;
    }

    .item-description {
      color: var(--text);
      font-family: var(--sans-font);
      font-size: var(--font-small);
      font-weight: 400;
      line-height: 1.4;
    }

    @media (max-width: variables.$breakpoint-md) {
      width: 100%;
    }
  }

  .target-item {
    select {
      display: flex;
      flex: 1;
      min-width: 15rem;
      padding: var(--spacing-default);

      @media (max-width: variables.$breakpoint-md) {
        width: 100%;
        min-width: auto;
      }
    }

    .target-value {
      display: flex;
      flex: 1;
      min-width: 15rem;
      padding: var(--spacing-default);
      align-items: center;
      color: var(--text);
      font-family: var(--sans-font);
      font-size: var(--font-medium);
      font-weight: 400;
      line-height: 1.25;

      @media (max-width: variables.$breakpoint-md) {
        width: 100%;
        min-width: auto;
      }
    }
  }

  .mapping-actions {
    display: flex;
    align-items: flex-start;
    gap: var(--spacing-small);
    margin-block-start: var(--spacing-default);
  }

  // Button styles are defined in main.scss
}
</style>
