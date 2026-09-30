const wordSeparatorPattern = /\s+/;
const initialsWordLimit = 2;
const unknownInitials = "?";

export function brandInitials(displayName: string): string {
  const initials = displayName
    .split(wordSeparatorPattern)
    .filter((word) => word.length > 0)
    .slice(0, initialsWordLimit)
    .map((word) => word[0]!.toUpperCase())
    .join("");
  return initials === "" ? unknownInitials : initials;
}
