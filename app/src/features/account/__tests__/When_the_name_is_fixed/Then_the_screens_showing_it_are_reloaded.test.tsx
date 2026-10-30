import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getMyAccount, updateMyProfile } from "@/features/account/accountApi";
import { EditMyProfileScreen } from "@/features/account/screens/EditMyProfileScreen";
import { memberQueryKeys } from "@/features/members/memberQueryKeys";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";
import { translate } from "@/i18n/translate";
import { createTestQueryClient, renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

describe("When the name is fixed", () => {
  beforeEach(() => {
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomas Perez",
      birthDate: "1990-04-12",
      hasAcceptedCurrentLegalTerms: true,
      preferredLanguage: "es",
    });
    jest.mocked(updateMyProfile).mockResolvedValue({
      email: "tomas@example.com",
      fullName: "Tomás Pérez",
      birthDate: "1990-04-12",
      hasAcceptedCurrentLegalTerms: true,
      preferredLanguage: "es",
    });
  });

  it("Then the screens showing it are reloaded", async () => {
    const queryClient = createTestQueryClient();
    const invalidateQueries = jest.spyOn(queryClient, "invalidateQueries");
    await renderWithProviders(<EditMyProfileScreen />, { queryClient });
    await fireEvent.changeText(
      await screen.findByLabelText(translate("profile.name.fullName")),
      "Tomás Pérez",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("profile.edit.submit") }));

    await waitFor(() =>
      expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: memberQueryKeys.all }),
    );
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: studentAppQueryKeys.home() });
    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: studentAppQueryKeys.family() });
  });
});
