import { Component, OnInit } from '@angular/core';
import { ComicApiService } from 'src/app/comic-api/comic-api.service';
import { RandomWordResult } from 'src/app/comic-api/model';

@Component({
  selector: 'app-giant-screen',
  templateUrl: './giant-screen.component.html',
  styleUrls: ['./giant-screen.component.css']
})
export class GiantScreenComponent implements OnInit {

  result:RandomWordResult;
  constructor(private service:ComicApiService) { }

  ngOnInit() {
    this.service.getRandom().subscribe(x=>{
      this.result=x.data;
    });
  }

}
