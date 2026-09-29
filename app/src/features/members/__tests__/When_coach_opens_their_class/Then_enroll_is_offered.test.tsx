import { screen } from "@testing-library/react-native";
import { listClassGroupsIncludingInactive } from "@/features/classGroups/classGroupsApi";
import { listClassRoster } from "@/features/enrollments/enrollmentsApi";
import { ClassGroupDetailScreen } from "@/features/enrollments/screens/ClassGroupDetailScreen";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { buildRosterEntry } from "@/testing/enrollmentFactory";
import { buildCoach } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/enrollments/enrollmentsApi");

const classGroup = buildClassGroup({ enrolledCount: 1 });
const rosterEntry = buildRosterEntry();

describe("When coach opens their class", () => {
  beforeEach(() => {
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([classGroup]);
    jest.mocked(listClassRoster).mockResolvedValue([rosterEntry]);
  });

  it("Then enroll is offered", async () => {
    await renderWithProviders(<ClassGroupDetailScreen classGroupId={classGroup.id} />, {
      member: buildCoach(),
    });

    expect(
      await screen.findByRole("button", { name: translate("enrollments.detail.enrollStudent") }),
    ).toBeOnTheScreen();
    expect(
      screen.getByRole("button", {
        name: translate("enrollments.detail.unenrollStudent", {
          name: rosterEntry.studentFullName,
        }),
      }),
    ).toBeOnTheScreen();
  });
});
