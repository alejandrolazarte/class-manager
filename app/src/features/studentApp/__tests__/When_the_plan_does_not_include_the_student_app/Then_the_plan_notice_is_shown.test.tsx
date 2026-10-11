import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { subscriptionErrorCodes } from "@/features/subscriptions/subscriptionCodes";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/studentApp/studentAppApi");

const forbiddenStatus = 403;
const client = buildClient({ email: "ana@example.com" });

describe("When the plan does not include the student app", () => {
  beforeEach(() => {
    jest
      .mocked(inviteStudentApp)
      .mockRejectedValue(
        new ApiError(forbiddenStatus, { code: subscriptionErrorCodes.featureNotInPlan }),
      );
  });

  it("Then the plan notice is shown", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.inviteAccessibility") }),
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("student.invite.send") }));

    expect(await screen.findByText(translate("subscriptions.locked.studentApp"))).toBeOnTheScreen();
    expect(screen.queryByText(translate("common.unexpectedError"))).toBeNull();
  });
});
