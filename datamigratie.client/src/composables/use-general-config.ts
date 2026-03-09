import { ref } from "vue";
import { get } from "@/utils/fetchWrapper";
import type { Mapping } from "@/components/MappingGrid.vue";

export function useGeneralConfig() {
  const loading = ref(false);
  const isGeneralConfigComplete = ref(false);

  async function checkGeneralConfig() {
    loading.value = true;
    try {
      // checking rsin and if all DET documentstatussen have mappings
      const [rsin, documentstatus] = await Promise.all(
        ["rsin", "documentstatus"].map((prop) => get<Mapping[]>(`/api/mappings/properties/${prop}`))
      );
      isGeneralConfigComplete.value = !!rsin[0]?.targetId && !!documentstatus.length;
    } catch (error) {
      console.error("Failed to check general configuration:", error);
    } finally {
      loading.value = false;
    }
  }

  return {
    isGeneralConfigComplete,
    loading,
    checkGeneralConfig
  };
}
