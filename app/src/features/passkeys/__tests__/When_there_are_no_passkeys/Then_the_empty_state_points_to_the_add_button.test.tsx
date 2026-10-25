import { fireEvent, screen } from "@testing-library/react-native";
import { listMyPasskeys } from "@/features/passkeys/passkeysApi";
import { PasskeysScreen } from "@/features/passkeys/screens/PasskeysScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/passkeys/passkeysApi");

describe("When there are no passkeys", () => {
  beforeEach(() => {
    jest.mocked(listMyPasskeys).mockResolvedValue([]);
  });

  it("Then the empty state points to the add button", async () => {
    await renderWithProviders(<PasskeysScreen addPasskeyRoute={routes.addMyPasskey} />);

    expect(await screen.findByText(translate("profile.passkeys.empty"))).toBeOnTheScreen();
    await fireEvent.press(screen.getByRole("button", { name: translate("profile.passkeys.add") }));
    expect(routerMock.push).toHaveBeenCalledWith(routes.addMyPasskey);
  });
});
