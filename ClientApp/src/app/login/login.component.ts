import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  invalidLogin: boolean = false;
  invalidLoginMessage: string = "";

  constructor(private router: Router, private http: HttpClient) { }

  async login(form: NgForm){
    const credentials = {
      'username': form.value.username,
      'password': form.value.password
    }

    try{
      let response = await this.http.post("https://localhost:8383/api/auth/login", credentials).toPromise();
      const token = (<any>response).token;
      localStorage.setItem("jwt", token);
      localStorage.setItem("username", credentials.username)
      this.invalidLogin = false;
      this.router.navigate(["/"]);
    }
    catch(err: any){
      this.invalidLogin = true;
      this.invalidLoginMessage = err.error
    }
  }
}
