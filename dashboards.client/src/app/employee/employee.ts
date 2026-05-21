import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-employee',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './employee.html',
  styleUrls: ['./employee.css']
})
export class EmployeeComponent implements OnInit {

  employees: any[] = [];

  constructor(private http: HttpClient) { }

  ngOnInit(): void {

    this.http
      .get<any>('https://localhost:7192/api/employees')
      .subscribe({
        next: (response) => {

          console.log(response);

          this.employees = response.users;

          console.log(this.employees);

        },
        error: (err) => {

          console.log(err);

        }
      });
  }
}
