import { ref } from "vue";

interface FeatureFlags {
  showDocumenttypeTestHelper: boolean;
}

export const featureFlags = ref<FeatureFlags>({
  showDocumenttypeTestHelper: false
});

export async function loadFeatureFlags() {
  try {
    const response = await fetch("/api/app-version");
    const data = await response.json();
    featureFlags.value.showDocumenttypeTestHelper = data.enableTestHelpers || false;
  } catch (error) {
    console.error("Failed to load feature flags:", error);
  }
}
