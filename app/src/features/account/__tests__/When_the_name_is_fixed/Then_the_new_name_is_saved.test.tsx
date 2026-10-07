import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { changeMyFullName, getMyAccount } from "@/features/account/accountApi";
import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

describe("When the name is fixed", () => {
  beforeEach(() => {
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomas Perez",
    });
    jest.mocked(changeMyFullName).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomás Pérez",
    });
  });

  it("Then the new name is saved", async () => {
    await renderWithProviders(<MyProfileScreen />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("profile.name.fullName")),
      " Tomás Pérez ",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("profile.name.submit") }));

    await waitFor(() => expect(changeMyFullName).toHaveBeenCalledWith("Tomás Pérez"));
    expect(await screen.findByText(translate("profile.name.saved"))).toBeOnTheScreen();
  });
});
