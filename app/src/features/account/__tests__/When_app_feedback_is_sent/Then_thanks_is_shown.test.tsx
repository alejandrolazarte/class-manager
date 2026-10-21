import { fireEvent, screen } from "@testing-library/react-native";
import { sendAppFeedback } from "@/features/account/accountApi";
import { AppFeedbackScreen } from "@/features/account/screens/AppFeedbackScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

describe("When app feedback is sent", () => {
  beforeEach(() => {
    jest.mocked(sendAppFeedback).mockResolvedValue(undefined);
  });

  it("Then thanks is shown", async () => {
    await renderWithProviders(<AppFeedbackScreen />);
    await fireEvent.changeText(screen.getByLabelText(translate("appFeedback.message")), "Hello");

    await fireEvent.press(screen.getByRole("button", { name: translate("appFeedback.submit") }));

    expect(await screen.findByText(translate("appFeedback.sent"))).toBeOnTheScreen();
  });
});
