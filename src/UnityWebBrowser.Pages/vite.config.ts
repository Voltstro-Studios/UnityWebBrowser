import { viteSingleFile } from "vite-plugin-singlefile"
const { resolve } = require('path')
const { defineConfig } = require('vite')

module.exports = defineConfig({

  plugins: [viteSingleFile()],
  build: {
    target: "esnext",
    assetsInlineLimit: 100000000,
    chunkSizeWarningLimit: 100000000,
    cssCodeSplit: false,
    brotliSize: false,
    rollupOptions: {
        inlineDynamicImports: true,
        input: {
            main: resolve(__dirname, 'about.html'),
            nested: resolve(__dirname, 'error.html')
        },
        output: {
            manualChunks: () => "everything.js",
        },
    }
  }
})
