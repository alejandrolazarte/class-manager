import { useFocusEffect } from "expo-router";
import { useCallback, useRef } from "react";

export function useRefetchOnFocus(refetch: () => unknown) {
  const isFirstFocus = useRef(true);
  useFocusEffect(
    useCallback(() => {
      if (isFirstFocus.current) {
        isFirstFocus.current = false;
        return;
      }
      refetch();
    }, [refetch]),
  );
}
