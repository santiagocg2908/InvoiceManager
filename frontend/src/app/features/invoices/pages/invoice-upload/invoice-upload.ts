import { HttpErrorResponse} from '@angular/common/http';
import { finalize } from 'rxjs';
import { InvoiceService } from '../../services/invoice.service';

import { ChangeDetectorRef, Component, OnDestroy } from '@angular/core';

import type { ApiProblemDetails } from '../../../../shared/models/api-problem-details.model';
import type { InvoiceUploadRequest } from '../../models/invoice-upload-request.model';

@Component({
  imports: [],
  selector: 'app-invoice-upload',
  styleUrl: './invoice-upload.css',
  templateUrl: './invoice-upload.html',
})
export class InvoiceUpload implements OnDestroy {

  constructor(private readonly invoiceService: InvoiceService,
              private readonly cdr: ChangeDetectorRef) 
  {}

  selectedFile: File | null = null;

  previewUrl: string | null = null;

  errorMessage: string | null = null;

  successMessage: string | null = null;

  isUploading: boolean = false;

  private readonly allowedFiles = [
    { extension: '.pdf', mimeType: 'application/pdf' },
    { extension: '.png', mimeType: 'image/png' },
    { extension: '.jpg', mimeType: 'image/jpeg' },
    { extension: '.jpeg', mimeType: 'image/jpeg' }
  ];

  private readonly maxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
  onFileSelected(event: Event): void {
    if (this.isUploading){
      return;
    }
    const input = event.target;

    if (!(input instanceof HTMLInputElement)) {
      return;
    }

    const file = input.files?.[0];

    this.resetSelection();
    this.errorMessage = null;

    this.successMessage = null;

    if (!file) {
      return;
    }

    const fileName = file.name.toLowerCase();
    const mimeType = file.type.toLowerCase();

    const isValid = this.allowedFiles.some(
      allowed =>
        fileName.endsWith(allowed.extension) &&
        mimeType === allowed.mimeType
    );

    if (!isValid) {

      this.errorMessage =
        'Formato no permitido. Selecciona un documento PDF, PNG o JPG.';

      input.value = '';

      return;
    }

    if(file.size < 0) {
      this.errorMessage = 'El archivo no puede estar vacío.'; 
    }

    if (file.size > this.maxFileSizeBytes) {
      this.errorMessage =
      `El documento supera el tamaño máximo permitido de ${
        this.formatFileSize(this.maxFileSizeBytes)
      }.`;
        input.value = '';
      return;
    }

    this.selectedFile = file;

    // Generar vista previa únicamente para imágenes.
    if (mimeType === 'image/png' || mimeType === 'image/jpeg') {

      this.previewUrl = URL.createObjectURL(file);

    }
  }

uploadSelectedFile(): void {
  if (!this.selectedFile || this.isUploading) return;

  const request: InvoiceUploadRequest = {
    file: this.selectedFile
  };

  this.isUploading = true;
  this.errorMessage = null;
  this.successMessage = null;

  this.invoiceService.uploadInvoice(request)
    .pipe(
      finalize(() => {
        this.isUploading = false;

        // Notificar a Angular que debe actualizar la vista.
        this.cdr.markForCheck();
      })
    )
    .subscribe({
      next: (result) => {
        console.log('Factura registrada:', result);

        this.successMessage = 'Factura registrada exitosamente.';

        this.cdr.markForCheck();
      },
      error: (error: unknown) => {
        this.errorMessage = this.getErrorMessage(error);

        this.cdr.markForCheck();
      }
    });
}
  private getErrorMessage(error: unknown): string {
      if (error instanceof HttpErrorResponse) {

    const problem = error.error as
      Partial<ApiProblemDetails> | null;

    if (problem && typeof problem === 'object') {

      if (problem.detail?.trim()) {
        return problem.detail;
      }

      if (problem.title?.trim()) {
        return problem.title;
      }

      if (problem.errors) {

        const messages = Object.values(problem.errors).flat();

        if (messages.length > 0) {
          return messages.join(' ');
        }
      }
    }

    if (error.status === 0) {
      return 'No fue posible conectar con el servidor.';
    }

    return `No se pudo procesar el documento. Código HTTP: ${error.status}.`;
  }

  return 'Ocurrió un error inesperado al procesar el documento.';
  }
  removeFile(input: HTMLInputElement): void {

    if(this.isUploading){
      return;
    }

    this.resetSelection();

    this.errorMessage = null;
    this.successMessage = null;

    input.value = '';
  }

  formatFileSize(sizeBytes: number): string {

    if (sizeBytes < 1024) {
      return `${sizeBytes} B`;
    }

    if (sizeBytes < 1024 * 1024) {
      return `${(sizeBytes / 1024).toFixed(2)} KB`;
    }

    return `${(sizeBytes / (1024 * 1024)).toFixed(2)} MB`;
  }

  private resetSelection(): void {

    if (this.previewUrl) {
      URL.revokeObjectURL(this.previewUrl);
    }

    this.previewUrl = null;

    this.selectedFile = null;
  }

  ngOnDestroy(): void {
    this.resetSelection();
  }
}
