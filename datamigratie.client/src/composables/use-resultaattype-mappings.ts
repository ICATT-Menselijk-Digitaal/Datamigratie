import { ref, computed, type Ref } from "vue";
import toast from "@/components/toast/toast";
import type { DetResultaattypen } from "@/services/detService";
import { datamigratieService, type ResultaattypeMappingItem } from "@/services/datamigratieService";

export function useResultaattypeMappings(
  mappingId: Ref<string>,
  detZaaktype: Ref<{ resultaten?: DetResultaattypen[] } | undefined>,
  ozZaaktypeId: Ref<string>
) {
  const mappings = ref<ResultaattypeMappingItem[]>([]);
  const isComplete = ref(false);
  const isEditing = ref(false);
  const isLoading = ref(false);

  const canEdit = computed(() => isComplete.value && !isEditing.value);

  const setEditing = (value: boolean) => {
    isEditing.value = value;
  };

  const fetchMappings = async () => {
    if (!ozZaaktypeId.value) {
      mappings.value = [];
      isComplete.value = false;
      return;
    }

    isLoading.value = true;
    try {
      const existingMappings = mappingId.value 
        ? await datamigratieService.getResultaattypeMappings(mappingId.value)
        : [];
      
      const detResultaattypen = detZaaktype.value?.resultaten || [];

      mappings.value = detResultaattypen.map((detResultaattypen) => {
        const existingMapping = existingMappings.find((m) => m.detResultaattypeNaam === detResultaattypen.resultaat.naam);
        return existingMapping || {
          detResultaattypeNaam: detResultaattypen.resultaat.naam,
          ozResultaattypeId: null
        };
      });

      isComplete.value = mappings.value.length > 0 && 
        mappings.value.every((m) => m.ozResultaattypeId !== null);
    } catch (error) {
      mappings.value = [];
      isComplete.value = false;
      throw error;
    } finally {
      isLoading.value = false;
    }
  };

  const saveMappings = async () => {
    isLoading.value = true;
    try {
      const mappingsToSave = mappings.value.filter((m) => m.ozResultaattypeId !== null);

      await datamigratieService.saveResultaattypeMappings(mappingId.value, {
        mappings: mappingsToSave
      });

      toast.add({ text: "De resultaattype mappings zijn succesvol opgeslagen." });

      isComplete.value = mappings.value.length > 0 && 
        mappings.value.every((m) => m.ozResultaattypeId !== null);

      setEditing(false);
    } catch (error) {
      toast.add({ text: `Fout bij opslaan van de resultaattype mappings - ${error}`, type: "error" });
      throw error;
    } finally {
      isLoading.value = false;
    }
  };

  return {
    mappings,
    isComplete,
    isEditing,
    isLoading,
    canEdit,
    setEditing,
    fetchMappings,
    saveMappings
  };
}
