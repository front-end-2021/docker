import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// Proxy /api sang FastAPI để khỏi phải cấu hình CORS khi phát triển.
export default defineConfig({
  plugins: [vue()],
  server: { proxy: { '/api': 'http://localhost:8000' } },
})
