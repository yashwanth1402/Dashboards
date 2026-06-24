import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { DashboardService, EmployeeDashboardData } from './dashboard.service';

interface MonthlyJoiningChartItem {
  label: string;
  count: number;
  height: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  dashboardData: EmployeeDashboardData | null = null;
  monthlyJoiningChart: MonthlyJoiningChartItem[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(private readonly dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getEmployeeDashboard().subscribe({
      next: (data) => {
        this.dashboardData = data;
       this.monthlyJoiningChart = this.buildMonthlyJoiningChart(data);
        this.isLoading = false;
        console.log(data.employees);
      },
      error: () => {
        this.errorMessage = 'Unable to load employee dashboard data.';
        this.isLoading = false;
      }
    });
  }

  get inactiveEmployees(): number {
    if (!this.dashboardData) {
      return 0;
    }

    return this.dashboardData.totalEmployees - this.dashboardData.activeEmployees;
  }

  private buildMonthlyJoiningChart(data: EmployeeDashboardData): MonthlyJoiningChartItem[] {
    const monthCounts = new Map<string, number>();

    for (const employee of data.employees) {

      if (!employee.joiningDate) {
        continue;
      }

      const joiningDate = new Date(employee.joiningDate);

      if (Number.isNaN(joiningDate.getTime())) {
        continue;
      }

      const label = joiningDate.toLocaleString('en-US', {
        month: 'short'
      });

      monthCounts.set(label, (monthCounts.get(label) ?? 0) + 1);
    }

    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    const maxCount = Math.max(...months.map((month) => monthCounts.get(month) ?? 0), 1);

    return months.map((month) => {
      const count = monthCounts.get(month) ?? 0;

      return {
        label: month,
        count,
        height: count === 0 ? 4 : Math.max((count / maxCount) * 100, 12)
      };
    });
  }
}
