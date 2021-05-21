import { Component } from '@angular/core';
import { ThemeService } from '../theme.service'
@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  themeSer: ThemeService;
  constructor(themeSer: ThemeService) {
    this.themeSer = themeSer;
  }
  public switchTheme() {
    this.themeSer.toggleTheme();
  }
}
