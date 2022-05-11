import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AccountService } from 'src/app/core/services/account/account.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent implements OnInit {
  repos = []

  constructor(
    private jwtHelper: JwtHelperService, 
    private auth: AccountService
  ) { }

  ngOnInit(): void {
    
  }

  logout() {
    this.auth.logout();
  }
  
  isUserAuthenticated = (): boolean => {
    const token = localStorage.getItem("JWT_TOKEN");

    if (token && !this.jwtHelper.isTokenExpired(token)) return true;

    return false;
  }
}
