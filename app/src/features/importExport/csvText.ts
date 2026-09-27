export const csvMimeType = "text/csv";

const byteOrderMark = "﻿";

export function withByteOrderMark(csvText: string): string {
  return csvText.startsWith(byteOrderMark) ? csvText : byteOrderMark + csvText;
}
