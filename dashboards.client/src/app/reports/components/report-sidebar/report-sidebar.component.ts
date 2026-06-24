import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ReportCategory, ReportKey } from '../../models/report.models';

@Component({
  selector: 'app-report-sidebar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './report-sidebar.component.html',
  styleUrl: './report-sidebar.component.css'
})
export class ReportSidebarComponent {
  @Input({ required: true }) categories: ReportCategory[] = [];
  @Input({ required: true }) activeReport: ReportKey = 'vessel-activity';
  @Output() reportChange = new EventEmitter<ReportKey>();
}
