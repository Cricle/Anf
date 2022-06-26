import { ChangeDetectionStrategy, Component, Input, OnInit, Output } from '@angular/core';

import { ActivatedRoute } from '@angular/router'
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { ComicApiService } from '../comic-api/comic-api.service';
import { ComicScope, VisitManager, VisitPos } from '../comic-api/comic-visit.mgr';
import { AnfComicEntityTruck, ComicChapter, ComicPage, WithPageChapter } from '../comic-api/model';
import { BehaviorSubject, firstValueFrom, Observable, Subject } from 'rxjs'
import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { catchError, takeUntil } from 'rxjs/operators';
@Component({
  selector: 'app-img-of',
  templateUrl: './img-of.component.html',
  styleUrls: ['./img-of.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ImgOfComponent {
  _pages:ComicPage[];
  items = Array.from({length: 1000}).map((_, i) => `Item #${i}`);
  @Input()
  public set pages(v : ComicPage[]) {
    this._pages = v;
  }

  @Output()
  public
  public get pages() : ComicPage[] {
    return this._pages;
  }

}
