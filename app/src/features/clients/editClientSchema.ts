import { z } from "zod";
import { formatPhoneNumberForDisplay } from "@/features/clients/phoneNumberFormatting";
import { clientContactSchema } from "@/features/clients/registerClientSchema";
import { Client, UpdateClientRequest } from "@/features/clients/types";

export const editClientSchema = clientContactSchema;

export type EditClientFormValues = z.input<typeof editClientSchema>;

export const editClientFieldNames = [
  "fullName",
  "phoneNumber",
  "email",
  "notes",
] as const satisfies readonly (keyof EditClientFormValues)[];

export const editClientRequiredFieldNames = [
  "fullName",
  "phoneNumber",
] as const satisfies readonly (keyof EditClientFormValues)[];

export function toEditClientFormValues(
  client: Client,
  signInEmail?: string | null,
): EditClientFormValues {
  return {
    fullName: client.fullName,
    phoneNumber: formatPhoneNumberForDisplay(client.phoneNumber),
    email: signInEmail ?? client.email ?? "",
    notes: client.notes ?? "",
  };
}

function trimToNull(value: string): string | null {
  const trimmedValue = value.trim();
  return trimmedValue.length === 0 ? null : trimmedValue;
}

export function toUpdateClientRequest(formValues: EditClientFormValues): UpdateClientRequest {
  return {
    fullName: formValues.fullName.trim(),
    phoneNumber: formValues.phoneNumber.trim(),
    email: trimToNull(formValues.email),
    notes: trimToNull(formValues.notes),
  };
}
