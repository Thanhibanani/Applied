import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  template: `
    <main style="max-width:360px;margin:80px auto;padding:0 16px;">
      <h1>Applied</h1>
      <p style="color:var(--ink-dim)">{{ mode() === 'login' ? 'Logg inn' : 'Opprett konto' }}</p>

      <form (ngSubmit)="submit()" style="display:flex;flex-direction:column;gap:10px;">
        @if (mode() === 'register') {
          <input name="fullName" [(ngModel)]="fullName" placeholder="Fullt navn" required />
        }
        <input name="email" type="email" [(ngModel)]="email" placeholder="E-post" required />
        <input name="password" type="password" [(ngModel)]="password" placeholder="Passord" required minlength="8" />
        <button type="submit" class="primary">{{ mode() === 'login' ? 'Logg inn' : 'Registrer' }}</button>
      </form>

      @if (error()) {
        <p style="color:var(--bad);font-size:0.85rem;">{{ error() }}</p>
      }

      <button type="button" style="margin-top:12px;" (click)="toggleMode()">
        {{ mode() === 'login' ? 'Har ikke konto? Registrer deg' : 'Har konto? Logg inn' }}
      </button>
    </main>
  `
})
export class LoginComponent {
  mode = signal<'login' | 'register'>('login');
  email = '';
  password = '';
  fullName = '';
  error = signal<string | null>(null);

  constructor(private auth: AuthService, private router: Router) {}

  toggleMode(): void {
    this.mode.set(this.mode() === 'login' ? 'register' : 'login');
    this.error.set(null);
  }

  submit(): void {
    this.error.set(null);
    const request$ =
      this.mode() === 'login'
        ? this.auth.login(this.email, this.password)
        : this.auth.register(this.email, this.password, this.fullName);

    request$.subscribe({
      next: () => this.router.navigateByUrl('/dashboard'),
      error: (err) => this.error.set(err?.error?.message ?? 'Noe gikk galt. Prøv igjen.')
    });
  }
}
