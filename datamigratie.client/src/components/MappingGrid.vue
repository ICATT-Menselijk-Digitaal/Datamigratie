<template>
  <section class="mapping-section">
    <h2>{{ title }}</h2>
    <p>{{ description }}</p>

    <simple-spinner v-if="loading" />

    <div v-else-if="sourceItems.length === 0">
      <p>{{ emptyMessage }}</p>
    </div>

    <div v-else class="mapping-grid">
      <div class="mapping-header">
        <div>{{ sourceLabel }}</div>
        <div>{{ targetLabel }}</div>
      </div>

      <div
        v-for="sourceItem in sourceItems"
        :key="sourceItem.id"
        class="mapping-row"
      >
        <div class="source-item">
          <strong>{{ sourceItem.name }}</strong>
          <span v-if="sourceItem.description" class="item-description">{{ sourceItem.description }}</span>
        </div>

        <div class="target-item">
          <select
            :value="getMappingForSource(sourceItem.id).targetId || ''"
            @change="updateMapping(sourceItem.id, ($event.target as HTMLSelectElement).value)"
            :disabled="!isEditing && allMapped || disabled"
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
        </div>
      </div>
      <div v-if="(!allMapped || isEditing) && !disabled" class="mapping-actions">
        <button type="button" @click="handleSave">{{ saveButtonText }}</button>
      </div>

      <div v-if="showEditButton && allMapped && !isEditing && !disabled" class="mapping-actions">
        <button type="button" class="secondary" @click="handleEdit">{{ editButtonText }}</button>
      </div>

      <alert-inline v-if="!allMapped && showWarning" type="warning">
        {{ warningMessage }}
      </alert-inline>
    </div>
  </section>
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
  editButtonText?: string;
  showEditButton?: boolean;
  showWarning?: boolean;
  warningMessage?: string;
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false,
  loading: false,
  emptyMessage: "Er zijn geen items beschikbaar voor dit zaaktype.",
  targetPlaceholder: "Kies een item",
  saveButtonText: "Mappings opslaan",
  editButtonText: "Mappings aanpassen",
  showEditButton: false,
  showWarning: true,
  warningMessage: "Niet alle items zijn gekoppeld. Migratie kan niet worden gestart."
});

const emit = defineEmits<{
  (e: "update:modelValue", value: Mapping[]): void;
  (e: "save"): void;
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

const updateMapping = (sourceId: string, targetId: string) => {
  // Convert empty string to null
  const normalizedTargetId = targetId === '' || targetId === 'null' ? null : targetId;
  
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

  .mapping-grid {
    display: flex;
    flex-direction: column;
    gap: var(--spacing-default);
  }

  .mapping-header {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: var(--spacing-default);
    padding: var(--spacing-small);
    background-color: var(--background-secondary);
    font-weight: 600;
    border-radius: 4px;

    @media (max-width: variables.$breakpoint-md) {
      display: none;
    }
  }

  .mapping-row {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: var(--spacing-default);
    padding: var(--spacing-small);
    border: 1px solid var(--border);
    border-radius: 4px;
    align-items: center;

    @media (max-width: variables.$breakpoint-md) {
      grid-template-columns: 1fr;
    }
  }

  .source-item {
    display: flex;
    flex-direction: column;
    gap: var(--spacing-xs, 0.25rem);

    strong {
      font-size: 1rem;
    }

    .item-description {
      font-size: 0.875rem;
      color: var(--text-secondary, #666);
    }
  }

  .target-item {
    select {
      width: 100%;
      margin-block-end: 0;
    }
  }

  .mapping-actions {
    display: flex;
    justify-content: flex-end;
    margin-block-start: var(--spacing-default);
  }
}
</style>
