import { acceptTypedNumberParts, TypedNumberPart } from "@/forms/typedNumberParts";

const timeSeparator = ":";
const minutesPerHour = 60;

export const startTimePattern = /^([01]\d|2[0-3]):[0-5]\d$/;
export const minutesPerDay = 24 * minutesPerHour;

const startTimeParts: readonly TypedNumberPart[] = [
  { digitCount: 2, minimum: 0, maximum: 23 },
  { digitCount: 2, minimum: 0, maximum: 59 },
];

export function formatStartTimeAsTyped(typedText: string): string {
  return acceptTypedNumberParts(typedText, startTimeParts).join(timeSeparator);
}

export function toMinutesOfDay(startTime: string): number {
  const [hours, minutes] = startTime.split(timeSeparator).map(Number);
  return (hours ?? 0) * minutesPerHour + (minutes ?? 0);
}
