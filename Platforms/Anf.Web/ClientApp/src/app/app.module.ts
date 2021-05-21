import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import {NzListModule} from 'ng-zorro-antd/list'
import {NzButtonModule} from 'ng-zorro-antd/button'
import {NzSpinModule} from 'ng-zorro-antd/spin'
import {NzIconModule} from 'ng-zorro-antd/icon'
import {NzNotificationModule} from 'ng-zorro-antd/notification'
import {NzBadgeModule} from 'ng-zorro-antd/badge'
import {NzDrawerModule} from 'ng-zorro-antd/drawer'
import {NzTypographyModule} from 'ng-zorro-antd/typography'
import {NzAutocompleteModule} from 'ng-zorro-antd/auto-complete'
import {NzInputModule} from 'ng-zorro-antd/input'
import {NzTagModule} from 'ng-zorro-antd/tag'
import {NzImageModule} from 'ng-zorro-antd/image'
import {NzSpaceModule} from 'ng-zorro-antd/space'
import {NzPaginationModule} from 'ng-zorro-antd/pagination'
import {NzAffixModule } from 'ng-zorro-antd/affix'

import { ComicApiService } from './comic-api/comic-api.service';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about/about.component';
import { DownloadComponent } from './download/download/download.component';
import { ReferenceComponent } from './reference/reference/reference.component';
import { GiantScreenComponent } from './giant-screen/giant-screen/giant-screen.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { UserManager } from './comic-api/usermanager';
import { TopRankComponent } from './top-rank/top-rank.component'
import { DetailComponent} from './detail/detail.component'
import { VisitComponent} from './visit/visit.component'
import { ThemeService } from './theme.service'

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    AboutComponent,
    DownloadComponent,
    ReferenceComponent,
    GiantScreenComponent,
    DetailComponent,
    TopRankComponent,
    VisitComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    
    NzButtonModule,
    NzListModule,
    NzSpinModule,
    NzIconModule,
    NzNotificationModule,
    NzBadgeModule,
    NzDrawerModule,
    NzTypographyModule,
    NzAutocompleteModule,
    NzInputModule,
    NzTagModule,
    NzImageModule,
    NzSpaceModule,
    NzPaginationModule,
    NzAffixModule,
    
    BrowserAnimationsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'download', component: DownloadComponent },
      { path: 'about', component: AboutComponent },
      { path: 'detail', component: DetailComponent },
      { path: 'visit/:url',component:VisitComponent}
    ]),
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production })
  ],
  providers: [
    ComicApiService,
    UserManager,
    ThemeService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
