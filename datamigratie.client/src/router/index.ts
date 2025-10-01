import { createRouter, createWebHistory } from "vue-router";
import ZaaktypesView from "@/views/ZaaktypesView.vue";
import ZaaktypeView from "@/views/ZaaktypeView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "detZaaktypes",
      component: ZaaktypesView,
      meta: {
        title: "DET Zaaktypes"
      }
    },
    {
      path: "/det/zaaktype/:detZaaktypeId",
      name: "detZaaktype",
      component: ZaaktypeView,
      props: true,
      meta: {
        title: "DET Zaaktype"
      }
    }
  ]
});

const title = document.title;

router.beforeEach((to) => {
  document.title = `${to.meta?.title ? to.meta.title + " | " : ""}${title}`;
});

export default router;
