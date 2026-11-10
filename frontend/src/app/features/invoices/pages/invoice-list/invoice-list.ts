import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { InvoiceService } from '../../services/invoice.service';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import type { InvoiceListItem } from '../../models/invoice-list-item.model';
import type { InvoiceListQuery } from '../../models/invoice-list-query.model';
import type { PagedResult } from '../../../../shared/models/paged-result.model';
import type { InvoiceSummary } from '../../models/invoice-summary.model';

@Component({
  selector: 'app-invoice-list',
  standalone: true,
  imports: [DecimalPipe, FormsModule, RouterLink],
  templateUrl: './invoice-list.html',
  styleUrl: './invoice-list.css'
})
export class InvoiceList implements OnInit {

  invoices: InvoiceListItem[] = [];

  isLoading = false;

  errorMessage: string | null = null;

  currentPage = 1;

  pageSize = 5;

  totalItems = 0;

  supplierSummaries: InvoiceSummary[] = [];

  showSupplierSummary: boolean = false;

  isSummaryLoading: boolean = false;

  summaryErrorMessage: string | null = null;

  supplierFilter = '';
  issueDateFromFilter = '';
  issueDateToFilter = '';

  constructor(
    private readonly invoiceService: InvoiceService,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadInvoices();
  }

  applyFilters(): void {
  this.currentPage = 1;
  this.loadInvoices();
}
    get totalPages(): number {
      return Math.ceil(this.totalItems / this.pageSize);
    }

    get hasPreviousPage(): boolean {
      return this.currentPage > 1;
    }

    get hasNextPage(): boolean {
      return this.currentPage < this.totalPages;
    }
  goToPreviousPage(): void {
    if (!this.hasPreviousPage || this.isLoading) {
      return;
    }

    this.currentPage--;
    this.loadInvoices();
  }

  goToNextPage(): void {
    if (!this.hasNextPage || this.isLoading) {
      return;
    }

    this.currentPage++;
    this.loadInvoices();
  }
  showInvoiceList(): void {
  this.showSupplierSummary = false;
  this.summaryErrorMessage = null;
}

 loadSupplierSummaries(): void {
  this.showSupplierSummary = true;
  this.isSummaryLoading = true;
  this.summaryErrorMessage = null;

  this.invoiceService.getInvoiceSummary()
    .pipe(
      finalize(() => {
        this.isSummaryLoading = false;
        this.cdr.markForCheck();
      })
    )
    .subscribe({
      next: (result) => {
        this.supplierSummaries = result;
        this.cdr.markForCheck();
      },
      error: (error: unknown) => {
        console.error('Error al cargar los resúmenes:', error);

        this.summaryErrorMessage =
          'No fue posible cargar los resúmenes por proveedor.';

        this.cdr.markForCheck();
      }
    });
} 

loadInvoices(): void {
  const query: InvoiceListQuery = {
    page: this.currentPage,
    pageSize: this.pageSize,
    supplier: this.supplierFilter.trim() || undefined,
    issueDateFrom: this.issueDateFromFilter || undefined,
    issueDateTo: this.issueDateToFilter || undefined
  };

  this.isLoading = true;
  this.errorMessage = null;

  this.invoiceService.getInvoices(query).subscribe({
next: (result: PagedResult<InvoiceListItem>) => {

    const invoice = result.items[0];

    console.log('Objeto recibido:', invoice);

    console.log('Propiedades disponibles:',
        Object.keys(invoice));

    console.log('totalAmount:',
        invoice.totalAmount);

    console.log('totalaAmount:',
        (invoice as any)['totalaAmount']);

    this.invoices = result.items;
    this.currentPage = result.page;
    this.pageSize = result.pageSize;
    this.totalItems = result.totalItems;
    this.isLoading = false;

    this.cdr.markForCheck();
},
    error: (error: unknown) => {
      console.error('Error al consultar facturas:', error);

      this.invoices = [];
      this.totalItems = 0;
      this.errorMessage = 'No fue posible consultar las facturas.';
      this.isLoading = false;
      this.cdr.markForCheck();
    }
  });
}
retryLoadInvoices(): void {
  if (this.isLoading) {
    return;
  }

  this.loadInvoices();
}

  
}
