import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../services/user.service';
import { AlertyfiService } from '../services/alertyfi.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Message } from '../models/message';
import { AuthService } from '../services/auth.service';

@Injectable()
export class MessagesResolver implements Resolve<Message[]> {

    pageNumber = 1;
    pageSize = 5;
    messageContainer = 'Unread';

    constructor(
        private userService: UserService,
        private router: Router,
        private alertify: AlertyfiService,
        private authService: AuthService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
        return this.userService.getMessages(this.authService.decodedToken.nameid, this.pageNumber,
            this.pageSize, this.messageContainer).pipe(

            catchError(errorFromResponse => {
                this.alertify.error('Problem retrieving messages');
                this.router.navigate(['/home']);
                return of(null);
            })

        );
    }
}
