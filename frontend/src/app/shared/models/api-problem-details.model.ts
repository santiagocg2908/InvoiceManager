export interface ApiProblemDetails{
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    instace?: string;
    traceId?: string;
    errors?: Record<string, string[]>;
}