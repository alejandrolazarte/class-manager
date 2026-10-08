import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getMyAccount, requestEmailChange } from "@/features/account/accountApi";
import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

describe("When a new email is requested", () => {
  beforeEach(() => {
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "laura@example.com",
      fullName: "Laura Gómez",
    });
    jest.mocked(requestEmailChange).mockResolvedValue({ newEmail: "laura.nueva@example.com" });
  });

  it("Then the link is sent to it", async () => {
    await renderWithProviders(<MyProfileScreen editProfileRoute={routes.editMyProfile} />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("profile.email.newEmail")),
      " laura.nueva@example.com ",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("profile.email.currentPassword")),
      "a long passphrase",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("profile.email.submit") }));

    await waitFor(() =>
      expect(requestEmailChange).toHaveBeenCalledWith({
        newEmail: "laura.nueva@example.com",
        currentPassword: "a long passphrase",
      }),
    );
    expect(
      await screen.findByText(
        translate("profile.email.sent", { email: "laura.nueva@example.com" }),
      ),
    ).toBeOnTheScreen();
  });
});
