import { Component } from '@angular/core';
import { ComicApiService } from '../comic-api/comic-api.service';
import { UserManager } from '../comic-api/usermanager';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  constructor(um:UserManager){
    um.login("hello","Asdfg123456").subscribe(x=>{
      console.log(x);
    });
  }
}
