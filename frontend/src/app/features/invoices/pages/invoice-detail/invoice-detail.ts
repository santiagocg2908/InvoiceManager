
import { ChangeDetectorRef,Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { InvoiceService } from '../../services/invoice.service';
import {AbstractControl, ValidationErrors, ValidatorFn} from '@angular/forms'; 
import { RouterLink } from '@angular/router';
import type { InvoiceDetail } from '../../models/invoice-detail.model';
import type { UpdateInvoiceRequest } from '../../models/update-invoice-request.model';
import { ApiProblemDetails } from '../../../../shared/models/api-problem-details.model';
import { HttpErrorResponse } from '@angular/common/http';

const dateRangeValidator: ValidatorFn = (
  control: AbstractControl
): ValidationErrors | null => {

  const issueDate = control.get('issueDate')?.value;
  const dueDate = control.get('dueDate')?.value;

  if (!issueDate || !dueDate) {
    return null;
  }

  return dueDate < issueDate
    ? { invalidDateRange: true }
    : null;
};

@Component({
  selector: 'app-invoice-detail',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './invoice-detail.html',
  styleUrl: './invoice-detail.css'
})
export class InvoiceDetailComponent implements OnInit {

  invoice: InvoiceDetail | null = null;
  isLoading = false;
  errorMessage: string | null = null;
  invoiceForm: FormGroup;
  isSaving = false;
  successMessage: string | null = null;

    constructor(
      private readonly route: ActivatedRoute,
      private readonly invoiceService: InvoiceService,
      private readonly cdRef: ChangeDetectorRef,
      private readonly formBuilder: FormBuilder
    ) {
      this.invoiceForm = this.formBuilder.group(
        {
          supplierName: ['', [
            Validators.required,
            Validators.maxLength(200)
          ]],

          invoiceNumber: ['', [
            Validators.required,
            Validators.maxLength(100)
          ]],

          issueDate: ['', Validators.required],

          dueDate: [''],

          totalAmount: [null, [
            Validators.required,
            Validators.min(0.01),
            Validators.max(9999999999999999.99)
          ]],

          currency: ['', [
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(3)
          ]],

          customerOrContactNumber: ['', [
            Validators.maxLength(100)
          ]]
        },
        {
          validators: dateRangeValidator
        }
      );
    }
  

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

      this.invoiceForm.patchValue({
        supplierName: result.supplierName,
        invoiceNumber: result.invoiceNumber,
        issueDate: result.issueDate,
        dueDate: result.dueDate ?? '',
        totalAmount: result.totalAmount,
        currency: result.currency,
        customerOrContactNumber:
          result.customerOrContactNumber ?? ''
      });

      this.isLoading = false;
      this.cdRef.markForCheck();
    },
    error: (error: unknown) => {
      console.error('Error al consultar la factura:', error);

      this.invoice = null;
      this.errorMessage =
        'No fue posible consultar la factura. Verifica que exista.';

      this.isLoading = false;
      this.cdRef.markForCheck();
    }
  });
}

hasError(controlName: string, errorName: string): boolean {
  const control = this.invoiceForm.get(controlName);

  return !!control &&
    control.hasError(errorName) &&
    (control.touched || control.dirty);
}


saveChanges(): void {
  if (this.invoiceForm.invalid || this.isSaving || !this.invoice) {
    this.invoiceForm.markAllAsTouched();
    return;
  }

  const request: UpdateInvoiceRequest = {
    supplierName: this.invoiceForm.value.supplierName.trim(),
    invoiceNumber: this.invoiceForm.value.invoiceNumber.trim(),
    issueDate: this.invoiceForm.value.issueDate,
    dueDate: this.invoiceForm.value.dueDate || null,
    totalAmount: Number(this.invoiceForm.value.totalAmount),
    currency: this.invoiceForm.value.currency.trim().toUpperCase(),
    customerOrContactNumber:
      this.invoiceForm.value.customerOrContactNumber || null
  };

  this.isSaving = true;
  this.errorMessage = null;
  this.successMessage = null;

  this.invoiceService.updateInvoice(this.invoice.id, request)
    .subscribe({
      next: (result: InvoiceDetail) => {
        this.invoice = result;

        this.invoiceForm.patchValue({
          supplierName: result.supplierName,
          invoiceNumber: result.invoiceNumber,
          issueDate: result.issueDate,
          dueDate: result.dueDate ?? '',
          totalAmount: result.totalAmount,
          currency: result.currency,
          customerOrContactNumber:
            result.customerOrContactNumber ?? ''
        });

        this.successMessage = 'Factura actualizada correctamente.';
        this.isSaving = false;
        this.cdRef.markForCheck();
      },
      error: (error: unknown) => {
        console.error('Error al actualizar la factura:', error);

        this.errorMessage = this.getUpdateErrorMessage(error);
        this.isSaving = false;
        this.cdRef.markForCheck();
      }
    });
}
private getUpdateErrorMessage(error: unknown): string {

  if (!(error instanceof HttpErrorResponse)) {
    return 'Ocurrió un error inesperado al actualizar la factura.';
  }

  if (error.status === 0) {
    return 'No fue posible conectar con el servidor.';
  }

  if (error.status === 404) {
    return 'La factura que intentas actualizar no existe.';
  }

  if (error.status === 409) {
    return 'Ya existe una factura con ese número para el proveedor indicado.';
  }

  const problem = error.error as Partial<ApiProblemDetails>;

  if (problem.detail) {
    return problem.detail;
  }

  if (problem.title) {
    return problem.title;
  }

  return 'No fue posible actualizar la factura. Revisa los datos e inténtalo nuevamente.';
}


}
