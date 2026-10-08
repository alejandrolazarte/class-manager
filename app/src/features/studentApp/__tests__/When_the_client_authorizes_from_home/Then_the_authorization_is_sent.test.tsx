import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getStudentAppHome,
  getStudentAppShop,
  giveGuardianConsentInApp,
} from "@/features/studentApp/studentAppApi";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { translate } from "@/i18n/translate";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";
import { buildStudentAppHome, buildStudentAppShop } from "@/testing/studentAppFactory";

jest.mock("@/features/studentApp/studentAppApi");

const invitationId = "0192f0c7-0000-7000-8000-000000000001";

describe("When the client authorizes from home", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        pendingGuardianConsents: [
          { invitationId, studentFullName: "Tomás Pérez", studentEmail: "tomas@example.com" },
        ],
      }),
    );
    jest.mocked(giveGuardianConsentInApp).mockResolvedValue();
  });

  it("Then the authorization is sent", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);
    expect(
      await screen.findByText(translate("guardianConsent.cardTitle", { name: "Tomás" })),
    ).toBeOnTheScreen();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("guardianConsent.authorize") }),
    );

    await waitFor(() => expect(giveGuardianConsentInApp).toHaveBeenCalledWith(invitationId));
  });
});
