<template>
  <form @submit.prevent="handleSave">
    <details class="mapping-section mapping-section--collapsible" :open="initiallyExpanded">
      <summary class="mapping-header-collapsible">
        <span>{{ title }}</span>
        <img
          v-if="showCollapseWarning"
          src="@/assets/bi-exclamation-circle-fill.svg"
          alt="Niet compleet"
          class="warning-icon"
        />
      </summary>
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
              v-if="isEditing"
              :value="getMappingForSource(sourceItem.id).targetId || ''"
              @change="updateMapping(sourceItem.id, ($event.target as HTMLSelectElement).value)"
              :disabled="disabled"
            >
              <option value="">{{ targetPlaceholder }}</option>
              <option v-for="targetItem in targetItems" :key="targetItem.id" :value="targetItem.id">
                {{ targetItem.name }}
              </option>
            </select>
            <div v-else class="target-value">
              <template v-if="getMappingForSource(sourceItem.id).targetId">
                {{ getTargetName(getMappingForSource(sourceItem.id).targetId) }}
              </template>
              <template v-else>
                <span>Geen</span>
                <img src="@/assets/bi-exclamation-circle-fill.svg" alt="Niet gekoppeld" />
              </template>
            </div>
          </div>
        </div>

        <alert-inline v-if="!allMapped && showWarning" type="warning">
          {{ warningMessage }}
        </alert-inline>
      </div>
      <div v-if="isEditing && !disabled" class="form-actions">
        <button type="submit">
          {{ saveButtonText }}
        </button>
        <button type="button" class="secondary" @click="handleCancel">
          {{ cancelButtonText }}
        </button>
      </div>
      <div v-if="showEditButton && !isEditing && !disabled" class="form-actions">
        <button type="button" class="secondary" @click="handleEdit">
          {{ editButtonText }}
        </button>
      </div>
    </details>
  </form>
</template>

<script setup lang="ts">
import { ref } from "vue";
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

// for edit state management
const isEditing = ref(false);

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
  isEditing.value = false;
};

const handleCancel = () => {
  isEditing.value = false;
  emit("cancel");
};

const handleEdit = () => {
  isEditing.value = true;
  emit("edit");
};
</script>

<style lang="scss" scoped>
@use "@/assets/variables";
summary {
  .warning-icon {
    width: 1em;
    height: 1em;
  }
}

.mapping-section {
  margin-block-end: var(--spacing-small);

  p {
    margin-block: var(--spacing-small);
  }

  .mapping-grid {
    display: grid;
    column-gap: var(--spacing-large);
    grid-template-columns: 1fr 1fr;
    margin-block-start: var(--spacing-default);

    @media (max-width: variables.$breakpoint-md) {
      grid-template-columns: 1fr;
      gap: var(--spacing-default);
    }
  }

  .mapping-header,
  .mapping-row {
    display: grid;
    grid-template-columns: subgrid;
    grid-column: 1 / -1;
    align-items: center;
    padding: var(--spacing-extrasmall);

    &:nth-child(even) {
      background: var(--accent-bg);
    }

    @media (max-width: variables.$breakpoint-md) {
      gap: var(--spacing-extrasmall);
    }
  }

  .mapping-header {
    font-weight: 600;

    font-size: var(--font-large);
    white-space: nowrap;

    @media (max-width: variables.$breakpoint-md) {
      display: none;
    }
  }

  .source-item {
    display: flex;
    flex-direction: column;
    gap: var(--spacing-extrasmall);

    .item-description {
      font-size: var(--font-small);
    }
  }
}

// same padding as select
.target-value,
.mapping-header > :nth-child(2) {
  padding: var(--input-padding);
  border: 1px transparent solid;
  display: flex;
  align-items: center;
  gap: 0.5ch;
}

.form-actions {
  padding-block: var(--spacing-small);
  position: sticky;
  bottom: 0;
  --transparent-bg: rgb(from var(--bg) r g b / 0);
  background-image: linear-gradient(to top, var(--bg) 0%, var(--bg) 66%, var(--transparent-bg));
}
// Button styles are defined in main.scss
</style>
