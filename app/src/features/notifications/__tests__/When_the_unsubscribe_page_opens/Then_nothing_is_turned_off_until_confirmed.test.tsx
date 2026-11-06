import { screen } from "@testing-library/react-native";
import {
  getEmailUnsubscription,
  resubscribeToEmail,
  unsubscribeFromEmail,
} from "@/features/notifications/emailUnsubscribeApi";
import { EmailUnsubscribeScreen } from "@/features/notifications/screens/EmailUnsubscribeScreen";
import { translate } from "@/i18n/translate";
import { subscribedToEmails, unsubscribeToken } from "@/testing/emailUnsubscribeFactory";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/notifications/emailUnsubscribeApi");

describe("When the unsubscribe page opens", () => {
  beforeEach(() => {
    searchParametersMock.current = { token: unsubscribeToken };
    jest.mocked(getEmailUnsubscription).mockResolvedValue(subscribedToEmails);
  });

  it("Then nothing is turned off until confirmed", async () => {
    await renderWithProviders(<EmailUnsubscribeScreen />);

    expect(
      await screen.findByRole("button", { name: translate("unsubscribe.confirm") }),
    ).toBeOnTheScreen();
    expect(getEmailUnsubscription).toHaveBeenCalledWith(unsubscribeToken);
    expect(unsubscribeFromEmail).not.toHaveBeenCalled();
    expect(resubscribeToEmail).not.toHaveBeenCalled();
  });
});
