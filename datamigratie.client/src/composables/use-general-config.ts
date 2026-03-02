import { ref, computed } from "vue";
import { get } from "@/utils/fetchWrapper";
import type { RsinConfiguration, DocumentstatusMappingResponse } from "@/types/datamigratie";
import { detService } from "@/services/detService";

export function useGeneralConfig() {
  const rsin = ref<string | null>(null);
  const documentstatusMappingsComplete = ref(false);
  const loading = ref(false);

  const isGeneralConfigComplete = computed(() => {
    return !!rsin.value && documentstatusMappingsComplete.value;
  });

  async function checkGeneralConfig() {
    loading.value = true;
    try {
      // checking rsin and if all DET documentstatussen have mappings
      const rsinConfig = await get<RsinConfiguration>("/api/globalmapping/rsin");
      rsin.value = rsinConfig.rsin || null;

      const [detStatuses, savedMappings] = await Promise.all([
        detService.getAllDocumentstatussen(),
        get<DocumentstatusMappingResponse[]>("/api/globalmapping/documentstatuses")
      ]);

      documentstatusMappingsComplete.value =
        detStatuses.length > 0 &&
        detStatuses.every((status) =>
          savedMappings.some((m) => m.detDocumentstatus === status.naam && m.ozDocumentstatus)
        );
    } catch (error) {
      console.error("Failed to check general configuration:", error);
      rsin.value = null;
      documentstatusMappingsComplete.value = false;
    } finally {
      loading.value = false;
    }
  }

  return {
    rsin,
    documentstatusMappingsComplete,
    isGeneralConfigComplete,
    loading,
    checkGeneralConfig
  };
}
