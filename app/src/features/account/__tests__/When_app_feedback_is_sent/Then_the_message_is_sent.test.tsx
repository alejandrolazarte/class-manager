import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { Platform } from "react-native";
import { sendAppFeedback } from "@/features/account/accountApi";
import { AppFeedbackScreen } from "@/features/account/screens/AppFeedbackScreen";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

describe("When app feedback is sent", () => {
  beforeEach(() => {
    jest.mocked(sendAppFeedback).mockResolvedValue(undefined);
  });

  it("Then the message is sent", async () => {
    await renderWithProviders(<AppFeedbackScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("appFeedback.message")),
      " The calendar takes long to open. ",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("appFeedback.submit") }));

    await waitFor(() =>
      expect(sendAppFeedback).toHaveBeenCalledWith(
        expect.objectContaining({
          message: "The calendar takes long to open.",
          platform: Platform.OS,
        }),
      ),
    );
    expect(routerMock.back).toHaveBeenCalled();
  });
});
