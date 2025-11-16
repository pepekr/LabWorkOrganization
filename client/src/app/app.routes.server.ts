import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  // Prerender the static routes
  {
    path: '',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'login',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'register',
    renderMode: RenderMode.Prerender
  },

  // Server-side render all other routes (including dynamic ones)
  // Changed from Prerender due to error when building via vagrant
  {
    path: '**',
    renderMode: RenderMode.Server
  }
];
