import { fireEvent, screen } from "@testing-library/react-native";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/students/studentsApi");

describe("When the owner browses an ended plan", () => {
  beforeEach(() => {
    jest.mocked(searchStudents).mockResolvedValue([]);
  });

  it("Then creating offers a plan", async () => {
    const subscription = buildSubscription({ isActive: false, features: [] });
    await renderWithProviders(<StudentListScreen />, {
      member: buildCurrentMember({ subscription }),
    });

    await fireEvent.press(
      screen.getByRole("button", { name: translate("students.list.newStudent") }),
    );

    expect(screen.getByText(translate("subscriptions.ended.trialTitle"))).toBeTruthy();
    expect(routerMock.push).not.toHaveBeenCalled();
  });
});
