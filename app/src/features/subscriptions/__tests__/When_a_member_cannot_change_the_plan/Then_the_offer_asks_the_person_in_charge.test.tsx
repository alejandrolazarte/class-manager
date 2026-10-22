import { fireEvent, screen } from "@testing-library/react-native";
import { everyPermission, permissions } from "@/features/members/permissions";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/students/studentsApi");

const studentLimit = 30;

describe("When a member cannot change the plan", () => {
  beforeEach(() => {
    jest.mocked(searchStudents).mockResolvedValue([]);
  });

  it("Then the offer asks the person in charge", async () => {
    const member = buildCurrentMember({
      isBrandOwner: false,
      permissions: everyPermission.filter(
        (permission) => permission !== permissions.subscriptionView,
      ),
      subscription: buildSubscription({
        features: [{ code: "students", limit: studentLimit, used: studentLimit }],
      }),
    });
    await renderWithProviders(<StudentListScreen />, { member });

    await fireEvent.press(
      screen.getByRole("button", { name: translate("students.list.newStudent") }),
    );

    expect(screen.getByText(translate("subscriptions.askOwner"))).toBeTruthy();
    expect(screen.queryByRole("button", { name: translate("subscriptions.seePlans") })).toBeNull();
  });
});
