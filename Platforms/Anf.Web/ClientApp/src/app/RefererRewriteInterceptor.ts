import {
    HttpEvent, HttpInterceptor, HttpHandler, HttpRequest
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class RefererRewriteInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const originRef = req.headers.get("referer");
        if (originRef) {
            const urlRef = new URL(req.url).origin;
            req.headers.set("referer", urlRef);
        }
        console.log(req);
        return next.handle(req);
    }
}