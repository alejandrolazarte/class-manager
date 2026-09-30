import { AppState, AppStateStatus } from "react-native";
import { subscribeToAppFocus } from "@/api/appFocus";

describe("When the app comes back to the foreground", () => {
  it("Then queries are told the app is focused", () => {
    let changeListener: (status: AppStateStatus) => void = () => undefined;
    jest.spyOn(AppState, "addEventListener").mockImplementation((_, listener) => {
      changeListener = listener;
      return { remove: jest.fn() };
    });
    const onFocusChange = jest.fn();

    subscribeToAppFocus(onFocusChange);
    changeListener("background");
    changeListener("active");

    expect(onFocusChange.mock.calls).toEqual([[false], [true]]);
  });
});
