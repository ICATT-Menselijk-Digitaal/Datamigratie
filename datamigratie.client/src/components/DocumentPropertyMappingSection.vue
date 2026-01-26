<template>
  <div v-if="showMapping" class="document-property-mapping-section">
    <mapping-grid
      v-model="publicatieNiveauMappingsModel"
      title="Document publicatieniveau mapping"
      description="Koppel de e-Suite publicatieniveaus voor documenten aan de Open Zaak vertrouwelijkheidaanduiding."
      source-label="e-Suite Publicatieniveau"
      target-label="Open Zaak Vertrouwelijkheidaanduiding"
      :source-items="publicatieNiveauSourceItems"
      :target-items="vertrouwelijkheidaanduidingTargetItems"
      :all-mapped="allPublicatieNiveauMapped"
      :is-editing="isEditingPublicatieniveau"
      :disabled="disabled"
      :loading="loading"
      empty-message="Er zijn geen publicatieniveaus beschikbaar."
      target-placeholder="Kies een vertrouwelijkheidaanduiding"
      save-button-text="Publicatieniveau mappings opslaan"
      edit-button-text="Publicatieniveau mappings aanpassen"
      :show-edit-button="true"
      :show-warning="showWarning && !allPublicatieNiveauMapped"
      warning-message="Niet alle publicatieniveaus zijn gekoppeld."
      @save="handleSavePublicatieNiveau"
      @edit="handleEditPublicatieNiveau"
    />

    <div class="documenttype-section">
      <!-- only shown when feature flag is enabled -->
      <div v-if="featureFlags.showDocumenttypeTestHelper && isEditingDocumenttype && documenttypeSourceItems.length > 0" class="test-helper">
        <label>
          <input type="checkbox" @change="fillRandomDocumenttypeMappings($event)" />
          <span style="color: #e74c3c; font-weight: bold;">for testing: check to autofill with random selections</span>
        </label>
      </div>

      <mapping-grid
        v-model="documenttypeMappingsModel"
        title="Documenttype mapping"
        description="Koppel de e-Suite documenttypes aan de Open Zaak informatieobjecttypes."
        source-label="e-Suite Documenttype"
        target-label="Open Zaak Informatieobjecttype"
        :source-items="documenttypeSourceItems"
        :target-items="informatieobjecttypeTargetItems"
        :all-mapped="allDocumenttypeMapped"
        :is-editing="isEditingDocumenttype"
        :disabled="disabled"
        :loading="loading"
        empty-message="Er zijn geen documenttypes beschikbaar."
        target-placeholder="Kies een informatieobjecttype"
        save-button-text="Documenttype mappings opslaan"
        edit-button-text="Documenttype mappings aanpassen"
        :show-edit-button="true"
        :show-warning="showWarning && !allDocumenttypeMapped"
        warning-message="Niet alle documenttypes zijn gekoppeld."
        @save="handleSaveDocumenttype"
        @edit="handleEditDocumenttype"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from "vue";
import MappingGrid, { type MappingItem, type Mapping } from "@/components/MappingGrid.vue";
import type { DetDocumenttype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";
import { datamigratieService, type DocumentPropertyMappingItem } from "@/services/datamigratieService";
import { featureFlags } from "@/config/featureFlags";

interface Props {
  detDocumenttypen?: DetDocumenttype[];
  ozZaaktype?: OZZaaktype;
  documentPropertyMappings: DocumentPropertyMappingItem[];
  isEditingPublicatieniveau: boolean;
  isEditingDocumenttype: boolean;
  disabled?: boolean;
  loading?: boolean;
  showWarning?: boolean;
  showMapping: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false,
  loading: false,
  showWarning: true
});

const emit = defineEmits<{
  (e: "update:documentPropertyMappings", value: DocumentPropertyMappingItem[]): void;
  (e: "save"): void;
  (e: "editPublicatieniveau"): void;
  (e: "editDocumenttype"): void;
}>();

const publicatieNiveauOptions = ref<string[]>([]);
const vertrouwelijkheidaanduidingOptions = ref<{ value: string; label: string }[]>([]);

onMounted(async () => {
  try {
    publicatieNiveauOptions.value = await datamigratieService.getPublicatieNiveauOptions();
    vertrouwelijkheidaanduidingOptions.value = await datamigratieService.getVertrouwelijkheidaanduidingOptions();
  } catch (error) {
    console.error("Error fetching document property options:", error);
  }
});

const publicatieNiveauSourceItems = computed<MappingItem[]>(() => {
  return publicatieNiveauOptions.value.map(option => ({
    id: option,
    name: option.charAt(0).toUpperCase() + option.slice(1),
    description: undefined
  }));
});

const vertrouwelijkheidaanduidingTargetItems = computed<MappingItem[]>(() => {
  return vertrouwelijkheidaanduidingOptions.value.map(option => ({
    id: option.value,
    name: option.label,
    description: undefined
  }));
});

const documenttypeSourceItems = computed<MappingItem[]>(() => {
  if (!props.detDocumenttypen) return [];
  return props.detDocumenttypen
    .filter(dt => dt.actief)
    .map(dt => ({
      id: dt.naam,
      name: dt.naam,
      description: dt.omschrijving
    }));
});

const informatieobjecttypeTargetItems = computed<MappingItem[]>(() => {
  if (!props.ozZaaktype?.informatieobjecttypen) return [];
  return props.ozZaaktype.informatieobjecttypen.map(iot => ({
    id: iot.url,
    name: iot.omschrijving,
    description: undefined
  }));
});

const publicatieNiveauMappings = computed(() =>
  props.documentPropertyMappings.filter(m => m.detPropertyName === "publicatieniveau")
);

const documenttypeMappings = computed(() =>
  props.documentPropertyMappings.filter(m => m.detPropertyName === "documenttype")
);

// check if all are mapped
const allPublicatieNiveauMapped = computed(() => {
  const mappedValues = new Set(publicatieNiveauMappings.value.map(m => m.detValue));
  return publicatieNiveauSourceItems.value.every(item => 
    mappedValues.has(item.id) && 
    publicatieNiveauMappings.value.find(m => m.detValue === item.id)?.ozValue !== null
  );
});

const allDocumenttypeMapped = computed(() => {
  const mappedValues = new Set(documenttypeMappings.value.map(m => m.detValue));
  return documenttypeSourceItems.value.every(item =>
    mappedValues.has(item.id) &&
    documenttypeMappings.value.find(m => m.detValue === item.id)?.ozValue !== null
  );
});

// mapping for MappingGrid
const publicatieNiveauMappingsModel = computed<Mapping[]>({
  get: () => {
    return publicatieNiveauMappings.value.map(m => ({
      sourceId: m.detValue,
      targetId: m.ozValue
    }));
  },
  set: (newMappings: Mapping[]) => {
    const updatedPublicatieNiveau = newMappings.map(m => ({
      detPropertyName: "publicatieniveau",
      detValue: m.sourceId,
      ozValue: m.targetId
    }));
    
    const allMappings = [...updatedPublicatieNiveau, ...documenttypeMappings.value];
    
    // emit if data actually changed
    const hasChanged = JSON.stringify(allMappings) !== JSON.stringify(props.documentPropertyMappings);
    if (hasChanged) {
      emit("update:documentPropertyMappings", allMappings);
    }
  }
});

const documenttypeMappingsModel = computed<Mapping[]>({
  get: () => {
    return documenttypeMappings.value.map(m => ({
      sourceId: m.detValue,
      targetId: m.ozValue
    }));
  },
  set: (newMappings: Mapping[]) => {
    const updatedDocumenttypes = newMappings.map(m => ({
      detPropertyName: "documenttype",
      detValue: m.sourceId,
      ozValue: m.targetId
    }));
    
    const allMappings = [...publicatieNiveauMappings.value, ...updatedDocumenttypes];
    
    const hasChanged = JSON.stringify(allMappings) !== JSON.stringify(props.documentPropertyMappings);
    if (hasChanged) {
      emit("update:documentPropertyMappings", allMappings);
    }
  }
});

const handleSavePublicatieNiveau = () => {
  emit("save");
};

const handleSaveDocumenttype = () => {
  emit("save");
};

const handleEditPublicatieNiveau = () => {
  emit("editPublicatieniveau");
};

const handleEditDocumenttype = () => {
  emit("editDocumenttype");
};

// fills documenttype mappings with random selections (when VITE_ENABLE_TEST_HELPERS=true)
const fillRandomDocumenttypeMappings = (event: Event) => {
  const checkbox = event.target as HTMLInputElement;
  
  if (checkbox.checked && informatieobjecttypeTargetItems.value.length > 0) {
    const randomMappings = documenttypeSourceItems.value.map(sourceItem => {
      const randomIndex = Math.floor(Math.random() * informatieobjecttypeTargetItems.value.length);
      const randomTarget = informatieobjecttypeTargetItems.value[randomIndex];
      
      return {
        sourceId: sourceItem.id,
        targetId: randomTarget.id
      };
    });
    
    documenttypeMappingsModel.value = randomMappings;
  } else {
    const clearedMappings = documenttypeSourceItems.value.map(sourceItem => ({
      sourceId: sourceItem.id,
      targetId: null
    }));
    
    documenttypeMappingsModel.value = clearedMappings;
  }
};
</script>

<style lang="scss" scoped>
.document-property-mapping-section {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-large);
}

.documenttype-section {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-default);
}

.test-helper {
  padding: var(--spacing-default);
  background-color: var(--marked);
  border: 2px dashed var(--accent);
  border-radius: var(--radius-default);
  
  label {
    display: flex;
    align-items: center;
    gap: var(--spacing-small);
    cursor: pointer;
    margin: 0;
    
    input[type="checkbox"] {
      cursor: pointer;
    }
    
    span {
      color: var(--code);
      font-weight: var(--font-bold);
    }
  }
}
</style>
