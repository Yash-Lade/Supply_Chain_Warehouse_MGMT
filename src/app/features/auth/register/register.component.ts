import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {

  private fb = inject(FormBuilder);
  private router = inject(Router);
  private authService = inject(AuthService);

  registerForm = this.fb.group({
    FullName: ['', Validators.required],
    Email: ['', [Validators.required, Validators.email]],
    Password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required],
    RoleId: [2, Validators.required]
  });

  register() {

    console.log(this.registerForm);
  console.log(this.registerForm.errors);
  console.log(this.registerForm.controls.Password.errors);
    if (this.registerForm.invalid) {
      console.log("Form invalid", this.registerForm.value);
      return;
    }

    const formValue = this.registerForm.value;

    if (formValue.Password !== formValue.confirmPassword) {
      alert("Passwords do not match");
      return;
    }

    const payload = {
      fullName: formValue.FullName,
      email: formValue.Email,
      password: formValue.Password,
      roleId: Number(formValue.RoleId)
    };

    console.log("Sending payload:", payload);

    this.authService.register(payload).subscribe({
      next: () => {
        alert("Registration successful");
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error(err);
        alert("Registration failed");
      }
    });
  }
}