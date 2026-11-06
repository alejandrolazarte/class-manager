import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getEmailUnsubscription,
  resubscribeToEmail,
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

describe("When resubscribing from the page", () => {
  beforeEach(() => {
    searchParametersMock.current = { token: unsubscribeToken };
    jest.mocked(getEmailUnsubscription).mockResolvedValue(unsubscribedFromEmails);
    jest.mocked(resubscribeToEmail).mockResolvedValue(subscribedToEmails);
  });

  it("Then the emails are on again", async () => {
    await renderWithProviders(<EmailUnsubscribeScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("unsubscribe.resubscribe") }),
    );

    await waitFor(() => expect(resubscribeToEmail).toHaveBeenCalledWith(unsubscribeToken));
    expect(await screen.findByText(translate("unsubscribe.resubscribed"))).toBeOnTheScreen();
    expect(
      screen.getByRole("button", { name: translate("unsubscribe.confirm") }),
    ).toBeOnTheScreen();
  });
});
