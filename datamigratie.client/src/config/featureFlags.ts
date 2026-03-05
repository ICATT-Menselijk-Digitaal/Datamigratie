import { ref } from "vue";

interface FeatureFlags {
  showTestHelpers: boolean;
}

export const featureFlags = ref<FeatureFlags>({
  showTestHelpers: false
});

export async function loadFeatureFlags() {
  try {
    const response = await fetch("/api/app-version");
    const data = await response.json();
    featureFlags.value.showTestHelpers = data.enableTestHelpers || false;
  } catch (error) {
    console.error("Failed to load feature flags:", error);
  }
}
