import { fireEvent, screen } from "@testing-library/react-native";
import { listAccounts } from "@/features/authentication/authenticationApi";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppSettingsScreen } from "@/features/studentApp/screens/StudentAppSettingsScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildAccount } from "@/testing/accountFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/studentApp/studentAppApi");

describe("When student also has a team account", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest
      .mocked(listAccounts)
      .mockResolvedValue([
        buildAccount({ isCurrent: false }),
        buildAccount({ businessId: "business-school", kind: "student", isCurrent: true }),
      ]);
  });

  it("Then it can switch from settings", async () => {
    await renderStudentAppScreen(<StudentAppSettingsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("accounts.switch") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.chooseAccount);
  });
});
