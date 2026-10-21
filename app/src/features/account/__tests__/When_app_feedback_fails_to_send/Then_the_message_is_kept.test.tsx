import { fireEvent, screen } from "@testing-library/react-native";
import { sendAppFeedback } from "@/features/account/accountApi";
import { AppFeedbackScreen } from "@/features/account/screens/AppFeedbackScreen";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

describe("When app feedback fails to send", () => {
  beforeEach(() => {
    jest.mocked(sendAppFeedback).mockRejectedValue(new Error("server down"));
  });

  it("Then the message is kept", async () => {
    await renderWithProviders(<AppFeedbackScreen />);
    await fireEvent.changeText(screen.getByLabelText(translate("appFeedback.message")), "Hello");

    await fireEvent.press(screen.getByRole("button", { name: translate("appFeedback.submit") }));

    expect(await screen.findByText(translate("common.unexpectedError"))).toBeOnTheScreen();
    expect(screen.getByLabelText(translate("appFeedback.message"))).toHaveDisplayValue("Hello");
    expect(routerMock.back).not.toHaveBeenCalled();
  });
});
