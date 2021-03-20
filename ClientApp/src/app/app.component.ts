import { Component } from '@angular/core';
import { ComicWsService} from './comic-ws/comic-ws.service'
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';
  constructor(private wsService:ComicWsService){
    this.wsService.connect();
  }
}
