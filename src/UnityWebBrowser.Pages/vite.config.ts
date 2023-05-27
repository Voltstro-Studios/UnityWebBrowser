import { viteSingleFile } from 'vite-plugin-singlefile';
import { createHtmlPlugin } from 'vite-plugin-html';
import { resolve } from 'path';
import { defineConfig } from 'vite';

export default defineConfig({
  plugins: [viteSingleFile({useRecommendedBuildConfig: true, removeViteModuleLoader: true}), createHtmlPlugin({minify: true})],
  build: {
    target: 'esnext',
    rollupOptions: {
        input: {
            main: resolve(__dirname, 'about.html')
        }
    }
  },
  server: {
    open: '/about.html'
  },
  
})
