import { DateOnly } from "./date-only";

export interface InvoiceListItem{
    id: number;
    supplierName: string;
    invoiceNumber: string;
    issueDate: DateOnly;
    dueDate: DateOnly | null;
    totalAmount: number;
    currency: string;
    isActive: boolean;
}