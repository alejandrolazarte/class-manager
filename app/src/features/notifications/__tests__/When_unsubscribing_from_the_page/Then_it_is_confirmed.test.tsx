import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getEmailUnsubscription,
  unsubscribeFromEmail,
} from "@/features/notifications/emailUnsubscribeApi";
import { EmailUnsubscribeScreen } from "@/features/notifications/screens/EmailUnsubscribeScreen";
import { translate } from "@/i18n/translate";
import {
  subscribedToEmails,
  unsubscribedFromEmails,
  unsubscribeToken,
} from "@/testing/emailUnsubscribeFactory";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/notifications/emailUnsubscribeApi");

describe("When unsubscribing from the page", () => {
  beforeEach(() => {
    searchParametersMock.current = { token: unsubscribeToken };
    jest.mocked(getEmailUnsubscription).mockResolvedValue(subscribedToEmails);
    jest.mocked(unsubscribeFromEmail).mockResolvedValue(unsubscribedFromEmails);
  });

  it("Then it is confirmed", async () => {
    await renderWithProviders(<EmailUnsubscribeScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("unsubscribe.confirm") }),
    );

    await waitFor(() => expect(unsubscribeFromEmail).toHaveBeenCalledWith(unsubscribeToken));
    expect(
      await screen.findByText(
        translate("unsubscribe.done", {
          category: translate("notificationPreferences.category.Orders"),
          business: subscribedToEmails.businessName,
        }),
      ),
    ).toBeOnTheScreen();
  });
});
