import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite'

// Photos are served by Caddy over HTTPS with a self-signed certificate, which browsers refuse for
// sub-resources. In development the client asks for /media/... instead (see .env.development) and
// Vite forwards it to Caddy, ignoring the certificate. Production builds keep using VITE_IMAGES_URL.
const media = {
  '/media': {
    target: 'https://localhost:5702',
    changeOrigin: true,
    secure: false,
    rewrite: (path: string) => path.replace(/^\/media/, ''),
  },
}

// 5173 is the origin the API's CORS policy allows by default (FrontendOrigin),
// so neither the API nor compose.yaml needs any change to run this client.
export default defineConfig({
  plugins: [vue(), tailwindcss()],
  server: { port: 5173, strictPort: true, proxy: media },
  preview: { port: 5173, strictPort: true, proxy: media },
})
