import { defineConfig } from '@playwright/test'

export default defineConfig({
  testDir: './e2e',
  timeout: 30000,
  use: {
    baseURL: 'http://localhost:5000',
    headless: true,
  },
  webServer: {
    command: 'cd ../.. && cargo run -p anf-web',
    port: 5000,
    timeout: 120000,
    reuseExistingServer: !process.env.CI,
  },
})
