import { createRouter, createWebHistory } from 'vue-router'
import { checkAuth } from './composables/useAuth'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', redirect: '/home' },
    { path: '/home', component: () => import('./views/HomeView.vue'), meta: { title: 'Home' } },
    { path: '/albums', component: () => import('./views/AlbumsView.vue'), meta: { title: 'Albums' } },
    { path: '/albums/:id', component: () => import('./views/AlbumView.vue'), meta: { title: 'Album' } },
    { path: '/about-me', component: () => import('./views/AboutView.vue'), meta: { title: 'About' } },
    { path: '/contact', component: () => import('./views/ContactView.vue'), meta: { title: 'Contact' } },
    { path: '/config/login', component: () => import('./views/admin/LoginView.vue'), meta: { title: 'Sign in' } },
    {
      path: '/config',
      component: () => import('./views/admin/AdminLayout.vue'),
      meta: { requiresAuth: true },
      children: [
        { path: '', redirect: '/config/photos' },
        { path: 'photos', component: () => import('./views/admin/PhotosAdmin.vue'), meta: { title: 'Photos' } },
        { path: 'albums', component: () => import('./views/admin/AlbumsAdmin.vue'), meta: { title: 'Albums' } },
        { path: 'albums/:id', component: () => import('./views/admin/AlbumEdit.vue'), meta: { title: 'Edit album' } },
        { path: 'tiles', component: () => import('./views/admin/TilesAdmin.vue'), meta: { title: 'Tiles' } },
        { path: 'home', component: () => import('./views/admin/HomeAdmin.vue'), meta: { title: 'Home page' } },
      ],
    },
    { path: '/:pathMatch(.*)*', component: () => import('./views/NotFoundView.vue'), meta: { title: 'Not found' } },
  ],
  scrollBehavior: (to, from, saved) => saved ?? (to.path === from.path ? undefined : { top: 0 }),
})

router.beforeEach(async (to) => {
  if (to.matched.some((r) => r.meta.requiresAuth) && !(await checkAuth())) {
    return { path: '/config/login', query: { redirect: to.fullPath } }
  }
})

router.afterEach((to) => {
  document.title = `${(to.meta.title as string | undefined) ?? 'Admin'} - ByShutter`
})

export default router
