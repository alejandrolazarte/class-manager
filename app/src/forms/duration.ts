import { twoDigits } from "@/forms/TimeColumn";

const secondsPerMinute = 60;
const minutesSeparator = ":";

export const minuteOptions = Array.from({ length: secondsPerMinute }, (_, minute) => minute);
export const secondOptions = Array.from({ length: secondsPerMinute }, (_, second) => second);

export function minutesOf(durationInSeconds: number): number {
  return Math.floor(durationInSeconds / secondsPerMinute);
}

export function secondsOf(durationInSeconds: number): number {
  return Math.round(durationInSeconds - minutesOf(durationInSeconds) * secondsPerMinute);
}

export function durationInSeconds(minutes: number, seconds: number): number {
  return minutes * secondsPerMinute + seconds;
}

export function formatDuration(durationInSeconds: number): string {
  return `${minutesOf(durationInSeconds)}${minutesSeparator}${twoDigits(secondsOf(durationInSeconds))}`;
}
