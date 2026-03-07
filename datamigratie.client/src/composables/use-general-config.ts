import { ref, computed } from "vue";
import { get } from "@/utils/fetchWrapper";
import type { RsinConfiguration } from "@/types/datamigratie";

export function useGeneralConfig() {
  const rsin = ref<string | null>(null);
  const loading = ref(false);

  const isGeneralConfigComplete = computed(() => {
    return !!rsin.value;
  });

  async function checkGeneralConfig() {
    loading.value = true;
    try {
      // checking rsin and if all DET documentstatussen have mappings
      const rsinConfig = await get<RsinConfiguration>("/api/globalmapping/rsin");
      rsin.value = rsinConfig.rsin || null;
    } catch (error) {
      console.error("Failed to check general configuration:", error);
      rsin.value = null;
    } finally {
      loading.value = false;
    }
  }

  return {
    rsin,
    isGeneralConfigComplete,
    loading,
    checkGeneralConfig
  };
}
