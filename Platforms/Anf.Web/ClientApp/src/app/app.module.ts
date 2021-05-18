import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { NgZorroAntdModule} from 'ng-zorro-antd';
import {NzListModule} from 'ng-zorro-antd/list'

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
import {TimeThrowComponent} from './time-throw/time-throw.component'
import {BookMgrComponent} from './book-mgr/book-mgr.component'
import { UserManager } from './comic-api/usermanager';
import { TopRankComponent } from './top-rank/top-rank.component'

import { ClipboardModule } from 'ngx-clipboard';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    AboutComponent,
    DownloadComponent,
    ReferenceComponent,
    GiantScreenComponent,
    TimeThrowComponent,
    BookMgrComponent,
    TopRankComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    NgZorroAntdModule,
    NzListModule,
    ClipboardModule,
    BrowserAnimationsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'download', component: DownloadComponent },
      { path: 'about', component: AboutComponent },
      { path: 'ref', component: ReferenceComponent },
    ]),
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production })
  ],
  providers: [
    ComicApiService,
    UserManager
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
