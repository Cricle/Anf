import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { NgZorroAntdModule, NzDrawerModule, NzIconModule} from 'ng-zorro-antd'

import { ComicApiService } from './comic-api/comic-api.service';
import { ComicManager } from './comic-api/comic-mgr'
import { ComicWsService } from './comic-ws/comic-ws.service';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about/about.component';
import { DownloadComponent } from './download/download/download.component';
import { ReferenceComponent } from './reference/reference/reference.component';
import { GiantScreenComponent } from './giant-screen/giant-screen/giant-screen.component';
import { AnalysisStatusComponent } from './analysisi-status/analysis-status/analysis-status.component';
import { AnalysisSearchComponent } from './analysis-search/analysis-search/analysis-search.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { IconDefinition } from '@ant-design/icons-angular';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    AboutComponent,
    DownloadComponent,
    ReferenceComponent,
    GiantScreenComponent,
    AnalysisStatusComponent,
    AnalysisSearchComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    NgZorroAntdModule,
    BrowserAnimationsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'download', component: DownloadComponent },
      { path: 'about', component: AboutComponent },
      { path: 'ref', component: ReferenceComponent }
    ]),
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production })
  ],
  providers: [
    ComicApiService,
    ComicWsService,
    ComicManager
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
