import { translate } from "@/i18n/translate";

const listSeparator = ", ";

export function summarizeNames(names: readonly string[]): string {
  const firstNames = names.map((name) => name.split(" ")[0] ?? name);
  const lastName = firstNames.pop();
  if (lastName === undefined) {
    return "";
  }
  return firstNames.length === 0
    ? lastName
    : `${firstNames.join(listSeparator)}${translate("enrollments.lastItemJoiner")}${lastName}`;
}
