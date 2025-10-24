import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  email = '';
  password = '';
  loading = false;
  error = '';

  constructor(private http: HttpClient, private router: Router) {}

  login() {
    this.loading = true;
    this.error = '';

    this.http
      .post<{ token: string }>('http://localhost:8080/api/auth/login', {
        email: this.email,
        password: this.password,
      })
      .subscribe({
        next: (res) => {
          localStorage.setItem('token', res.token);
          this.router.navigate(['/dashboard']);
        },
        error: () => {
          this.error = 'Credenciales incorrectas.';
          this.loading = false;
        },
      });
  }
}
