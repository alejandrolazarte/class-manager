import { renderHook } from "@testing-library/react-native";
import { useStackScreenOptions } from "@/navigation/stackScreenOptions";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When reduced motion is on", () => {
  it("Then screens change without a slide", async () => {
    const { result } = await renderHook(() => useStackScreenOptions());

    expect(result.current.animation).toBe("none");
  });
});
