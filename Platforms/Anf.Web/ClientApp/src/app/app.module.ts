import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { ScrollingModule } from "@angular/cdk/scrolling";

import { NzBackTopModule } from 'ng-zorro-antd/back-top';
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
import {NzDropDownModule } from 'ng-zorro-antd/dropdown'
import {NzFormModule } from 'ng-zorro-antd/form'
import { NzProgressModule } from 'ng-zorro-antd/progress'
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzAlertModule } from 'ng-zorro-antd/alert';

import { ComicApiService } from './comic-api/comic-api.service';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about/about.component';
import { DownloadComponent } from './download/download/download.component';
import { ReferenceComponent } from './reference/reference/reference.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { UserManager } from './comic-api/usermanager';
import { TopRankComponent } from './top-rank/top-rank.component'
import { DetailComponent} from './detail/detail.component'
import { VisitComponent} from './visit/visit.component'
import { ThemeService } from './theme.service'
import { BookshelfComponent } from './bookshelf/bookshelf.component'
import { ComicListComponent } from './comic-list/comic-list.component'
import { LoginComponent } from './login/login.component'
// import { LazyLoadImageModule,LAZYLOAD_IMAGE_HOOKS, ScrollHooks } from 'ng-lazyload-image';

import { SearchService } from './comic-api/comic-search.service';
import { VisitManager } from './comic-api/comic-visit.mgr'
import { ErrorComponent } from './error/error.component';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { ImgOfComponent } from './img-of/img-of.component';
@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    AboutComponent,
    DownloadComponent,
    ReferenceComponent,
    DetailComponent,
    TopRankComponent,
    VisitComponent,
    BookshelfComponent,
    ComicListComponent,
    LoginComponent,
    ImgOfComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,

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
    NzDropDownModule,
    NzFormModule,
    NzToolTipModule,
    NzProgressModule,
    NzBackTopModule,
    NzStepsModule,
    NzAlertModule,

    ScrollingModule,

    BrowserAnimationsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'download', component: DownloadComponent },
      { path: 'about', component: AboutComponent },
      { path: 'rank', component: TopRankComponent },
      { path: 'about', component: AboutComponent },
      { path: 'bookshelf', component: BookshelfComponent },
      { path: 'login', component: LoginComponent },
      { path: 'visit/:url/:index',component:VisitComponent},
      { path: 'error', component:ErrorComponent}
    ]),
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production })
  ],
  providers: [
    ComicApiService,
    UserManager,
    ThemeService,
    SearchService,
    VisitManager
    // { provide: LAZYLOAD_IMAGE_HOOKS, useClass: ScrollHooks }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
