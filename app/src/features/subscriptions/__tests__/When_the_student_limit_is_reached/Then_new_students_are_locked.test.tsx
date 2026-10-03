import { screen } from "@testing-library/react-native";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/students/studentsApi");

const studentLimit = 30;

describe("When the student limit is reached", () => {
  beforeEach(() => {
    jest.mocked(searchStudents).mockResolvedValue([]);
  });

  it("Then new students are locked", async () => {
    const subscription = buildSubscription({
      features: [{ code: "students", limit: studentLimit, used: studentLimit }],
    });

    await renderWithProviders(<StudentListScreen />, {
      member: buildCurrentMember({ subscription }),
    });

    expect(screen.getByText(translate("subscriptions.locked.students"))).toBeTruthy();
    expect(
      screen.queryByRole("button", { name: translate("students.list.newStudent") }),
    ).toBeNull();
  });
});
