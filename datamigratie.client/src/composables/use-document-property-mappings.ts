import { ref, computed, type Ref } from "vue";
import toast from "@/components/toast/toast";
import type { DetDocumenttype } from "@/services/detService";
import { datamigratieService, type DocumentPropertyMappingItem } from "@/services/datamigratieService";

export function useDocumentPropertyMappings(
  mappingId: Ref<string>,
  detZaaktype: Ref<{ documenttypen?: DetDocumenttype[] } | undefined>,
  ozZaaktypeId: Ref<string>
) {
  const mappings = ref<DocumentPropertyMappingItem[]>([]);
  const detDocumenttypen = ref<DetDocumenttype[]>([]);
  const isComplete = ref(false);
  const isEditingPublicatieniveau = ref(true);
  const isEditingDocumenttype = ref(true);
  const isLoading = ref(false);
  const isFetching = ref(false);

  const canEdit = computed(() => 
    isComplete.value && 
    (!isEditingPublicatieniveau.value || !isEditingDocumenttype.value)
  );

  const setEditingPublicatieniveau = (value: boolean) => {
    isEditingPublicatieniveau.value = value;
  };

  const setEditingDocumenttype = (value: boolean) => {
    isEditingDocumenttype.value = value;
  };

  const fetchMappings = async () => {
    if (!ozZaaktypeId.value) {
      detDocumenttypen.value = [];
      mappings.value = [];
      isComplete.value = false;
    isFetching.value = false;
    return;
    }

    if (isFetching.value) {
      return;
    }
    isFetching.value = true;
    isLoading.value = true;

    try {
      const detDocumenttypenData = detZaaktype.value?.documenttypen || [];
      const publicatieNiveauValuesData = await datamigratieService.getPublicatieNiveauOptions();

      const existingMappings = mappingId.value 
        ? await datamigratieService.getDocumentPropertyMappings(mappingId.value)
        : [];

      const publicatieNiveauMappings = publicatieNiveauValuesData.map((val: string) => {
        const existingMapping = existingMappings.find(
          m => m.detPropertyName === "publicatieniveau" && m.detValue === val
        );
        return existingMapping || {
          detPropertyName: "publicatieniveau",
          detValue: val,
          ozValue: null
        };
      });

      const documenttypeMappings = detDocumenttypenData.map(dt => {
        const existingMapping = existingMappings.find(
          m => m.detPropertyName === "documenttype" && m.detValue === dt.naam
        );
        return existingMapping || {
          detPropertyName: "documenttype",
          detValue: dt.naam,
          ozValue: null
        };
      });

      const publicatieNiveauMapped = publicatieNiveauMappings.every(m => m.ozValue !== null);
      const documenttypeMapped = documenttypeMappings.length === 0 || 
        documenttypeMappings.every(m => m.ozValue !== null);

      const savedPublicatieNiveauMappings = existingMappings.filter(
        m => m.detPropertyName === "publicatieniveau"
      );
      const savedDocumenttypeMappings = existingMappings.filter(
        m => m.detPropertyName === "documenttype"
      );

      const shouldEditPublicatieniveau = !(publicatieNiveauMapped && savedPublicatieNiveauMappings.length > 0);
      const shouldEditDocumenttype = !(documenttypeMapped && savedDocumenttypeMappings.length > 0);

      detDocumenttypen.value = detDocumenttypenData;
      mappings.value = [...publicatieNiveauMappings, ...documenttypeMappings];
      isComplete.value = publicatieNiveauMapped && documenttypeMapped;

      if (isEditingPublicatieniveau.value !== shouldEditPublicatieniveau) {
        setEditingPublicatieniveau(shouldEditPublicatieniveau);
      }
      if (isEditingDocumenttype.value !== shouldEditDocumenttype) {
        setEditingDocumenttype(shouldEditDocumenttype);
      }
    } catch (error) {
      mappings.value = [];
      isComplete.value = false;
      throw error;
    } finally {
      isLoading.value = false;
      isFetching.value = false;
    }
  };

  const saveMappings = async () => {
    isLoading.value = true;
    try {
      const mappingsToSave = mappings.value.filter(m => m.ozValue !== null && m.ozValue !== "");

      await datamigratieService.saveDocumentPropertyMappings(mappingId.value, {
        mappings: mappingsToSave
      });

      toast.add({ text: "De documentproperty mappings zijn succesvol opgeslagen." });

      await fetchMappings();
    } catch (error) {
      toast.add({ text: `Fout bij opslaan van de documentproperty mappings - ${error}`, type: "error" });
      throw error;
    } finally {
      isLoading.value = false;
    }
  };

  return {
    mappings,
    detDocumenttypen,
    isComplete,
    isEditingPublicatieniveau,
    isEditingDocumenttype,
    isLoading,
    canEdit,
    setEditingPublicatieniveau,
    setEditingDocumenttype,
    fetchMappings,
    saveMappings
  };
}
