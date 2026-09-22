import { Component, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Job, JobService } from '../../core/job.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DatePipe],
  template: `
    <main style="max-width:880px;margin:0 auto;padding:28px 20px;">
      <header style="display:flex;justify-content:space-between;align-items:baseline;margin-bottom:20px;">
        <h1>Applied</h1>
        <span style="color:var(--ink-dim);font-size:0.85rem;">{{ jobs().length }} stillinger</span>
      </header>

      @if (jobs().length === 0) {
        <div style="border:1px dashed var(--border);border-radius:14px;padding:36px;text-align:center;color:var(--ink-dim);">
          Ingen stillinger ennå. Legg til en via API-et (<code>POST /api/jobs</code>) eller en "legg til"-dialog du bygger videre her.
        </div>
      }

      <div style="display:flex;flex-direction:column;gap:12px;">
        @for (job of jobs(); track job.id) {
          <article style="background:var(--surface);border:1px solid var(--border);border-radius:14px;padding:16px;">
            <div style="display:flex;justify-content:space-between;gap:12px;">
              <div>
                <h3 style="margin:0 0 2px;">{{ job.title }}</h3>
                <div style="color:var(--ink-dim);font-size:0.88rem;">{{ job.company }} · {{ job.location }}</div>
                <div style="color:var(--ink-dim);font-size:0.78rem;margin-top:4px;">
                  Lagt til {{ job.addedAt | date: 'short' }}
                  @if (job.deadline) { · Frist: {{ job.deadline }} }
                </div>
              </div>
              @if (job.application) {
                <div style="text-align:center;flex-shrink:0;">
                  <div style="font-size:1.1rem;font-weight:700;">{{ job.application.matchScore }}</div>
                  <div style="font-size:0.65rem;color:var(--ink-dim);text-transform:uppercase;">match</div>
                </div>
              }
            </div>

            @if (job.application; as app) {
              <div style="margin-top:10px;display:flex;justify-content:space-between;align-items:center;">
                <span style="font-size:0.75rem;font-weight:600;padding:3px 9px;border-radius:999px;background:var(--bg);">
                  {{ statusLabel(app.status) }}
                </span>
                <div style="display:flex;gap:8px;">
                  @if (app.status === 'New') {
                    <button class="primary" (click)="setStatus(job, 'Approved')">Godkjenn</button>
                    <button (click)="setStatus(job, 'Rejected')">Avvis</button>
                  }
                  @if (app.status === 'Approved') {
                    <button (click)="setStatus(job, 'Sent')">Merk som sendt</button>
                  }
                </div>
              </div>
            }
          </article>
        }
      </div>
    </main>
  `
})
export class DashboardComponent implements OnInit {
  jobs = signal<Job[]>([]);

  constructor(private jobService: JobService) {}

  ngOnInit(): void {
    this.jobService.getAll().subscribe((jobs) => this.jobs.set(jobs));
  }

  setStatus(job: Job, status: 'Approved' | 'Rejected' | 'Sent'): void {
    this.jobService.setStatus(job.id, status).subscribe((updated) => {
      this.jobs.update((list) => list.map((j) => (j.id === job.id ? { ...j, application: updated } : j)));
    });
  }

  statusLabel(status: string): string {
    return { New: 'Ny', Approved: 'Godkjent', Rejected: 'Avvist', Sent: 'Sendt' }[status] ?? status;
  }
}
