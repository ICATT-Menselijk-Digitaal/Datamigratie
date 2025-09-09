import { createRouter, createWebHistory } from "vue-router";
import HomeView from "@/views/HomeView.vue";
import ZaaktypeView from "@/views/ZaaktypeView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: HomeView,
      meta: {
        title: "Home"
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
