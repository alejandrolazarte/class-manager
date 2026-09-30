import { renderHook } from "@testing-library/react-native";
import { useFocusEffect } from "expo-router";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";

describe("When a screen gets focus again", () => {
  it("Then its data is refetched", async () => {
    const focusCallbacks: (() => void)[] = [];
    jest.mocked(useFocusEffect).mockImplementation((callback) => {
      focusCallbacks.push(callback as () => void);
    });
    const refetch = jest.fn();

    await renderHook(() => useRefetchOnFocus(refetch));
    focusCallbacks.at(-1)?.();
    focusCallbacks.at(-1)?.();

    expect(refetch).toHaveBeenCalledTimes(1);
  });
});
