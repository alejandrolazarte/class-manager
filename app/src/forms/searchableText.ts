const diacriticalMarks = /[̀-ͯ]/g;

export function searchableText(text: string): string {
  return text.normalize("NFD").replace(diacriticalMarks, "").trim().toLocaleLowerCase();
}
