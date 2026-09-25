const sortLocale = "es";

interface Activatable {
  isActive: boolean;
}

export function sortActiveFirstByName<TItem extends Activatable>(
  items: TItem[],
  getName: (item: TItem) => string,
): TItem[] {
  return [...items].sort(
    (first, second) =>
      Number(second.isActive) - Number(first.isActive) ||
      getName(first).localeCompare(getName(second), sortLocale),
  );
}
