import { useEffect, useState } from "react";

export function useDebouncedValue<TValue>(value: TValue, delayMilliseconds: number): TValue {
  const [debouncedValue, setDebouncedValue] = useState(value);

  useEffect(() => {
    const timeoutId = setTimeout(() => setDebouncedValue(value), delayMilliseconds);
    return () => clearTimeout(timeoutId);
  }, [value, delayMilliseconds]);

  return debouncedValue;
}
