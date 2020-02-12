import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertyfiService } from '../services/alertyfi.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  user: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;

  constructor(
    private authSerivce: AuthService,
    private alertify: AlertyfiService,
    private formBuilder: FormBuilder,
    private router: Router
    ) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red'
    },
    this.createRegisterForm();
  }

  passwordMatchValidator(group: FormGroup) {
    return group.get('password').value === group.get('confirmPassword').value ? null : {mismatch: true};
  }

  createRegisterForm() {
    this.registerForm = this.formBuilder.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(16)]],
      confirmPassword: ['', Validators.required]
    }, {validator: this.passwordMatchValidator});
  }

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);

      this.authSerivce.register(this.user).subscribe(() => {
      this.alertify.success('Registration successfull');
      }, error => {
        this.alertify.error(error);
      }, () => {
        this.authSerivce.login(this.user).subscribe(() => {
          this.router.navigate(['/members']);
        });
      });

    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
