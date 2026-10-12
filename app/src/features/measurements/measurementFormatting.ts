import { parseIsoDate } from "@/features/sessions/dates";
import {
  ImprovementDirection,
  Measurement,
  MeasurementKind,
  MeasurementRecord,
} from "@/features/measurements/types";
import { translate, TranslationKey } from "@/i18n/translate";

const numberLocale = "es-AR";
const secondsUnit = "s";
const secondsPerMinute = 60;
const timeDecimals = 2;
const otherDecimals = 1;
const minusSign = "−";
const plusSign = "+";
const decimalComma = ",";
const decimalPoint = ".";
const whitespacePattern = /\s/g;
const typedNumberPattern = /^-?\d+([.,]\d+)?$/;
const minutesSeparator = ":";

export type MeasurementTone = "good" | "bad" | "neutral";

export interface MeasurementTrend {
  first: MeasurementRecord;
  last: MeasurementRecord;
  change: number;
  tone: MeasurementTone;
  isPersonalBest: boolean;
}

export const measurementKinds: readonly MeasurementKind[] = [
  "Time",
  "Weight",
  "Distance",
  "Length",
  "Repetitions",
  "Number",
];

export const improvementDirections: readonly ImprovementDirection[] = [
  "Increases",
  "Decreases",
  "Neither",
];

export const unitsByKind: Record<MeasurementKind, readonly string[]> = {
  Time: [secondsUnit],
  Weight: ["kg", "lb"],
  Distance: ["m", "km"],
  Length: ["cm"],
  Repetitions: ["rep"],
  Number: [],
};

const kindLabelKeys: Record<MeasurementKind, TranslationKey> = {
  Time: "measurements.kind.time",
  Weight: "measurements.kind.weight",
  Distance: "measurements.kind.distance",
  Length: "measurements.kind.length",
  Repetitions: "measurements.kind.repetitions",
  Number: "measurements.kind.number",
};

const directionLabelKeys: Record<ImprovementDirection, TranslationKey> = {
  Increases: "measurements.improvesWhen.increases",
  Decreases: "measurements.improvesWhen.decreases",
  Neither: "measurements.improvesWhen.neither",
};

const directionRuleKeys: Record<ImprovementDirection, TranslationKey> = {
  Increases: "measurements.rule.increases",
  Decreases: "measurements.rule.decreases",
  Neither: "measurements.rule.neither",
};

export function kindLabel(kind: MeasurementKind): string {
  return translate(kindLabelKeys[kind]);
}

export function directionLabel(direction: ImprovementDirection): string {
  return translate(directionLabelKeys[direction]);
}

export function measurementSummary(measurement: Measurement): string {
  return [
    kindLabel(measurement.kind),
    measurement.unit,
    translate(directionRuleKeys[measurement.improvesWhen]),
  ].join(" · ");
}

function formatNumber(value: number, decimals: number): string {
  const fractionDigits = Number.isInteger(value) ? 0 : decimals;
  return new Intl.NumberFormat(numberLocale, {
    minimumFractionDigits: fractionDigits,
    maximumFractionDigits: fractionDigits,
  }).format(value);
}

function formatTime(seconds: number): string {
  if (seconds < secondsPerMinute) {
    return `${formatNumber(seconds, timeDecimals)} ${secondsUnit}`;
  }
  const minutes = Math.floor(seconds / secondsPerMinute);
  const remainingSeconds = Math.round(seconds - minutes * secondsPerMinute);
  return `${minutes}:${String(remainingSeconds).padStart(2, "0")}`;
}

export function formatMeasurementValue(
  measurement: Pick<Measurement, "kind" | "unit">,
  value: number,
): string {
  if (measurement.kind === "Time") {
    return value < 0 ? `${minusSign}${formatTime(-value)}` : formatTime(value);
  }
  const formatted = formatNumber(Math.abs(value), otherDecimals);
  return `${value < 0 ? minusSign : ""}${formatted} ${measurement.unit}`;
}

export function formatMeasurementChange(
  measurement: Pick<Measurement, "kind" | "unit">,
  change: number,
): string {
  const sign = change > 0 ? plusSign : change < 0 ? minusSign : "";
  return `${sign}${formatMeasurementValue(measurement, Math.abs(change))}`;
}

function isBetter(direction: ImprovementDirection, candidate: number, reference: number): boolean {
  return direction === "Increases" ? candidate > reference : candidate < reference;
}

export function measurementTrend(
  measurement: Pick<Measurement, "improvesWhen">,
  records: readonly MeasurementRecord[],
): MeasurementTrend | null {
  if (records.length === 0) {
    return null;
  }
  const sorted = [...records].sort((first, second) =>
    first.measuredOn.localeCompare(second.measuredOn),
  );
  const first = sorted[0];
  const last = sorted[sorted.length - 1];
  const change = last.value - first.value;
  const isMeaningful = sorted.length > 1 && measurement.improvesWhen !== "Neither";
  const tone: MeasurementTone =
    !isMeaningful || change === 0
      ? "neutral"
      : isBetter(measurement.improvesWhen, last.value, first.value)
        ? "good"
        : "bad";
  const isPersonalBest =
    isMeaningful &&
    sorted
      .slice(0, -1)
      .every((record) => isBetter(measurement.improvesWhen, last.value, record.value));
  return { first, last, change, tone, isPersonalBest };
}

export function formatMeasurementDate(isoDate: string): string {
  const date = parseIsoDate(isoDate);
  return translate("measurements.date", {
    day: date.getDate(),
    month: translate(`months.${date.getMonth() + 1}` as TranslationKey),
  });
}

export function monthOfMeasurement(isoDate: string): string {
  return translate(`months.${parseIsoDate(isoDate).getMonth() + 1}` as TranslationKey);
}

export function parseMeasurementNumber(typedValue: string): number | null {
  const compact = typedValue.replace(whitespacePattern, "");
  if (!typedNumberPattern.test(compact)) {
    return null;
  }
  return Number(compact.replace(decimalComma, decimalPoint));
}

export function secondsFromTyped(typedMinutes: string, typedSeconds: string): number | null {
  const seconds = parseMeasurementNumber(typedSeconds);
  const minutes = typedMinutes.trim().length === 0 ? 0 : parseMeasurementNumber(typedMinutes);
  if (seconds === null || minutes === null || seconds < 0 || minutes < 0) {
    return null;
  }
  return minutes * secondsPerMinute + seconds;
}

export function measurementValueFromTyped(
  kind: MeasurementKind,
  typedValue: string,
): number | null {
  if (kind !== "Time" || !typedValue.includes(minutesSeparator)) {
    return parseMeasurementNumber(typedValue);
  }
  const [typedMinutes = "", typedSeconds = ""] = typedValue.split(minutesSeparator);
  return secondsFromTyped(typedMinutes, typedSeconds);
}

export function studentChangeText(
  measurement: Pick<Measurement, "kind" | "unit">,
  trend: MeasurementTrend,
): string | null {
  if (trend.tone !== "good") {
    return null;
  }
  return translate(trend.change < 0 ? "student.measurements.less" : "student.measurements.more", {
    amount: formatMeasurementValue(measurement, Math.abs(trend.change)),
    month: monthOfMeasurement(trend.first.measuredOn),
  });
}

export function goalProgress(
  measurement: Pick<Measurement, "improvesWhen">,
  trend: MeasurementTrend,
  goal: number,
): { ratio: number; remaining: number; isReached: boolean } {
  const isReached =
    measurement.improvesWhen === "Decreases" ? trend.last.value <= goal : trend.last.value >= goal;
  const span = goal - trend.first.value;
  const ratio = isReached ? 1 : span === 0 ? 0 : (trend.last.value - trend.first.value) / span;
  return {
    ratio: Math.max(0, Math.min(1, ratio)),
    remaining: Math.abs(goal - trend.last.value),
    isReached,
  };
}
