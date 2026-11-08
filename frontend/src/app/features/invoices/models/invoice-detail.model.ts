import {DateOnly} from './date-only';

export interface InvoiceDetail{
    id: number;
    supplierName: string;
    invoiceNumber: string;
    issueDate: DateOnly;
    dueDate: DateOnly | null;
    totalAmount: number;
    currency: string;
    customerOrContactNumber: string | null;
    isActive: boolean;
}