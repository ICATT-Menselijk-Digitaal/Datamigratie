<template>
  <form @submit.prevent="submit">
    <details class="mapping-section mapping-section--collapsible" :open="initiallyExpanded">
      <summary class="mapping-header-collapsible">
        <span>{{ title }}</span>
        <img
          v-if="!isComplete"
          src="@/assets/bi-exclamation-circle-fill.svg"
          alt="Niet compleet"
          class="warning-icon"
        />
      </summary>
      <p v-if="description && sourceItems.length">{{ description }}</p>
      <slot name="extra-content"></slot>
      <simple-spinner v-if="loading" />
      <div v-else-if="sourceItems.length === 0">
        <p>{{ emptyMessage }}</p>
      </div>
      <template v-else>
        <div class="mapping-grid">
          <div class="mapping-header">
            <div>{{ sourceLabel }}</div>
            <div>{{ targetLabel }}</div>
          </div>
          <div
            v-for="{ sourceItem, targetItem } in mappings"
            :key="sourceItem.id"
            class="mapping-row"
          >
            <div class="source-item">
              <strong>{{ sourceItem.name }}</strong>
              <span v-if="sourceItem.description" class="item-description">{{
                sourceItem.description
              }}</span>
            </div>
            <div class="target-item">
              <select v-if="isEditing" :name="sourceItem.id" :aria-label="sourceItem.name">
                <option value="" :selected="!targetItem">{{ targetPlaceholder }}</option>
                <option
                  v-for="item in targetItems"
                  :key="item.id"
                  :value="item.id"
                  :selected="item === targetItem"
                >
                  {{ item.name }}
                </option>
              </select>
              <div v-else class="target-value">
                <template v-if="targetItem">
                  {{ targetItem.name }}
                </template>
                <template v-else>
                  <span>Geen</span>
                  <img src="@/assets/bi-exclamation-circle-fill.svg" alt="Niet gekoppeld" />
                </template>
              </div>
            </div>
          </div>
        </div>
        <div v-if="isEditing && !isDisabled" class="form-actions">
          <button type="submit">
            {{ saveButtonText }}
          </button>
          <button type="reset" class="secondary" @click="handleCancel">
            {{ cancelButtonText }}
          </button>
        </div>
        <div v-if="!isEditing && !isDisabled" class="form-actions">
          <button type="button" class="secondary" @click="handleEdit">
            {{ editButtonText }}
          </button>
        </div>
      </template>
    </details>
  </form>
</template>

<script setup lang="ts">
import { computed, onWatcherCleanup, ref, watch, watchEffect } from "vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import { get, put } from "@/utils/fetchWrapper";
import toast from "./toast/toast";
import { useMigration } from "@/composables/migration-store";

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
  detZaaktypeId?: string;
  mappingId?: string;
  sourceItems: MappingItem[];
  targetItems: MappingItem[];
  mappingProperty: string;
  emptyMessage?: string;
  targetPlaceholder?: string;
  saveButtonText?: string;
  cancelButtonText?: string;
  editButtonText?: string;
  initiallyExpanded?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  emptyMessage: "Er zijn geen items beschikbaar voor dit zaaktype.",
  targetPlaceholder: "Kies een item",
  saveButtonText: "Mapping opslaan",
  cancelButtonText: "Annuleren",
  editButtonText: "Mapping aanpassen",
  initiallyExpanded: false
});

// for edit state management
const isEditing = ref(false);

const serverMappings = ref<Mapping[]>([]);
const loading = ref(true);
const error = ref("");

const serverUrl = computed(() =>
  ["/api", "mappings", "properties", props.mappingProperty, props.mappingId]
    .filter((x) => !!x)
    .join("/")
);

watchEffect(async () => {
  const controller = new AbortController();
  onWatcherCleanup(() => controller.abort());
  loading.value = true;
  error.value = "";
  try {
    serverMappings.value = await get(serverUrl.value);
  } catch {
    error.value = "fout bij ophalen mappings";
  } finally {
    loading.value = false;
  }
});

function submit(e: Event) {
  if (!(e.target instanceof HTMLFormElement)) return;
  const formData = new FormData(e.target);
  const newMappings = [...formData]
    .map(([sourceId, targetId]) => ({
      sourceId,
      targetId: targetId.toString()
    }))
    .filter((x) => !!x.targetId);
  const oldMappings = serverMappings.value;
  serverMappings.value = newMappings;
  isEditing.value = false;
  put(serverUrl.value, newMappings).catch(() => {
    serverMappings.value = oldMappings;
    toast.add({
      text: "er ging iets mis bij het opslaan van de wijzigingen. probeer het opnieuw",
      type: "error"
    });
    isEditing.value = true;
  });
}

const mappings = computed(() =>
  props.sourceItems.map((sourceItem) => {
    const targetMapping = serverMappings.value.find(({ sourceId }) => sourceId == sourceItem.id);
    const targetItem =
      targetMapping && props.targetItems.find(({ id }) => id === targetMapping.targetId);
    return {
      sourceItem,
      targetItem
    };
  })
);

const { migration } = useMigration();

const isComplete = computed(
  () => !loading.value && mappings.value.every(({ targetItem }) => targetItem)
);

const emit = defineEmits<{ (e: "update:complete", value: boolean): void }>();
watch(isComplete, (v) => emit("update:complete", v), { immediate: true });

const isDisabled = computed(
  () => !!migration.value?.detZaaktypeId && migration.value.detZaaktypeId === props.detZaaktypeId
);

const handleCancel = () => {
  isEditing.value = false;
};

const handleEdit = () => {
  isEditing.value = true;
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
