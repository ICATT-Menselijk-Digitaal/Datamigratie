import { ref, type Ref } from "vue";
import toast from "@/components/toast/toast";
import type { DETZaaktype } from "@/services/detService";
import type { OZZaaktype } from "@/services/ozService";
import {
  datamigratieService,
  type ZaaktypeMapping,
  type CreateZaaktypeMapping,
  type UpdateZaaktypeMapping,
  type MigrationHistoryItem
} from "@/services/datamigratieService";
import { detService } from "@/services/detService";
import { ozService } from "@/services/ozService";
import { knownErrorMessages } from "@/utils/fetchWrapper";

export function useZaaktypeMapping(detZaaktypeId: string) {
  const detZaaktype = ref<DETZaaktype>();
  const ozZaaktypes = ref<OZZaaktype[]>();
  const mapping = ref({ ozZaaktypeId: "" } as ZaaktypeMapping);
  const migrationHistory = ref<MigrationHistoryItem[]>([]);
  const previousOzZaaktypeId = ref<string>("");
  const isEditing = ref(false);
  const isLoading = ref(false);
  const errors = ref<unknown[]>([]);

  const setEditing = (value: boolean) => {
    isEditing.value = value;
  };

  /**
   * Fetches all initial data: DET zaaktype, OZ zaaktypes, existing mapping, and migration history
   */
  const fetchData = async () => {
    isLoading.value = true;
    errors.value = [];

    try {
      const services = [
        {
          service: detService.getZaaktypeById(detZaaktypeId),
          target: detZaaktype
        },
        {
          service: ozService.getAllZaaktypes(),
          target: ozZaaktypes
        },
        {
          service: datamigratieService.getMappingByDETZaaktypeId(detZaaktypeId),
          target: mapping,
          ignore404: true
        },
        {
          service: datamigratieService.getMigrationHistory(detZaaktypeId),
          target: migrationHistory,
          ignore404: false
        }
      ];

      const results = await Promise.allSettled(services.map((s) => s.service));

      results.forEach((result, index) => {
        const { target, ignore404 } = services[index];

        if (result.status === "fulfilled") {
          target.value = result.value;
        } else {
          const { reason } = result;

          if (
            ignore404 &&
            reason instanceof Error &&
            reason.message === knownErrorMessages.notFound
          ) {
            return;
          }

          errors.value.push(reason);
        }
      });
    } catch (err: unknown) {
      errors.value.push(err);
    } finally {
      isLoading.value = false;
    }

    // Store the current ozZaaktypeId for change detection
    if (mapping.value.ozZaaktypeId) {
      previousOzZaaktypeId.value = mapping.value.ozZaaktypeId;
    }
  };

  /**
   * Saves or updates the zaaktype mapping
   * @param onMappingChanged - Callback to execute when the OZ zaaktype is changed
   */
  const saveMapping = async (onMappingChanged?: () => Promise<void>) => {
    const hasZaaktypeChanged = mapping.value.ozZaaktypeId !== previousOzZaaktypeId.value;

    isLoading.value = true;
    setEditing(false);

    try {
      if (!mapping.value.detZaaktypeId) {
        // Create new mapping
        mapping.value = { ...mapping.value, detZaaktypeId };

        const createMapping: CreateZaaktypeMapping = {
          detZaaktypeId: mapping.value.detZaaktypeId,
          ozZaaktypeId: mapping.value.ozZaaktypeId
        };

        await datamigratieService.createMapping(createMapping);

        // Fetch the created mapping to get its ID
        const createdMapping = await datamigratieService.getMappingByDETZaaktypeId(detZaaktypeId);
        mapping.value = createdMapping;
      } else {
        // Update existing mapping
        const updatedMapping: UpdateZaaktypeMapping = {
          detZaaktypeId: mapping.value.detZaaktypeId,
          updatedOzZaaktypeId: mapping.value.ozZaaktypeId
        };

        await datamigratieService.updateMapping(updatedMapping);
      }

      previousOzZaaktypeId.value = mapping.value.ozZaaktypeId;

      toast.add({ text: "De mapping is succesvol opgeslagen." });

      // If the OZ zaaktype changed, trigger callback to refresh related mappings
      if (hasZaaktypeChanged && onMappingChanged) {
        await onMappingChanged();
      }
    } catch (err: unknown) {
      toast.add({ text: `Fout bij opslaan van de mapping - ${err}`, type: "error" });
      throw err;
    } finally {
      isLoading.value = false;
    }
  };

  /**
   * Checks if any child mappings exist
   * @param mappingChecks - Object with boolean checks for each mapping type
   * @returns true if any mappings exist
   */
  const hasExistingMappings = (mappingChecks: {
    hasStatusMappings: boolean;
    hasResultaattypeMappings: boolean;
    hasBesluittypeMappings: boolean;
    hasDocumentPropertyMappings: boolean;
  }): boolean => {
    return (
      mappingChecks.hasStatusMappings ||
      mappingChecks.hasResultaattypeMappings ||
      mappingChecks.hasBesluittypeMappings ||
      mappingChecks.hasDocumentPropertyMappings
    );
  };

  /**
   * Checks if the OZ zaaktype has changed
   */
  const hasZaaktypeChanged = (): boolean => {
    return mapping.value.ozZaaktypeId !== previousOzZaaktypeId.value;
  };

  /**
   * Reverts the mapping to the previous OZ zaaktype
   */
  const revertMapping = () => {
    mapping.value.ozZaaktypeId = previousOzZaaktypeId.value;
  };

  return {
    // State
    detZaaktype,
    ozZaaktypes,
    mapping,
    migrationHistory,
    isEditing,
    isLoading,
    errors,

    // Methods
    setEditing,
    fetchData,
    saveMapping,
    hasExistingMappings,
    hasZaaktypeChanged,
    revertMapping
  };
}
