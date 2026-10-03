export type ImportModule = "students" | "instructors";

export type ImportColumnType = "Text" | "Date" | "Email" | "Phone";

export interface ImportColumn {
  key: string;
  header: string;
  type: ImportColumnType;
  isRequired: boolean;
  aliases: string[];
  example: string | null;
}

export interface ImportSchema {
  module: ImportModule;
  columns: ImportColumn[];
}

export interface MappedHeader {
  header: string;
  key: string | null;
}

export interface ImportCellError {
  key: string;
  code: string;
  message: string;
}

export type ImportRowStatus = "Valid" | "Error" | "Skipped";

export interface ImportRowResult {
  line: number;
  status: ImportRowStatus;
  errors: ImportCellError[];
}

export interface ImportSummary {
  total: number;
  valid: number;
  errors: number;
  skipped: number;
}

export interface ImportReport {
  module: ImportModule;
  columns: MappedHeader[];
  summary: ImportSummary;
  rows: ImportRowResult[];
  planLimit?: ImportPlanLimit | null;
}

export interface ImportPlanLimit {
  featureCode: string;
  remaining: number;
}
