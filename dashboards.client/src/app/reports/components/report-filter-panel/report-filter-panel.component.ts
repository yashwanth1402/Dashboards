import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { ReportFilters } from '../../models/report.models';

@Component({
  selector: 'app-report-filter-panel',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './report-filter-panel.component.html',
  styleUrl: './report-filter-panel.component.css'
})
export class ReportFilterPanelComponent {
  private readonly formBuilder = inject(FormBuilder);

  @Input({ required: true }) vessels: string[] = [];
  @Input({ required: true }) customers: string[] = [];
  @Output() filtersApplied = new EventEmitter<ReportFilters>();

  readonly form = this.formBuilder.nonNullable.group({
    startDate: '2026-03-01',
    endDate: '2026-03-31',
    vessel: 'All Vessels',
    customer: 'All Customers',
    search: ''
  });

  constructor() {
    this.form.valueChanges.pipe(debounceTime(120), takeUntilDestroyed()).subscribe(() => {
      this.applyFilters();
    });
  }

  applyFilters(): void {
    this.filtersApplied.emit(this.form.getRawValue());
  }
}
