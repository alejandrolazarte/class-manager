import { PickedCsvFile } from "@/features/importExport/pickCsvFile";
import { ImportReport, ImportSchema } from "@/features/importExport/types";

export const pickedStudentsFile: PickedCsvFile = {
  name: "alumnos.csv",
  uri: "file:///cache/alumnos.csv",
};

export function buildStudentsSchema(): ImportSchema {
  return {
    module: "students",
    columns: [
      {
        key: "studentName",
        header: "Alumno",
        type: "Text",
        isRequired: true,
        aliases: [],
        example: "Lucas Gómez",
      },
      {
        key: "phone",
        header: "Teléfono",
        type: "Phone",
        isRequired: true,
        aliases: [],
        example: "611 222 333",
      },
      {
        key: "contactName",
        header: "Responsable",
        type: "Text",
        isRequired: false,
        aliases: [],
        example: null,
      },
    ],
  };
}

export function buildInstructorsSchema(): ImportSchema {
  return {
    module: "instructors",
    columns: [
      {
        key: "fullName",
        header: "Profesor",
        type: "Text",
        isRequired: true,
        aliases: [],
        example: null,
      },
    ],
  };
}

export function buildStudentsReport(): ImportReport {
  return {
    module: "students",
    columns: [
      { header: "Alumno", key: "studentName" },
      { header: "Tel", key: "phone" },
      { header: "Grupo", key: null },
    ],
    summary: { total: 3, valid: 1, errors: 1, skipped: 1 },
    rows: [
      { line: 2, status: "Valid", errors: [] },
      {
        line: 3,
        status: "Error",
        errors: [{ key: "phone", code: "validation", message: "Phone number is required." }],
      },
      {
        line: 4,
        status: "Skipped",
        errors: [{ key: "studentName", code: "student.already_registered", message: "Exists." }],
      },
    ],
  };
}
