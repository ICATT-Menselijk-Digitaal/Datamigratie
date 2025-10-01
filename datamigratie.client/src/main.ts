import "./assets/base.css";
import "./assets/main.scss";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";

async function enableMocking() {
  if (import.meta.env.MODE !== "development") return;

  const { worker } = await import("./mock/browser");

  return worker.start({ onUnhandledRequest: "bypass" });
}

await enableMocking();

const app = createApp(App);

app.use(router);
app.mount("#app");
