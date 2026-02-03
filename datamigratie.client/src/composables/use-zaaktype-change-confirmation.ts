import { ref } from "vue";
import type { Ref } from "vue";
import { useConfirmDialog } from "@vueuse/core";

export interface ZaaktypeChangeOptions {
  currentOzZaaktypeId: Ref<string>;

  previousOzZaaktypeId: Ref<string>;

  hasMappings: () => boolean;

  onCancel?: () => void;

  onConfirm?: () => void;
}

export function useZaaktypeChangeConfirmation(options: ZaaktypeChangeOptions) {
  const confirmDialog = useConfirmDialog();

  // validate if a zaaktype change should proceed
  // show confirmation dialog if there are existing mappings
  const validateZaaktypeChange = async (detZaaktypeId?: string): Promise<boolean> => {
    const hasZaaktypeChanged =
      options.currentOzZaaktypeId.value !== options.previousOzZaaktypeId.value;

    const hasMappings = options.hasMappings();

    // If there's a DET zaaktype ID, previous OZ zaaktype, the zaaktype changed, and mappings exist
    if (detZaaktypeId && options.previousOzZaaktypeId.value && hasZaaktypeChanged && hasMappings) {
      const result = await confirmDialog.reveal();

      if (result.isCanceled) {
        // Revert to previous zaaktype
        options.currentOzZaaktypeId.value = options.previousOzZaaktypeId.value;
        options.onCancel?.();
        return false;
      }

      options.onConfirm?.();
    }

    return true;
  };

  return {
    confirmDialog,
    validateZaaktypeChange
  };
}
