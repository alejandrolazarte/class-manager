export function installInMemoryLocalStorage(): Map<string, string> {
  const storedItems = new Map<string, string>();
  const inMemoryLocalStorage: Pick<Storage, "getItem" | "setItem" | "removeItem"> = {
    getItem: (key) => storedItems.get(key) ?? null,
    setItem: (key, value) => {
      storedItems.set(key, value);
    },
    removeItem: (key) => {
      storedItems.delete(key);
    },
  };
  Object.defineProperty(globalThis, "localStorage", {
    value: inMemoryLocalStorage,
    configurable: true,
  });
  return storedItems;
}
