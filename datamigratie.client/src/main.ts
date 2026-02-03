import "./assets/base.css";
import "./assets/main.scss";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import { loadFeatureFlags } from "./config/featureFlags";

const app = createApp(App);

(async () => {
  await loadFeatureFlags();

  // Load router after theme, to be able to use theme settings
  const { default: router } = await import("./router");
  const { default: routerGuardsPlugin } = await import("./plugins/routerGuards");

  app.use(router);
  app.use(routerGuardsPlugin, router);

  app.mount("#app");
})();
