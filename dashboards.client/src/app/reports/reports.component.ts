import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ReportFilterPanelComponent } from './components/report-filter-panel/report-filter-panel.component';
import { ReportSidebarComponent } from './components/report-sidebar/report-sidebar.component';
import {
  ActivityCategory,
  DowntimeMetric,
  DowntimeRow,
  MonthlyRatePoint,
  ReportFilters,
  ReportKey,
  ReportsData,
  VesselActivityPoint
} from './models/report.models';
import { ReportsDataService } from './services/reports-data.service';

interface ChartTooltip {
  title: string;
  value: string;
  tone: 'primary' | 'success' | 'danger' | 'warning' | 'info';
}

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, ReportFilterPanelComponent, ReportSidebarComponent],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.css'
})
export class ReportsComponent {
  private readonly reportsDataService = inject(ReportsDataService);

  readonly data: ReportsData = this.reportsDataService.getReportsData();
  activeReport: ReportKey = 'vessel-activity';
  activeFilters: ReportFilters = {
    startDate: '2026-03-01',
    endDate: '2026-03-31',
    vessel: 'All Vessels',
    customer: 'All Customers',
    search: ''
  };
  filteredVesselChart: VesselActivityPoint[] = this.data.vesselActivity.chart;
  downtimeRows: DowntimeRow[] = this.data.vesselActivity.downtimeRows;
  selectedVesselDate = '';
  selectedActivityCategory = '';
  selectedDowntimeMetric = '';
  selectedMonth = '';
  chartTooltip: ChartTooltip | null = null;

  get reportTitle(): string {
    return this.data.categories.find((category) => category.key === this.activeReport)?.subtitle ?? 'Reports';
  }

  get maxTonnage(): number {
    return Math.max(...this.filteredVesselChart.map((point) => point.tonnage), 1);
  }

  get maxDowntimeHours(): number {
    return Math.max(...this.filteredDowntimeMetrics.map((item) => item.hours), 1);
  }

  get donutBackground(): string {
    let cursor = 0;
    const stops = this.data.activitySummary.categories.map((item) => {
      const start = cursor;
      cursor += item.percentage;
      return `${item.color} ${start}% ${cursor}%`;
    });

    return `conic-gradient(${stops.join(', ')})`;
  }

  get filteredActivityCategories(): ActivityCategory[] {
    const searchTerm = this.activeFilters.search.trim().toLowerCase();
    const categories = this.data.activitySummary.categories.filter(
      (item) => !searchTerm || item.category.toLowerCase().includes(searchTerm)
    );

    if (!this.selectedActivityCategory) {
      return categories;
    }

    return categories.filter((item) => item.category === this.selectedActivityCategory);
  }

  get filteredDowntimeMetrics(): DowntimeMetric[] {
    const searchTerm = this.activeFilters.search.trim().toLowerCase();
    const metrics = this.data.activitySummary.downtime.filter(
      (item) => !searchTerm || item.label.toLowerCase().includes(searchTerm)
    );

    if (!this.selectedDowntimeMetric) {
      return metrics;
    }

    return metrics.filter((item) => item.label === this.selectedDowntimeMetric);
  }

  get monthlyLinePoints(): string {
    const trend = this.data.monthlyRate.trend;
    const values = trend.map((point) => point.rate);
    const min = Math.min(...values);
    const max = Math.max(...values);
    const range = Math.max(max - min, 1);

    return trend
      .map((point, index) => {
        const x = 16 + (index / Math.max(trend.length - 1, 1)) * 468;
        const y = 156 - ((point.rate - min) / range) * 120;
        return `${x},${y}`;
      })
      .join(' ');
  }

  setActiveReport(report: ReportKey): void {
    this.activeReport = report;
    this.hideTooltip();
  }

  applyFilters(filters: ReportFilters): void {
    this.activeFilters = filters;
    this.filteredVesselChart = this.reportsDataService.filterVesselActivityChart(filters);
    this.refreshDowntimeRows();
  }

  print(): void {
    window.print();
  }

  tonnageHeight(point: VesselActivityPoint): number {
    return Math.max((point.tonnage / this.maxTonnage) * 100, 8);
  }

  downtimeWidth(item: DowntimeMetric): number {
    return Math.max((item.hours / this.maxDowntimeHours) * 100, 8);
  }

  trendLabel(category: ActivityCategory): string {
    if (category.trend === 'up') {
      return 'Trending up';
    }

    if (category.trend === 'down') {
      return 'Trending down';
    }

    return 'Stable';
  }

  showVesselTooltip(point: VesselActivityPoint): void {
    this.chartTooltip = {
      title: point.date,
      value: `${point.tonnage.toLocaleString()} MT`,
      tone: 'primary'
    };
  }

  showActivityTooltip(category: ActivityCategory): void {
    this.chartTooltip = {
      title: category.category,
      value: `${category.hours} hrs · ${category.percentage}%`,
      tone: category.trend === 'down' ? 'danger' : category.trend === 'up' ? 'success' : 'warning'
    };
  }

  showDowntimeTooltip(metric: DowntimeMetric): void {
    this.chartTooltip = {
      title: metric.label,
      value: `${metric.hours} hours`,
      tone: 'danger'
    };
  }

  showMonthlyTooltip(point: MonthlyRatePoint): void {
    this.chartTooltip = {
      title: point.month,
      value: `${point.rate.toLocaleString()} MT/Hr`,
      tone: 'info'
    };
  }

  hideTooltip(): void {
    this.chartTooltip = null;
  }

  selectVesselDate(point: VesselActivityPoint): void {
    this.selectedVesselDate = this.selectedVesselDate === point.dateValue ? '' : point.dateValue;
    this.refreshDowntimeRows();
  }

  selectActivityCategory(category: ActivityCategory): void {
    this.selectedActivityCategory =
      this.selectedActivityCategory === category.category ? '' : category.category;
  }

  selectDowntimeMetric(metric: DowntimeMetric): void {
    this.selectedDowntimeMetric = this.selectedDowntimeMetric === metric.label ? '' : metric.label;
  }

  selectMonth(point: MonthlyRatePoint): void {
    this.selectedMonth = this.selectedMonth === point.month ? '' : point.month;
  }

  monthlyPointY(point: MonthlyRatePoint): number {
    const values = this.data.monthlyRate.trend.map((trendPoint) => trendPoint.rate);
    const min = Math.min(...values);
    const max = Math.max(...values);
    const range = Math.max(max - min, 1);

    return 156 - ((point.rate - min) / range) * 120;
  }

  clearReportSelections(): void {
    this.selectedVesselDate = '';
    this.selectedActivityCategory = '';
    this.selectedDowntimeMetric = '';
    this.selectedMonth = '';
    this.refreshDowntimeRows();
  }

  private refreshDowntimeRows(): void {
    const rows = this.reportsDataService.filterDowntimeRows(this.activeFilters);

    this.downtimeRows = this.selectedVesselDate
      ? rows.filter((row) => this.toIsoDate(row.startDate) === this.selectedVesselDate)
      : rows;
  }

  private toIsoDate(displayDate: string): string {
    const [month, day, year] = displayDate.split('/').map(Number);
    return `${year}-${String(month).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
  }

  trackByLabel(_: number, item: { label?: string; category?: string; date?: string; month?: string }): string {
    return item.label ?? item.category ?? item.date ?? item.month ?? '';
  }
}
