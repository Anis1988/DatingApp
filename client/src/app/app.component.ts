import { PresenceService } from './_services/presence.service';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';
import { Component, OnInit } from '@angular/core';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating App';
  users : any;

  constructor(private accountService: AccountService,private presence: PresenceService) {}

  ngOnInit() {
    this.setCurrentUser();
  }
  setCurrentUser(){
    const user : User = JSON.parse(localStorage.getItem('user') as string);
    if (user) {
      this.accountService.setCurrentUser(user);
      this.presence.creatHubConnection(user)
    }
  }
}
