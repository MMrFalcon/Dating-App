import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertyfiService } from '../services/alertyfi.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerFrom: FormGroup;

  constructor(private authSerivce: AuthService, private alertify: AlertyfiService) { }

  ngOnInit() {
    this.registerFrom = new FormGroup({
      username: new FormControl('', Validators.required),
      password: new FormControl('',
      [Validators.required, Validators.minLength(6), Validators.maxLength(16)]),
      confirmPassword: new FormControl('', Validators.required)
    }, this.passwordMatchValidator);
  }

  passwordMatchValidator(group: FormGroup) {
    return group.get('password').value === group.get('confirmPassword').value ? null : {mismatch: true};
  }

  register() {
    // this.authSerivce.register(this.model).subscribe(() => {
    //   this.alertify.success('Registration successfull');
    // }, error => {
    //   this.alertify.error(error);
    // });
    console.log(this.registerFrom.value);
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
