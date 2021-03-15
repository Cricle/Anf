import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { NzButtonModule } from 'ng-zorro-antd/button'
import { NzTagComponent } from 'ng-zorro-antd/tag'

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { GiantScreenComponent } from './giant-screen/giant-screen/giant-screen.component';
import { AnalysisSearchComponent } from './analysis-search/analysis-search/analysis-search.component';
import { AnalysisStatusComponent } from './analysisi-status/analysis-status/analysis-status.component';
import { AboutComponent } from './about/about/about.component';
import { DownloadComponent } from './download/download/download.component';
import { ReferenceComponent } from './reference/reference/reference.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    GiantScreenComponent,
    AnalysisSearchComponent,
    AnalysisStatusComponent,
    AboutComponent,
    DownloadComponent,
    ReferenceComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    NzButtonModule,
    NzTagComponent,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'download', component: DownloadComponent },
      { path: 'about', component: AboutComponent },
      { path: 'ref', component: ReferenceComponent }
    ]),
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
