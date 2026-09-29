export type SessionKind = "team" | "family";

const familyKind: SessionKind = "family";
const defaultKind: SessionKind = "team";
const tokenSegmentSeparator = ".";
const payloadSegmentIndex = 1;
const base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
const bitsPerBase64Character = 6;
const bitsPerByte = 8;

function decodeBase64Url(encoded: string): string | null {
  const characters = encoded.replace(/-/g, "+").replace(/_/g, "/").replace(/=+$/, "");
  let bitBuffer = 0;
  let bitCount = 0;
  const bytes: number[] = [];
  for (const character of characters) {
    const value = base64Alphabet.indexOf(character);
    if (value < 0) {
      return null;
    }
    bitBuffer = (bitBuffer << bitsPerBase64Character) | value;
    bitCount += bitsPerBase64Character;
    if (bitCount >= bitsPerByte) {
      bitCount -= bitsPerByte;
      bytes.push((bitBuffer >> bitCount) & 0xff);
      bitBuffer &= (1 << bitCount) - 1;
    }
  }
  try {
    return decodeURIComponent(
      bytes.map((byte) => `%${byte.toString(16).padStart(2, "0")}`).join(""),
    );
  } catch {
    return null;
  }
}

export function sessionKindOf(accessToken: string): SessionKind {
  const payload = accessToken.split(tokenSegmentSeparator)[payloadSegmentIndex];
  const decodedPayload = payload === undefined ? null : decodeBase64Url(payload);
  if (decodedPayload === null) {
    return defaultKind;
  }
  try {
    const claims = JSON.parse(decodedPayload) as { kind?: unknown };
    return claims.kind === familyKind ? familyKind : defaultKind;
  } catch {
    return defaultKind;
  }
}
