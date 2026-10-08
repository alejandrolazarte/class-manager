import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { updateMyProfile, getMyAccount } from "@/features/account/accountApi";
import { EditMyProfileScreen } from "@/features/account/screens/EditMyProfileScreen";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

describe("When the name is fixed", () => {
  beforeEach(() => {
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomas Perez",
      birthDate: "1990-04-12",
    });
    jest.mocked(updateMyProfile).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomás Pérez",
    });
  });

  it("Then the new name is saved", async () => {
    await renderWithProviders(<EditMyProfileScreen />);
    const fullNameField = await screen.findByLabelText(translate("profile.name.fullName"));
    expect(fullNameField).toHaveDisplayValue("Tomas Perez");
    await fireEvent.changeText(fullNameField, " Tomás Pérez ");

    await fireEvent.press(screen.getByRole("button", { name: translate("profile.edit.submit") }));

    await waitFor(() =>
      expect(updateMyProfile).toHaveBeenCalledWith({
        fullName: "Tomás Pérez",
        birthDate: "1990-04-12",
      }),
    );
    expect(routerMock.back).toHaveBeenCalled();
  });
});
