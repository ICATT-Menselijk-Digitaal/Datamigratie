import { createRouter, createWebHistory } from "vue-router";
import ZaaktypesView from "@/views/ZaaktypesView.vue";
import ZaaktypeView from "@/views/ZaaktypeView.vue";
import UnauthorizedView from "@/views/UnauthorizedView.vue";
import LoginView from "@/views/LoginView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "detZaaktypes",
      component: ZaaktypesView,
      meta: {
        title: "e-Suite Zaaktypes"
      }
    },
    {
      path: "/det/zaaktype/:detZaaktypeId",
      name: "detZaaktype",
      component: ZaaktypeView,
      props: true,
      meta: {
        title: "e-Suite Zaaktype"
      }
    },
    {
      path: "/forbidden",
      name: "forbidden",
      component: UnauthorizedView,
      meta: {
        title: "Forbidden"
      }
    },
    {
      path: "/login",
      name: "login",
      component: LoginView,
      meta: {
        title: "Login"
      }
    }
  ]
});

const title = document.title;

router.beforeEach((to) => {
  document.title = `${to.meta?.title ? to.meta.title + " | " : ""}${title}`;
});

export default router;
