export const featureFlags = {
  // show checkbox to auto-fill documenttype mappings with random values
  showDocumenttypeTestHelper: import.meta.env.VITE_ENABLE_TEST_HELPERS === 'true' || import.meta.env.DEV
};
