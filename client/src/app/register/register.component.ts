import { AccountService } from './../_services/account.service';
import { Component, Input, OnInit, Output,EventEmitter } from '@angular/core';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  constructor(private acountService: AccountService, private toastr:ToastrService) {}

  ngOnInit(): void {}
  register() {
    this.acountService.register(this.model).subscribe(
      (res) => {
        console.log(res);
        this.cancel();
      },
      (err) => {
        console.log(err);
        this.toastr.error(err.error);
      }
    );
  }
  cancel() {
    this.cancelRegister.emit(false);
  }
}
