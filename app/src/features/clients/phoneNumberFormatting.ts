const nonDigitPattern = /\D/g;
const internationalPrefix = "+";
const argentinaMobilePrefix = "+549";
const argentinaMobileLength = 14;
const areaCodeLength = 2;
const firstBlockEnd = 6;
const maximumDigits = 15;

export function countDigits(phoneNumber: string): number {
  return phoneNumber.replace(nonDigitPattern, "").length;
}

export function formatPhoneNumberAsTyped(typedText: string): string {
  const digits = typedText.replace(nonDigitPattern, "").slice(0, maximumDigits);
  if (typedText.trimStart().startsWith(internationalPrefix)) {
    return `${internationalPrefix}${digits}`;
  }
  if (digits.length <= areaCodeLength) {
    return digits;
  }
  const areaCode = digits.slice(0, areaCodeLength);
  const firstBlock = digits.slice(areaCodeLength, firstBlockEnd);
  const secondBlock = digits.slice(firstBlockEnd);
  return secondBlock.length > 0
    ? `${areaCode} ${firstBlock}-${secondBlock}`
    : `${areaCode} ${firstBlock}`;
}

export function formatPhoneNumberForDisplay(normalizedPhoneNumber: string): string {
  if (
    !normalizedPhoneNumber.startsWith(argentinaMobilePrefix) ||
    normalizedPhoneNumber.length !== argentinaMobileLength
  ) {
    return normalizedPhoneNumber;
  }
  const localNumber = normalizedPhoneNumber.slice(argentinaMobilePrefix.length);
  return `+54 9 ${formatPhoneNumberAsTyped(localNumber)}`;
}

export function toDialableDigits(normalizedPhoneNumber: string): string {
  return normalizedPhoneNumber.replace(nonDigitPattern, "");
}
