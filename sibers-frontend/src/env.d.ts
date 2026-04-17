/// <reference types="vite/client" />

// #типы_среды / #environment_types
// Type declarations for environment variables
interface ImportMetaEnv {
    readonly VITE_API_URL: string
  }
  
interface ImportMeta {
readonly env: ImportMetaEnv
}