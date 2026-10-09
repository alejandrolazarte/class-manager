export interface TypedNumberPart {
  digitCount: number;
  minimum: number;
  maximum: number;
}

const nonDigitPattern = /\D/g;
const lowestDigit = "0";
const highestDigit = "9";

function canStillFit(partDigits: string, part: TypedNumberPart): boolean {
  const missingDigitCount = part.digitCount - partDigits.length;
  const smallestValue = Number(partDigits + lowestDigit.repeat(missingDigitCount));
  const largestValue = Number(partDigits + highestDigit.repeat(missingDigitCount));
  return smallestValue <= part.maximum && largestValue >= part.minimum;
}

export function acceptTypedNumberParts(
  typedText: string,
  parts: readonly TypedNumberPart[],
): string[] {
  const acceptedParts: string[] = [];
  let currentPartDigits = "";
  for (const typedDigit of typedText.replace(nonDigitPattern, "")) {
    const part = parts[acceptedParts.length];
    if (part === undefined) {
      break;
    }
    const candidateDigits = currentPartDigits + typedDigit;
    const paddedDigits = lowestDigit.repeat(part.digitCount - 1) + typedDigit;
    if (canStillFit(candidateDigits, part)) {
      currentPartDigits = candidateDigits;
    } else if (currentPartDigits === "" && canStillFit(paddedDigits, part)) {
      currentPartDigits = paddedDigits;
    } else {
      continue;
    }
    if (currentPartDigits.length === part.digitCount) {
      acceptedParts.push(currentPartDigits);
      currentPartDigits = "";
    }
  }
  return currentPartDigits === "" ? acceptedParts : [...acceptedParts, currentPartDigits];
}
