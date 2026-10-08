import { screen } from "@testing-library/react-native";
import { listClassGroupsIncludingInactive } from "@/features/classGroups/classGroupsApi";
import { listClassRoster } from "@/features/enrollments/enrollmentsApi";
import { ClassGroupDetailScreen } from "@/features/enrollments/screens/ClassGroupDetailScreen";
import { todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { buildRosterEntry } from "@/testing/enrollmentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/enrollments/enrollmentsApi");

const classGroup = buildClassGroup({ enrolledCount: 1 });
const leavingEntry = buildRosterEntry({ endDate: todayIsoDate() });

describe("When a student leaves the class today", () => {
  beforeEach(() => {
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([classGroup]);
    jest.mocked(listClassRoster).mockResolvedValue([leavingEntry]);
  });

  it("Then they cannot be unenrolled again", async () => {
    await renderWithProviders(<ClassGroupDetailScreen classGroupId={classGroup.id} />);
    await screen.findByText(leavingEntry.studentFullName);

    expect(
      screen.queryByRole("button", {
        name: translate("enrollments.detail.unenrollStudent", {
          name: leavingEntry.studentFullName,
        }),
      }),
    ).toBeNull();
  });
});
