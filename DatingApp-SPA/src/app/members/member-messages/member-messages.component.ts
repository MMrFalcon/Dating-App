import { Component, OnInit, Input, Output } from '@angular/core';
import { Message } from 'src/app/models/message';
import { UserService } from 'src/app/services/user.service';
import { AuthService } from 'src/app/services/auth.service';
import { AlertyfiService } from 'src/app/services/alertyfi.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private alertifyService: AlertyfiService
  ) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    const currentUserId = +this.authService.decodedToken.nameid;
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
    .pipe(
      tap(messages => {
        for (const message of messages) {
          if (message.isRead === false && message.recipientId === currentUserId) {
            this.userService.markAsRead(currentUserId, message.id);
          }
        }
      })
    )
    .subscribe(messages => {
      this.messages = messages;
    }, errorResponse => {
      this.alertifyService.error(errorResponse);
    });
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage).subscribe((message: Message) => {
      this.messages.unshift(message);
      this.newMessage.content = '';
    }, errorResponse => {
      this.alertifyService.error(errorResponse);
    });
  }

}
