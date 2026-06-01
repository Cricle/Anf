import { test, expect } from '@playwright/test'

test.describe('Downloads Page', () => {
  test('loads and shows empty state', async ({ page }) => {
    await page.goto('/downloads', { waitUntil: 'domcontentloaded' })
    await expect(page.locator('.app-title')).toHaveText('Anf', { timeout: 10000 })
    await expect(page.locator('h2')).toHaveText('Downloads', { timeout: 10000 })
    await expect(page.locator('text=No active downloads')).toBeVisible({ timeout: 10000 })
  })
})

test.describe('Comic Detail Page', () => {
  test('handles missing url parameter', async ({ page }) => {
    await page.goto('/comic', { waitUntil: 'domcontentloaded' })
    await expect(page.locator('.app-title')).toHaveText('Anf', { timeout: 10000 })
  })
})

test.describe('Reader Page', () => {
  test('handles missing parameters', async ({ page }) => {
    await page.goto('/reader', { waitUntil: 'domcontentloaded' })
    await expect(page.locator('.app-title')).toHaveText('Anf', { timeout: 10000 })
  })
})

test.describe('Bookshelf Page', () => {
  test('loads without errors', async ({ page }) => {
    await page.goto('/bookshelf', { waitUntil: 'domcontentloaded' })
    await expect(page.locator('.app-title')).toHaveText('Anf', { timeout: 10000 })
  })
})
