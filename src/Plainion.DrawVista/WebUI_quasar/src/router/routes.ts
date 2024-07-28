import { RouteRecordRaw } from 'vue-router';


const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'browse',
    component: () => import('../views/BrowserView.vue')
  },
  {
    path: '/upload',
    name: 'upload',
    component: () => import('../views/UploadView.vue')
  },
  {
    path: '/clear',
    name: 'clear',
    component: () => import('../views/ClearView.vue')
  }
];

const allRoutes: RouteRecordRaw[] = routes.concat([
  {
    path: '/:catchAll(.*)*',
    component: () => import('../views/ErrorNotFound.vue'),
  },
]);

export {routes, allRoutes};
