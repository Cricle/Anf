import { test, expect } from '@playwright/test'

test.describe('Home Page', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/', { waitUntil: 'domcontentloaded' })
  })

  test('loads and shows navbar', async ({ page }) => {
    await expect(page.locator('.app-title')).toHaveText('Anf', { timeout: 10000 })
  })

  test('shows search input', async ({ page }) => {
    const input = page.locator('.va-input input')
    await expect(input).toBeVisible({ timeout: 10000 })
  })

  test('search input accepts text', async ({ page }) => {
    const input = page.locator('.va-input input')
    await expect(input).toBeVisible({ timeout: 10000 })
    await input.fill('test query')
    await expect(input).toHaveValue('test query')
  })

  test('has navigation buttons', async ({ page }) => {
    await expect(page.locator('.app-title')).toHaveText('Anf', { timeout: 10000 })
    const navButtons = page.locator('.va-navbar__right button')
    const count = await navButtons.count()
    expect(count).toBeGreaterThanOrEqual(3)
  })
})
