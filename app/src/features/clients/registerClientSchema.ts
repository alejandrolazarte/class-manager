import { z } from "zod";
import { emailAddressSchema } from "@/forms/emailAddress";
import { countDigits } from "@/features/clients/phoneNumberFormatting";
import { RegisterClientRequest } from "@/features/clients/types";
import {
  isEmailOfAnotherPerson,
  normalizeStudentName,
  studentFormSchema,
  toNewStudentRequest,
} from "@/features/students/studentSchema";
import { NewStudentRequest } from "@/features/students/types";
import { translate } from "@/i18n/translate";

export const clientLimits = {
  fullNameMinimumLength: 2,
  fullNameMaximumLength: 120,
  phoneNumberMinimumDigits: 8,
  phoneNumberMaximumDigits: 15,
  emailMaximumLength: 254,
  notesMaximumLength: 1000,
} as const;

export const maximumStudentsPerRegistration = 10;

const phoneNumberAllowedCharactersPattern = /^[+\d\s()-]+$/;
const serverStudentFieldPattern = /^students\[(\d+)\]\.(\w+)$/;
const serverStudentsFieldName = "students";
const clientFieldNames = ["fullName", "phoneNumber", "email", "notes"] as const;
const studentFieldNames = ["fullName", "birthDate", "notes", "email"] as const;

export const clientContactSchema = z.object({
  fullName: z
    .string()
    .trim()
    .min(1, translate("clients.validation.fullNameRequired"))
    .min(clientLimits.fullNameMinimumLength, translate("clients.validation.fullNameTooShort"))
    .max(clientLimits.fullNameMaximumLength, translate("clients.validation.fullNameTooLong")),
  phoneNumber: z
    .string()
    .trim()
    .min(1, translate("clients.validation.phoneNumberRequired"))
    .regex(phoneNumberAllowedCharactersPattern, translate("clients.validation.phoneNumberInvalid"))
    .refine(
      (phoneNumber) =>
        countDigits(phoneNumber) >= clientLimits.phoneNumberMinimumDigits &&
        countDigits(phoneNumber) <= clientLimits.phoneNumberMaximumDigits,
      translate("clients.validation.phoneNumberInvalid"),
    ),
  email: z.union([
    z.literal(""),
    emailAddressSchema(translate("clients.validation.emailInvalid")).max(
      clientLimits.emailMaximumLength,
      translate("clients.validation.emailInvalid"),
    ),
  ]),
  notes: z
    .string()
    .max(clientLimits.notesMaximumLength, translate("clients.validation.notesTooLong")),
});

const clientFieldsSchema = clientContactSchema.extend({
  clientAttends: z.boolean(),
  sendAppInvitation: z.boolean(),
  additionalStudents: z.array(studentFormSchema),
});

export const registerClientSchema = clientFieldsSchema.superRefine((formValues, context) => {
  const attendeeCount = (formValues.clientAttends ? 1 : 0) + formValues.additionalStudents.length;
  if (attendeeCount === 0) {
    context.addIssue({
      code: "custom",
      path: ["clientAttends"],
      message: translate("clients.validation.attendeeRequired"),
    });
  }
  if (attendeeCount > maximumStudentsPerRegistration) {
    context.addIssue({
      code: "custom",
      path: ["clientAttends"],
      message: translate("clients.validation.tooManyAttendees"),
    });
  }
  const attendeeNames = new Set(
    formValues.clientAttends ? [normalizeStudentName(formValues.fullName)] : [],
  );
  formValues.additionalStudents.forEach((additionalStudent, index) => {
    const otherPeopleEmails = [
      formValues.email,
      ...formValues.additionalStudents
        .filter((_, otherIndex) => otherIndex < index)
        .map((otherStudent) => otherStudent.email),
    ];
    if (isEmailOfAnotherPerson(additionalStudent.email, otherPeopleEmails)) {
      context.addIssue({
        code: "custom",
        path: ["additionalStudents", index, "email"],
        message: translate("students.validation.emailOfAnotherPerson"),
      });
    }
    const attendeeName = normalizeStudentName(additionalStudent.fullName);
    if (attendeeName.length === 0) {
      return;
    }
    if (attendeeNames.has(attendeeName)) {
      context.addIssue({
        code: "custom",
        path: ["additionalStudents", index, "fullName"],
        message: translate("students.validation.duplicateName"),
      });
    }
    attendeeNames.add(attendeeName);
  });
});

export type RegisterClientFormValues = z.input<typeof registerClientSchema>;

export const emptyRegisterClientFormValues: RegisterClientFormValues = {
  fullName: "",
  phoneNumber: "",
  email: "",
  notes: "",
  clientAttends: true,
  sendAppInvitation: true,
  additionalStudents: [],
};

export type RegisterClientFieldName =
  | (typeof clientFieldNames)[number]
  | "clientAttends"
  | `additionalStudents.${number}.${(typeof studentFieldNames)[number]}`;

export function toRegisterClientFieldName(
  serverFieldName: string,
  clientAttends: boolean,
): RegisterClientFieldName | null {
  if (serverFieldName === serverStudentsFieldName) {
    return "clientAttends";
  }
  const studentFieldMatch = serverStudentFieldPattern.exec(serverFieldName);
  if (studentFieldMatch === null) {
    return (clientFieldNames as readonly string[]).includes(serverFieldName)
      ? (serverFieldName as RegisterClientFieldName)
      : null;
  }
  const studentIndex = Number(studentFieldMatch[1]);
  const studentFieldName = toCamelCase(studentFieldMatch[2] ?? "");
  if (!(studentFieldNames as readonly string[]).includes(studentFieldName)) {
    return null;
  }
  if (clientAttends && studentIndex === 0) {
    return studentFieldName === "fullName" ? "fullName" : null;
  }
  const additionalStudentIndex = clientAttends ? studentIndex - 1 : studentIndex;
  return `additionalStudents.${additionalStudentIndex}.${studentFieldName as (typeof studentFieldNames)[number]}`;
}

function toCamelCase(fieldName: string): string {
  return fieldName.charAt(0).toLowerCase() + fieldName.slice(1);
}

function toStudentRequests(formValues: RegisterClientFormValues): NewStudentRequest[] {
  const clientAsStudent: NewStudentRequest[] = formValues.clientAttends
    ? [{ fullName: formValues.fullName.trim(), birthDate: null, notes: null, email: null }]
    : [];
  return [...clientAsStudent, ...formValues.additionalStudents.map(toNewStudentRequest)];
}

export function toAppInvitationEmail(formValues: RegisterClientFormValues): string | null {
  const email = formValues.email.trim();
  return formValues.sendAppInvitation && email.length > 0 ? email : null;
}

export function toRegisterClientRequest(
  formValues: RegisterClientFormValues,
): RegisterClientRequest {
  const email = formValues.email.trim();
  const notes = formValues.notes.trim();
  return {
    fullName: formValues.fullName.trim(),
    phoneNumber: formValues.phoneNumber.trim(),
    ...(email.length > 0 ? { email } : {}),
    ...(notes.length > 0 ? { notes } : {}),
    students: toStudentRequests(formValues),
  };
}
