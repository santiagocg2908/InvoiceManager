
import { ChangeDetectorRef,Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InvoiceService } from '../../services/invoice.service';
import type { InvoiceDetail } from '../../models/invoice-detail.model';

@Component({
  selector: 'app-invoice-detail',
  standalone: true,
  imports: [],
  templateUrl: './invoice-detail.html',
  styleUrl: './invoice-detail.css'
})
export class InvoiceDetailComponent implements OnInit {

  invoice: InvoiceDetail | null = null;

  isLoading = false;
  errorMessage: string | null = null;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly invoiceService: InvoiceService,
    private readonly cdRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!Number.isInteger(id) || id <= 0) {
      this.errorMessage = 'El identificador de la factura no es válido.';
      return;
    }

    this.loadInvoice(id);
  }

  loadInvoice(id: number): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.invoiceService.getInvoiceById(id).subscribe({
      next: (result: InvoiceDetail) => {
        this.invoice = result;
        this.isLoading = false;
        this.cdRef.detectChanges();
      },
      error: (error: unknown) => {
        console.error('Error al consultar la factura:', error);

        this.invoice = null;
        this.errorMessage =
          'No fue posible consultar la factura. Verifica que exista.';
        this.isLoading = false;
        this.cdRef.detectChanges();
      }
    });
  }
}
