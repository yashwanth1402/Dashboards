import { Injectable } from '@angular/core';
import reportsData from '../data/reports.data.json';
import { DowntimeRow, ReportFilters, ReportsData, VesselActivityPoint } from '../models/report.models';

@Injectable({
  providedIn: 'root'
})
export class ReportsDataService {
  private readonly data = reportsData as ReportsData;

  getReportsData(): ReportsData {
    return this.data;
  }

  filterDowntimeRows(filters: ReportFilters): DowntimeRow[] {
    return this.data.vesselActivity.downtimeRows.filter((row) => {
      const matchesVessel = filters.vessel === 'All Vessels' || row.vesselName === filters.vessel;
      const matchesCustomer = filters.customer === 'All Customers' || row.customer === filters.customer;
      const matchesDate = this.isInDateRange(row.startDate, filters.startDate, filters.endDate);
      const searchTerm = filters.search.trim().toLowerCase();
      const matchesSearch =
        !searchTerm ||
        row.vesselName.toLowerCase().includes(searchTerm) ||
        row.customer.toLowerCase().includes(searchTerm);

      return matchesVessel && matchesCustomer && matchesDate && matchesSearch;
    });
  }

  filterVesselActivityChart(filters: ReportFilters): VesselActivityPoint[] {
    return this.data.vesselActivity.chart.filter((point) =>
      this.isIsoDateInRange(point.dateValue, filters.startDate, filters.endDate)
    );
  }

  private isInDateRange(displayDate: string, startDate: string, endDate: string): boolean {
    const [month, day, year] = displayDate.split('/').map(Number);
    const isoDate = `${year}-${String(month).padStart(2, '0')}-${String(day).padStart(2, '0')}`;

    return this.isIsoDateInRange(isoDate, startDate, endDate);
  }

  private isIsoDateInRange(value: string, startDate: string, endDate: string): boolean {
    if (!startDate || !endDate) {
      return true;
    }

    return value >= startDate && value <= endDate;
  }
}
