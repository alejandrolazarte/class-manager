import { screen } from "@testing-library/react-native";
import { confirmEmailChange } from "@/features/account/accountApi";
import { ConfirmEmailChangeScreen } from "@/features/account/screens/ConfirmEmailChangeScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

describe("When the email change link is opened", () => {
  beforeEach(() => {
    searchParametersMock.current = { token: "change-token" };
    jest.mocked(confirmEmailChange).mockResolvedValue({ newEmail: "laura.nueva@example.com" });
  });

  it("Then the new email is confirmed", async () => {
    await renderWithProviders(<ConfirmEmailChangeScreen />);

    expect(
      await screen.findByText(
        translate("profile.confirm.done", { email: "laura.nueva@example.com" }),
      ),
    ).toBeOnTheScreen();
    expect(confirmEmailChange).toHaveBeenCalledWith("change-token");
  });
});
