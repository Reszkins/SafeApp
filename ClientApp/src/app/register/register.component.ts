import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  invalidRegister: boolean = false;

  constructor(private router: Router, private http: HttpClient) { }

  async register(form: NgForm){
    const credentials = {
      'username': form.value.username,
      'password': form.value.password
    }

    if(credentials.password.length < 8){
      this.invalidRegister = true;
      return;
    }

    try{
      let response = await this.http.post("https://localhost:8383/api/auth/register", credentials).toPromise();
      const token = (<any>response).token;
      localStorage.setItem("jwt", token);
      localStorage.setItem("username", credentials.username)
      this.invalidRegister = false;
      this.router.navigate(["/"]);
    }
    catch(err: any){
      this.invalidRegister = true;
    }
  }

  checkStrength(password: string) {
    let force = 0;

    const regex = /[$-/:-?{-~!"^_@`\[\]]/g;
    const lowerLetters = /[a-z]+/.test(password);
    const upperLetters = /[A-Z]+/.test(password);
    const numbers = /[0-9]+/.test(password);
    const symbols = regex.test(password);

    const flags = [lowerLetters, upperLetters, numbers, symbols];

    let passedMatches = 0;
    for (const flag of flags) {
      passedMatches += flag === true ? 1 : 0;
    }

    force = 1;
    if(password.length < 6){
      force = 0;
    } 
    if(password.length > 10){
      force = 2;
    } 

    if(passedMatches === 1){
      force++;
    } 
    if(passedMatches === 2){
      force = force + 2;
    } 
    if(passedMatches === 3){
      force = force + 3;
    } 
    if(passedMatches === 4){
      force = force + 4;
    } 

    return force;
  }

}
