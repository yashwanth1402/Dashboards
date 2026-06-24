import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';

export interface Employee {
  id: number;
  name: string;
  email: string;
  isActive: boolean;
  joiningDate: Date | null;
}

export interface EmployeeDashboardData {
  totalEmployees: number;
  activeEmployees: number;
  employees: Employee[];
}

interface EmployeesApiResponse {
  users?: ApiEmployee[];
}

interface ApiEmployee {
  id?: number;
  firstName?: string;
  lastName?: string;
  name?: string;
  email?: string;
  active?: boolean;
  isActive?: boolean;
  status?: string;
  joiningDate?: string;
  joinedDate?: string;
  hireDate?: string;
  startDate?: string;
  createdAt?: string;
  birthDate?: string;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly employeesUrl = '/api/employees';

  constructor(private readonly http: HttpClient) {}

  getEmployeeDashboard(): Observable<EmployeeDashboardData> {
    return this.http.get<EmployeesApiResponse | ApiEmployee[]>(this.employeesUrl).pipe(
      map((response) => {
        const apiEmployees = Array.isArray(response) ? response : response.users ?? [];
        const employees = apiEmployees.map((employee, index) => this.mapEmployee(employee, index));

        return {
          totalEmployees: employees.length,
          activeEmployees: employees.filter((employee) => employee.isActive).length,
          employees
        };
      })
    );
  }

  private mapEmployee(employee: ApiEmployee, index: number): Employee {
    const firstName = employee.firstName?.trim() ?? '';
    const lastName = employee.lastName?.trim() ?? '';
    const name = employee.name?.trim() || `${firstName} ${lastName}`.trim() || 'Employee';

    return {
      id: employee.id ?? index + 1,
      name,
      email: employee.email ?? '',
      isActive: this.mapActiveStatus(employee),
      joiningDate: this.mapJoiningDate(employee)
    };
  }

  private mapActiveStatus(employee: ApiEmployee): boolean {
    if (typeof employee.isActive === 'boolean') {
      return employee.isActive;
    }

    if (typeof employee.active === 'boolean') {
      return employee.active;
    }

    if (employee.status) {
      return employee.status.toLowerCase() === 'active';
    }

    return true;
  }

  private mapJoiningDate(employee: ApiEmployee): Date | null {
    const dateValue =
      employee.joiningDate ??
      employee.joinedDate ??
      employee.hireDate ??
      employee.startDate ??
      employee.createdAt ??
      employee.birthDate;

    if (!dateValue) {
      return null;
    }

    const parsedDate = new Date(dateValue);
    return Number.isNaN(parsedDate.getTime()) ? null : parsedDate;
  }
}
