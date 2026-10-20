import { screen } from "@testing-library/react-native";
import { AppFeedbackScreen } from "@/features/account/screens/AppFeedbackScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

describe("When app feedback has no message", () => {
  it("Then send is disabled", async () => {
    await renderWithProviders(<AppFeedbackScreen />);

    expect(screen.getByRole("button", { name: translate("appFeedback.submit") })).toBeDisabled();
  });
});
