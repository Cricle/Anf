import { Component, Input, OnInit, Output } from '@angular/core';
import { ComicApiService } from '../comic-api/comic-api.service';
import { AnfComicEntityTruck, SetResult, SortedItem } from '../comic-api/model';
import { NzNotificationService } from 'ng-zorro-antd/notification';

import { Observable } from 'rxjs'

const coff: number = 0.6;

@Component({
  selector: 'app-comic-list',
  templateUrl: './comic-list.component.html',
  styleUrls: ['./comic-list.component.css']
})
export class ComicListComponent implements OnInit {
  @Input()
  @Output()
  loading: boolean;
  @Input()
  @Output()
  rank: SetResult<SortedItem>;
  @Output()
  selectedItem: SortedItem;
  @Output()
  visitComic: AnfComicEntityTruck;
  @Output()
  loadingVisit: boolean;
  @Output()
  emptyEntity: boolean;
  @Output()
  showVisit: boolean;
  @Output()
  drawerWidth: number;

  constructor(private api: ComicApiService,
    private notify: NzNotificationService) {
  }

  ngOnInit(): void {
  }

  loadEntity(item: SortedItem) {
    const locItem = item;
    this.selectedItem = item;
    this.showVisit = true;
    this.loadingVisit = true;
    this.emptyEntity = false;
    this.visitComic = null;
    this.drawerWidth = Math.max(300, document.body.clientWidth * coff);
    this.api.getEntity(item.address).subscribe({
      next:x => {
        if (this.selectedItem == locItem) {
          this.visitComic = x;
          this.emptyEntity = x == null;
          console.log(this.visitComic);
        }
      },
      error:err => {
        this.notify.error('Loading visit entity fail!', err);
      },
      complete:() =>{
        this.loadingVisit = false;
      }
    });
  }
  close() {
    this.showVisit = false;
  }
  copy(event: MouseEvent) {
  }
  loadAsync(dataProivder:()=>Observable<SetResult<SortedItem>>){
    if(this.loading){
      return;
    }
    this.loading=true;
    const provider=dataProivder();
    provider.subscribe({
      next:x => {
        this.rank=x;
      },
      error:err => {
        this.loading=false;
      },
      complete:() => this.loading = false
    });
  }
}
