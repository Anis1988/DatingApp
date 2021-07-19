import { AccountService } from './../_services/account.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public accountService: AccountService,private router:Router,private toastr:ToastrService) { }

  ngOnInit(): void {}

  login(){
    this.accountService.login(this.model).subscribe(res => {
      this.router.navigateByUrl('/members')

    }, err =>{
      console.log(err)
      this.toastr.error(err.error);
    });
  }
  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/')
  }
}
