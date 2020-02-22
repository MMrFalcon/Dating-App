import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode = false;
  loggedIn = false;

  constructor(private http: HttpClient, private authService: AuthService) { }

  ngOnInit() {
    this.checkIfLoggedIn();
  }

  checkIfLoggedIn() {
    if (this.authService.decodedToken) {
      this.loggedIn = true;
    }
  }

  registerToggle() {
      this.registerMode = true;
  }


  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }
}
