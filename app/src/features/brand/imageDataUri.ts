const base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
const base64Padding = "=";
const bytesPerGroup = 3;
const sextetMask = 0x3f;
const pngContentType = "image/png";
const jpegContentType = "image/jpeg";
const webpContentType = "image/webp";
const jpegFirstByte = 0xff;
const jpegSecondByte = 0xd8;
const riffLetterR = 0x52;
const webpSignatureOffset = 8;
const webpLetterW = 0x57;

function toBase64(bytes: Uint8Array): string {
  let encoded = "";
  for (let groupStart = 0; groupStart < bytes.length; groupStart += bytesPerGroup) {
    const first = bytes[groupStart]!;
    const second = bytes[groupStart + 1];
    const third = bytes[groupStart + 2];
    const group = (first << 16) | ((second ?? 0) << 8) | (third ?? 0);
    encoded += base64Alphabet[(group >> 18) & sextetMask];
    encoded += base64Alphabet[(group >> 12) & sextetMask];
    encoded += second === undefined ? base64Padding : base64Alphabet[(group >> 6) & sextetMask];
    encoded += third === undefined ? base64Padding : base64Alphabet[group & sextetMask];
  }
  return encoded;
}

function detectContentType(bytes: Uint8Array): string {
  if (bytes[0] === jpegFirstByte && bytes[1] === jpegSecondByte) {
    return jpegContentType;
  }
  if (bytes[0] === riffLetterR && bytes[webpSignatureOffset] === webpLetterW) {
    return webpContentType;
  }
  return pngContentType;
}

export function toImageDataUri(content: ArrayBuffer): string {
  const bytes = new Uint8Array(content);
  return `data:${detectContentType(bytes)};base64,${toBase64(bytes)}`;
}
