<template>
  <prompt-modal
    :dialog="dialog"
    cancel-text="Annuleren"
    confirm-text="Ja, wijzig zaaktype"
  >
    <h2>{{ title }}</h2>

    <p>
      <strong>Let op:</strong> {{ warningText }}
    </p>
    <p>
      {{ descriptionText }}
    </p>
    <p>
      {{ confirmationText }}
    </p>
  </prompt-modal>
</template>

<script setup lang="ts">
import PromptModal from "@/components/PromptModal.vue";
import type { UseConfirmDialogReturn } from "@vueuse/core";

interface Props {
  dialog: UseConfirmDialogReturn<any, any, any>;
  title?: string;
  warningText?: string;
  descriptionText?: string;
  confirmationText?: string;
  mappingType?: string;
}

withDefaults(defineProps<Props>(), {
  title: "Open Zaak zaaktype wijzigen",
  warningText: "Als je het Open Zaak zaaktype wijzigt, worden alle bestaande mappings verwijderd.",
  descriptionText: "Je moet de mappings opnieuw configureren voor het nieuwe zaaktype.",
  confirmationText: "Weet je zeker dat je wilt doorgaan?",
  mappingType: "mappings"
});
</script>
