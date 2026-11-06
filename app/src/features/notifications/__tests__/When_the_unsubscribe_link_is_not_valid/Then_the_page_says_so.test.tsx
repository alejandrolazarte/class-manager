import { screen } from "@testing-library/react-native";
import { getEmailUnsubscription } from "@/features/notifications/emailUnsubscribeApi";
import { EmailUnsubscribeScreen } from "@/features/notifications/screens/EmailUnsubscribeScreen";
import { translate } from "@/i18n/translate";
import { unsubscribeToken } from "@/testing/emailUnsubscribeFactory";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/notifications/emailUnsubscribeApi");

describe("When the unsubscribe link is not valid", () => {
  beforeEach(() => {
    searchParametersMock.current = { token: unsubscribeToken };
    jest.mocked(getEmailUnsubscription).mockRejectedValue(new Error("invalid"));
  });

  it("Then the page says so", async () => {
    await renderWithProviders(<EmailUnsubscribeScreen />);

    expect(await screen.findByText(translate("unsubscribe.invalidLink"))).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: translate("unsubscribe.confirm") })).toBeNull();
  });
});
