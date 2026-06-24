import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

interface User {
  id: number;
  firstName: string;
  email: string;
}

interface UsersResponse {
  users: User[];
}

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
  employees: User[] = [];

  constructor(private readonly http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<UsersResponse>('/api/employees').subscribe({
      next: (response) => {
        this.employees = response.users;
      },
      error: (error: unknown) => {
        console.error(error);
      }
    });
  }
}
