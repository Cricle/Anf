import { test, expect } from '@playwright/test'

test.describe('Navigation', () => {
  test('clicking title navigates to /', async ({ page }) => {
    await page.goto('/downloads', { waitUntil: 'domcontentloaded' })
    await expect(page.locator('.app-title')).toHaveText('Anf', { timeout: 10000 })
    await page.locator('.app-title').click()
    await expect(page).toHaveURL('/')
  })

  test('navbar has navigation buttons', async ({ page }) => {
    await page.goto('/', { waitUntil: 'domcontentloaded' })
    await expect(page.locator('.app-title')).toHaveText('Anf', { timeout: 10000 })
    const navButtons = page.locator('.va-navbar__right button')
    const count = await navButtons.count()
    expect(count).toBeGreaterThanOrEqual(3)
  })
})

test.describe('Dark Mode', () => {
  test('toggle adds dark class to html', async ({ page }) => {
    await page.goto('/', { waitUntil: 'domcontentloaded' })
    await expect(page.locator('.app-title')).toHaveText('Anf', { timeout: 10000 })

    const buttons = page.locator('.va-navbar__right button')
    const count = await buttons.count()
    await buttons.nth(count - 1).click()

    await expect(page.locator('html')).toHaveClass(/dark/)
  })

  test('dark mode persists after reload', async ({ page }) => {
    await page.goto('/', { waitUntil: 'domcontentloaded' })
    await expect(page.locator('.app-title')).toHaveText('Anf', { timeout: 10000 })

    const buttons = page.locator('.va-navbar__right button')
    const count = await buttons.count()
    await buttons.nth(count - 1).click()

    await page.reload({ waitUntil: 'domcontentloaded' })
    await expect(page.locator('.app-title')).toHaveText('Anf', { timeout: 10000 })
    await expect(page.locator('html')).toHaveClass(/dark/)
  })
})
