import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface CvProfile {
  tagline: string;
  rawCvText: string;
  skills: string;
  location: string;
}

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [FormsModule],
  template: `
    <main style="max-width:640px;margin:0 auto;padding:28px 20px;">
      <h1>Profil</h1>
      <form (ngSubmit)="save()" style="display:flex;flex-direction:column;gap:10px;">
        <input name="tagline" [(ngModel)]="profile.tagline" placeholder="Kort tagline (f.eks. IT-konsulent · Oslo)" />
        <input name="location" [(ngModel)]="profile.location" placeholder="Sted" />
        <input name="skills" [(ngModel)]="profile.skills" placeholder="Ferdigheter, kommaseparert" />
        <textarea name="rawCvText" [(ngModel)]="profile.rawCvText" placeholder="Lim inn CV-tekst her" rows="10"></textarea>
        <button type="submit" class="primary">Lagre</button>
      </form>
    </main>
  `
})
export class ProfileComponent implements OnInit {
  profile: CvProfile = { tagline: '', rawCvText: '', skills: '', location: '' };

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<CvProfile>('/api/profile').subscribe({
      next: (p) => (this.profile = p),
      error: () => {} // 404 = no profile saved yet, keep the empty form
    });
  }

  save(): void {
    this.http.put<CvProfile>('/api/profile', this.profile).subscribe((p) => (this.profile = p));
  }
}
