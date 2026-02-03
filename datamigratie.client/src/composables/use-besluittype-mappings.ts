import { ref, computed, type Ref } from "vue";
import toast from "@/components/toast/toast";
import type { DetBesluittype } from "@/services/detService";
import { datamigratieService, type BesluittypeMappingItem } from "@/services/datamigratieService";
import { detService } from "@/services/detService";

export function useBesluittypeMappings(mappingId: Ref<string>, ozZaaktypeId: Ref<string>) {
  const mappings = ref<BesluittypeMappingItem[]>([]);
  const detBesluittypen = ref<DetBesluittype[]>([]);
  const isComplete = ref(false);
  const isEditing = ref(false);
  const isLoading = ref(false);

  const canEdit = computed(() => isComplete.value && !isEditing.value);

  const setEditing = (value: boolean) => {
    isEditing.value = value;
  };

  const fetchMappings = async () => {
    if (!ozZaaktypeId.value) {
      detBesluittypen.value = [];
      mappings.value = [];
      isComplete.value = false;
      return;
    }

    isLoading.value = true;
    try {
      const detBesluittypeData = await detService.getAllBesluittypen();
      detBesluittypen.value = detBesluittypeData;

      const existingMappings = mappingId.value
        ? await datamigratieService.getBesluittypeMappings(mappingId.value)
        : [];

      mappings.value = detBesluittypeData.map((detBesluittype) => {
        const existingMapping = existingMappings.find(
          (m) => m.detBesluittypeNaam === detBesluittype.naam
        );
        return existingMapping
          ? {
              detBesluittypeNaam: existingMapping.detBesluittypeNaam,
              ozBesluittypeId: existingMapping.ozBesluittypeId
            }
          : {
              detBesluittypeNaam: detBesluittype.naam,
              ozBesluittypeId: null
            };
      });

      isComplete.value =
        mappings.value.length > 0 && mappings.value.every((m) => m.ozBesluittypeId !== null);
    } catch (error) {
      detBesluittypen.value = [];
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
      const mappingsToSave = mappings.value.filter((m) => m.ozBesluittypeId !== null);

      await datamigratieService.saveBesluittypeMappings(mappingId.value, {
        mappings: mappingsToSave
      });

      toast.add({ text: "De besluittype mappings zijn succesvol opgeslagen." });

      isComplete.value =
        mappings.value.length > 0 && mappings.value.every((m) => m.ozBesluittypeId !== null);

      setEditing(false);
    } catch (error) {
      toast.add({ text: `Fout bij opslaan van de besluittype mappings - ${error}`, type: "error" });
      throw error;
    } finally {
      isLoading.value = false;
    }
  };

  return {
    mappings,
    detBesluittypen,
    isComplete,
    isEditing,
    isLoading,
    canEdit,
    setEditing,
    fetchMappings,
    saveMappings
  };
}
