import { ref, computed, type Ref } from "vue";
import toast from "@/components/toast/toast";
import type { DetStatus } from "@/services/detService";
import { datamigratieService, type StatusMappingItem } from "@/services/datamigratieService";

export function useStatusMappings(
  mappingId: Ref<string>,
  detZaaktype: Ref<{ statuses?: DetStatus[] } | undefined>,
  ozZaaktypeId: Ref<string>
) {
  const mappings = ref<StatusMappingItem[]>([]);
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
        ? await datamigratieService.getStatusMappings(mappingId.value)
        : [];
      const detStatuses = detZaaktype.value?.statuses || [];

      mappings.value = detStatuses.map((detStatus) => {
        const existingMapping = existingMappings.find((m) => m.detStatusNaam === detStatus.naam);
        return {
          detStatusNaam: detStatus.naam,
          ozStatustypeId: existingMapping?.ozStatustypeId || null
        };
      });

      isComplete.value =
        mappings.value.length > 0 && mappings.value.every((m) => m.ozStatustypeId !== null);
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
      const mappingsToSave = mappings.value.filter((m) => m.ozStatustypeId !== null);

      await datamigratieService.saveStatusMappings(mappingId.value, {
        mappings: mappingsToSave
      });

      toast.add({ text: "De status mappings zijn succesvol opgeslagen." });

      isComplete.value =
        mappings.value.length > 0 && mappings.value.every((m) => m.ozStatustypeId !== null);

      setEditing(false);
    } catch (error) {
      toast.add({ text: `Fout bij opslaan van de status mappings - ${error}`, type: "error" });
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
