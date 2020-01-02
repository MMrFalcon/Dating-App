import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/user';
import { UserService } from 'src/app/services/user.service';
import { AlertyfiService } from 'src/app/services/alertyfi.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  user: User;

  constructor(private userService: UserService, private alertify: AlertyfiService, private route: ActivatedRoute) { }

  ngOnInit() {
 
    this.route.data.subscribe(data => {
      this.user = data.user;
    });
  }


}
