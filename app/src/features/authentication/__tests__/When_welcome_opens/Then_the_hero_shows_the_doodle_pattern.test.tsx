import { screen } from "@testing-library/react-native";
import { WelcomeScreen } from "@/features/authentication/screens/WelcomeScreen";
import { translate } from "@/i18n/translate";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

describe("When welcome opens", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
  });

  it("Then the hero shows the doodle pattern", async () => {
    await renderWithSession(<WelcomeScreen />);

    expect(await screen.findByText(translate("authentication.welcome.title"))).toBeOnTheScreen();
    expect(screen.getByTestId("doodle-pattern", { includeHiddenElements: true })).toBeTruthy();
  });
});
