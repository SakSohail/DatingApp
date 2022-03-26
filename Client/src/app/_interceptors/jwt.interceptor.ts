import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountService } from '../_services/account-service.service';
import { User } from '../_models/user';
// ng g interceptor jwt
@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  //interceptor initialized once application start
  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let currentUser: User | null;

    this.accountService.currentUser$.pipe(take(1)).subscribe((user)=>{
     currentUser = user
     //pipe(take(1)) - we want to complete after using one of the user received,then we dont need to unsubscribe it
    if (currentUser) {
      request = request.clone({ //here we are cloinign request and adding headers to it
        setHeaders: {
          Authorization: `Bearer ${currentUser.token}` //this will attach token to every request logs in 
        }
      })
    }
    }); 
    

    return next.handle(request);
  }
}
