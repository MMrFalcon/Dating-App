import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertyfiService } from 'src/app/services/alertyfi.service';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  @ViewChild('editForm', {static: true}) editForm: NgForm;
  user: User;

  constructor(private route: ActivatedRoute, private alertifyService: AlertyfiService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data.user;
    });
  }

  updateUser() {
    console.log(this.user);
    this.alertifyService.success('Profile updated successfully');
    this.editForm.reset(this.user);
  }

}
