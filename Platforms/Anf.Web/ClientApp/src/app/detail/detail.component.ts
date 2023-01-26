import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { ComicApiService } from '../comic-api/comic-api.service';
import { SearchService } from '../comic-api/comic-search.service';
import { ComicSource, SearchComicResult, SetResult, SortedItem } from '../comic-api/model';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.css']
})
export class DetailComponent implements OnInit {
  public searcher: SearchService;
  constructor(private route: ActivatedRoute,
    private notify: NzNotificationService,
    searcher: SearchService,
    private rt: Router) {
    this.searcher = searcher;
    this.route.queryParamMap.subscribe(x => {
      this.searcher.keyword = x.get("k");
      if (this.searcher.keyword) {
        this.searcher.provider = x.get("p");
        const currentPage = Number.parseInt(x.get("c"));
        if (!Number.isNaN(currentPage)) {
          this.searcher.currentPage = currentPage;
        }
        this.searcher.search();
      }
    });

  }

  searchKeydown(event: KeyboardEvent) {
    if (event.key == "Enter") {
      this.searcher.search();
    }
  }
  ngOnInit() {
  }
  goVisit(source: ComicSource) {
    this.rt.navigate(['/visit', source.targetUrl,0]);
  }
}
