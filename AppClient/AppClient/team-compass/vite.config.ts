import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";
import path from "path";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => ({
    server: {
        host: "::",
        port: 8080,
        hmr: {
            overlay: false,
        },

        // ?? OPCIÓN ORIGINAL (LOCAL) — NO TOCAR
        /*
        proxy: {
          "/api": {
            target: "https://localhost:5001", // Tu backend ASP.NET Core
            changeOrigin: true,
            secure: false, // Para certificados SSL de desarrollo
          },
        },
        */

        // ?? OPCIÓN DOCKER (ACTIVAR ESTA)
        proxy: {
            "/api": {
                target: "http://backend:8080",
                changeOrigin: true,
                secure: false,
            },
        },
    },

    plugins: [react()],
    resolve: {
        alias: {
            "@": path.resolve(__dirname, "./src"),
        },
    },
}));