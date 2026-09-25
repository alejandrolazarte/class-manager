const startTimeDigitsLength = 4;
const hourDigitsEnd = 2;
const nonDigitPattern = /\D/g;
const timeSeparator = ":";
const minutesPerHour = 60;

export const startTimePattern = /^([01]\d|2[0-3]):[0-5]\d$/;
export const minutesPerDay = 24 * minutesPerHour;

export function formatStartTimeAsTyped(typedText: string): string {
  const digits = typedText.replace(nonDigitPattern, "").slice(0, startTimeDigitsLength);
  return digits.length > hourDigitsEnd
    ? `${digits.slice(0, hourDigitsEnd)}${timeSeparator}${digits.slice(hourDigitsEnd)}`
    : digits;
}

export function toMinutesOfDay(startTime: string): number {
  const [hours, minutes] = startTime.split(timeSeparator).map(Number);
  return (hours ?? 0) * minutesPerHour + (minutes ?? 0);
}
