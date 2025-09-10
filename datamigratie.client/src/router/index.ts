import { createRouter, createWebHistory } from "vue-router";
import ZaaktypesView from "@/views/ZaaktypesView.vue";
import ZaaktypeView from "@/views/ZaaktypeView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "zaaktypes",
      component: ZaaktypesView,
      meta: {
        title: "Zaaktypes"
      }
    },
    {
      path: "/zaaktype/:functioneleIdentificatie",
      name: "zaaktype",
      component: ZaaktypeView,
      props: true,
      meta: {
        title: "Zaaktype"
      }
    }
  ]
});

export default router;
