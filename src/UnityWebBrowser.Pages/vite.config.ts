import { viteSingleFile } from 'vite-plugin-singlefile';
import { ViteMinifyPlugin } from 'vite-plugin-minify';
import { resolve } from 'path';
import { defineConfig } from 'vite';

export default defineConfig({
  plugins: [viteSingleFile({useRecommendedBuildConfig: true, removeViteModuleLoader: true}), ViteMinifyPlugin({})],
  build: {
    target: 'esnext',
    rollupOptions: {
      input: {
        main: resolve(__dirname, 'about.html')
      }
    },
    minify: true
  },
  server: {
    open: '/about.html'
  },
  
})
