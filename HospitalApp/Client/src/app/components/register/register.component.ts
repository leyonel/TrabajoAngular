import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { User } from 'src/app/core/models/user/user.model';
import { AccountService } from '../../core/services/account/account.service';
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {
  form: any = {
    email: null,
    password: null
  };
  isSuccessful = false;
  isSignUpFailed = false;
  errorMessage = '';
  
  constructor(
    private accountService: AccountService, 
    private router: Router
  ) { }

  ngOnInit(): void {
    
  }

  onSubmit(): void {
    const { email, password } = this.form;
    const values = {
      email,
      password
    }

    this.accountService.register(values)
      .subscribe((res) => {
        console.log(res);
        this.isSuccessful = true;
        this.isSignUpFailed = false;
        this.router.navigateByUrl("/");
      }, 
      (err) => {
        this.errorMessage = err.error.message;
        this.isSignUpFailed = true;
      }
    );
  }
}
