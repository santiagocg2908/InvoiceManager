import { DateOnly } from './date-only';

export interface UpdateInvoiceRequest {
  supplierName: string;
  invoiceNumber: string;
  issueDate: DateOnly;
  dueDate: DateOnly | null;
  totalAmount: number;
  currency: string;
  customerOrContactNumber: string | null;
}