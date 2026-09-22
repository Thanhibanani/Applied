import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export type ApplicationStatus = 'New' | 'Approved' | 'Rejected' | 'Sent';

export interface JobApplication {
  id: number;
  matchScore: number;
  matchReasons: string[];
  coverLetter: string;
  status: ApplicationStatus;
  updatedAt: string;
  sentAt: string | null;
}

export interface Job {
  id: number;
  title: string;
  company: string;
  location: string;
  source: string;
  url: string;
  deadline: string | null;
  addedAt: string;
  application: JobApplication | null;
}

export interface CreateJobRequest {
  title: string;
  company: string;
  location: string;
  source: 'Finn' | 'LinkedIn' | 'Other';
  url: string;
  deadline?: string;
}

@Injectable({ providedIn: 'root' })
export class JobService {
  private readonly base = '/api/jobs';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Job[]> {
    return this.http.get<Job[]>(this.base);
  }

  create(request: CreateJobRequest): Observable<Job> {
    return this.http.post<Job>(this.base, request);
  }

  saveApplication(jobId: number, matchScore: number, matchReasons: string[], coverLetter: string): Observable<JobApplication> {
    return this.http.put<JobApplication>(`${this.base}/${jobId}/application`, { matchScore, matchReasons, coverLetter });
  }

  /** Approved -> Sent is the only path the API allows to reach "Sent". */
  setStatus(jobId: number, status: ApplicationStatus): Observable<JobApplication> {
    return this.http.post<JobApplication>(`${this.base}/${jobId}/application/status`, { status });
  }
}
