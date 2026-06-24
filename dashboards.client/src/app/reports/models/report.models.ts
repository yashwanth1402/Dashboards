export type ReportKey = 'vessel-activity' | 'activity-summary' | 'monthly-rate';

export interface ReportCategory {
  key: ReportKey;
  title: string;
  subtitle: string;
}

export interface ReportFilters {
  startDate: string;
  endDate: string;
  vessel: string;
  customer: string;
  search: string;
}

export interface VesselActivityPoint {
  date: string;
  dateValue: string;
  tonnage: number;
}

export interface DowntimeRow {
  vesselName: string;
  customer: string;
  startDate: string;
  endDate: string;
  totalTonnage: number;
  activeHours: number;
  efficiency: number;
}

export interface ActivityCategory {
  category: string;
  hours: number;
  percentage: number;
  trend: 'up' | 'down' | 'flat';
  color: string;
}

export interface DowntimeMetric {
  label: string;
  hours: number;
}

export interface MonthlyRatePoint {
  month: string;
  rate: number;
}

export interface MonthlyMetric {
  label: string;
  value: string;
  caption: string;
  tone: 'primary' | 'success' | 'info';
}

export interface ReportsData {
  categories: ReportCategory[];
  filters: {
    vessels: string[];
    customers: string[];
  };
  vesselActivity: {
    chart: VesselActivityPoint[];
    downtimeRows: DowntimeRow[];
  };
  activitySummary: {
    categories: ActivityCategory[];
    downtime: DowntimeMetric[];
  };
  monthlyRate: {
    trend: MonthlyRatePoint[];
    metrics: MonthlyMetric[];
  };
}
