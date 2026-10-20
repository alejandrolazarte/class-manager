import { acceptTypedNumberParts, TypedNumberPart } from "@/forms/typedNumberParts";

const typedBirthDatePattern = /^(\d{2})\/(\d{2})\/(\d{4})$/;
const isoDatePattern = /^(\d{4})-(\d{2})-(\d{2})$/;
const dateSeparator = "/";
const isoDateLength = 10;

export const earliestBirthDate = "1900-01-01";

const typedDateParts: readonly TypedNumberPart[] = [
  { digitCount: 2, minimum: 1, maximum: 31 },
  { digitCount: 2, minimum: 1, maximum: 12 },
  { digitCount: 4, minimum: 1900, maximum: 2099 },
];

export function formatBirthDateAsTyped(typedText: string): string {
  return acceptTypedNumberParts(typedText, typedDateParts).join(dateSeparator);
}

export function parseBirthDate(typedBirthDate: string): string | null {
  const match = typedBirthDatePattern.exec(typedBirthDate.trim());
  if (match === null) {
    return null;
  }
  const [, day, month, year] = match;
  const isoDate = `${year}-${month}-${day}`;
  const parsedDate = new Date(`${isoDate}T00:00:00Z`);
  return !Number.isNaN(parsedDate.getTime()) && toIsoDate(parsedDate) === isoDate ? isoDate : null;
}

export function isValidBirthDate(typedBirthDate: string, today: Date = new Date()): boolean {
  const isoDate = parseBirthDate(typedBirthDate);
  return isoDate !== null && isoDate >= earliestBirthDate && isoDate <= toLocalIsoDate(today);
}

export function formatBirthDateForDisplay(isoDate: string): string {
  const match = isoDatePattern.exec(isoDate);
  if (match === null) {
    return isoDate;
  }
  const [, year, month, day] = match;
  return [day, month, year].join(dateSeparator);
}

export function ageInYears(isoDate: string, today: Date = new Date()): number {
  const [year, month, day] = isoDate.split("-").map(Number);
  const hasHadBirthdayThisYear =
    today.getMonth() + 1 > month || (today.getMonth() + 1 === month && today.getDate() >= day);
  return today.getFullYear() - year - (hasHadBirthdayThisYear ? 0 : 1);
}

function toIsoDate(date: Date): string {
  return date.toISOString().slice(0, isoDateLength);
}

function toLocalIsoDate(date: Date): string {
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${date.getFullYear()}-${month}-${day}`;
}
