import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/models/user';
import { AuthService } from 'src/app/services/auth.service';
import { UserService } from 'src/app/services/user.service';
import { AlertyfiService } from 'src/app/services/alertyfi.service';

@Component({
  selector: 'app-member-cards',
  templateUrl: './member-cards.component.html',
  styleUrls: ['./member-cards.component.css']
})
export class MemberCardsComponent implements OnInit {
  @Input() user: User;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertyfi: AlertyfiService
    ) { }

  ngOnInit() {
  }

  sendLike(recipientId: number) {
    this.userService.sendLike(this.authService.decodedToken.nameid, recipientId).subscribe(data => {
      this.alertyfi.success('You have liked: ' + this.user.knownAs);
    }, errorResponse => {
      this.alertyfi.error(errorResponse);
    });
  }

}
