import { Router } from '@angular/router';
import { take } from 'rxjs/operators';
import { User } from './../_models/user';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
   private hubConnection: HubConnection;
   private onlineuserSource = new BehaviorSubject<string[]>([]);
   onlineUser$ = this.onlineuserSource.asObservable();

  constructor(private toastr: ToastrService,private router:Router) { }

  creatHubConnection(user: User){
    this.hubConnection = new HubConnectionBuilder()
        .withUrl(this.hubUrl+ 'presence',{
          accessTokenFactory:() => user.token
        })
        .withAutomaticReconnect()
        .build()
    this.hubConnection
        .start()
        .catch(err => console.log(err))
    this.hubConnection.on('UserIsOnline', (username) => {
      this.toastr.info(username + ' has connected');
      this.onlineUser$.pipe(take(1)).subscribe(usernames => {
        this.onlineuserSource.next([...usernames,username])
      })
    });
    this.hubConnection.on('UserIsOffline', (username) => {
      this.toastr.warning(username + ' has disconnected');
      this.onlineUser$.pipe(take(1)).subscribe((usernames) => {
        this.onlineuserSource.next([...usernames.filter(x => x !== username)]);
      });
    });
    this.hubConnection.on('GetOnLineUsers',(usernames: string[]) => {
      this.onlineuserSource.next(usernames);
    this.hubConnection.on('NewMessageReceived',({username,knownAs}) => {
        this.toastr.info(knownAs+ " has sen you a new message!")
        .onTap
        .pipe(take(1))
        .subscribe(() => this.router.navigateByUrl('/members/'+ username+ '?tab=3'))
      });
    });
  }


  stopHubConnection(){
    this.hubConnection.stop().catch(err => console.log(err))
  }
}
