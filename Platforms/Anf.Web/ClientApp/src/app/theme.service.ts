import { Injectable } from '@angular/core';

enum ThemeType {
  dark = 'dark',
  default = 'default',
  followSystem = 'followSystem'
}
const THEME_KEY: string = "Anf.Theme";
@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  _actualIsDark: boolean = false;
  _currentTheme: ThemeType = ThemeType.default;

  public get currentTheme(): ThemeType {
    return this._currentTheme;
  }

  public get actualIsDark(): boolean {
    return this._actualIsDark;
  }

  public get isDarkTheme(): boolean {
    return this._currentTheme == ThemeType.dark;
  }


  public set currentTheme(v: ThemeType) {
    this._currentTheme = v;
    localStorage.setItem(THEME_KEY, v);
  }


  public get systemTheme(): ThemeType {
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? ThemeType.dark : ThemeType.default;
  }

  public get storedTheme(): ThemeType {
    const ss = localStorage.getItem(THEME_KEY);
    const type = <ThemeType>(ss);
    if (type) {
      return type;
    }
    return ThemeType.default;
  }
  public loadStoredTheme() {
    this.currentTheme = this.storedTheme;
    this.loadTheme(false);
  }
  constructor() {
    this.listenFollowSystem();
  }

  private reverseTheme(theme: string): ThemeType {
    if (this.currentTheme==ThemeType.followSystem) {
      return ThemeType.followSystem;
    }
    return theme === ThemeType.dark ? ThemeType.default : ThemeType.dark;
  }

  private removeUnusedTheme(theme: ThemeType): void {
    document.documentElement.classList.remove(theme);
    const removedThemeStyle = document.getElementById(theme);
    if (removedThemeStyle) {
      document.head.removeChild(removedThemeStyle);
    }
  }

  private loadCss(href: string, id: string): Promise<Event> {
    return new Promise((resolve, reject) => {
      const style = document.createElement('link');
      style.rel = 'stylesheet';
      style.href = href;
      style.id = id;
      style.onload = resolve;
      style.onerror = reject;
      document.head.append(style);
    });
  }

  public loadTheme(firstLoad = true): Promise<Event> {
    let theme = this.currentTheme;
    if (theme == ThemeType.followSystem) {
      theme = this.systemTheme;
    }
    if (firstLoad) {
      document.documentElement.classList.add(theme);
    }
    return new Promise<Event>((resolve, reject) => {
      this.loadCss(`${theme}.css`, theme).then(
        (e) => {
          if (!firstLoad) {
            document.documentElement.classList.add(theme);
          }
          this.removeUnusedTheme(this.reverseTheme(theme));
          this._actualIsDark = theme == ThemeType.dark;
          resolve(e);
        },
        (e) => reject(e)
      );
    });
  }
  private listenFollowSystem() {
    const that = this;
    const listenerMedia = [
      '(prefers-color-scheme: dark)',
      '(prefers-color-scheme: light)'
    ];
    for (const media of listenerMedia) {
      window.matchMedia(media).addEventListener('change', (ev: MediaQueryListEvent) => {
        if (this.currentTheme == ThemeType.followSystem) {
          this.loadTheme(false);
        }
        return null;
      });
    }
  }
  public toggleTheme(): Promise<Event> {
    this.currentTheme = this.reverseTheme(this.currentTheme);
    return this.loadTheme(false);
  }
}
