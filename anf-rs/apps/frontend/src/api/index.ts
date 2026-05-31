import type { ComicAdapter } from './types'

function isTauri(): boolean {
  return typeof window !== 'undefined' && '__TAURI_INTERNALS__' in window
}

let _adapter: ComicAdapter | null = null

export async function getAdapter(): Promise<ComicAdapter> {
  if (_adapter) return _adapter
  if (isTauri()) {
    const { tauriAdapter } = await import('./tauri-adapter')
    _adapter = tauriAdapter
  } else {
    const { httpAdapter } = await import('./http-adapter')
    _adapter = httpAdapter
  }
  return _adapter
}

export type * from './types'
