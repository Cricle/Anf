import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { UserManager } from '../comic-api/usermanager'

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  userMgr: UserManager;
  constructor(userMgr: UserManager,
    private fb: FormBuilder) {
    this.userMgr = userMgr;
  }

  validateForm!: FormGroup;

  async submitForm(): Promise<void> {
    this.validateForm.disable();
    for (const i in this.validateForm.controls) {
      this.validateForm.controls[i].markAsDirty();
      this.validateForm.controls[i].updateValueAndValidity();
    }
    if (!this.validateForm.invalid) {
      const userName = this.validateForm.get('userName').value;
      const password = this.validateForm.get('password').value;
      this.userMgr.login(userName, password).subscribe({
        next: x => {
          console.log(x);
        },
        error: err => {
          console.error(err)
        },
        complete: () => {
          this.validateForm.enable()
        }
      });
    }else{
      this.validateForm.enable()
    }
  }
  ngOnInit(): void {
    this.validateForm = this.fb.group({
      userName: [null, [Validators.required]],
      password: [null, [Validators.required]],
      remember: [true]
    });
  }
}
