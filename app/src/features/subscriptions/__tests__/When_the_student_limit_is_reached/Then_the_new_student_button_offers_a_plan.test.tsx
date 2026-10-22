import { fireEvent, screen } from "@testing-library/react-native";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/students/studentsApi");

const studentLimit = 30;

describe("When the student limit is reached", () => {
  beforeEach(() => {
    jest.mocked(searchStudents).mockResolvedValue([]);
  });

  it("Then the new student button offers a plan", async () => {
    const subscription = buildSubscription({
      features: [{ code: "students", limit: studentLimit, used: studentLimit }],
    });
    await renderWithProviders(<StudentListScreen />, {
      member: buildCurrentMember({ subscription }),
    });

    await fireEvent.press(
      screen.getByRole("button", { name: translate("students.list.newStudent") }),
    );

    expect(screen.getByText(translate("subscriptions.locked.students"))).toBeTruthy();
    expect(screen.getByRole("button", { name: translate("subscriptions.seePlans") })).toBeTruthy();
    expect(routerMock.push).not.toHaveBeenCalled();
  });
});
