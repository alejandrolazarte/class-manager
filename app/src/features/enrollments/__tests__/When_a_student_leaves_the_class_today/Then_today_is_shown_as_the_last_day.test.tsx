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

describe("When a student leaves the class today", () => {
  beforeEach(() => {
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([classGroup]);
    jest.mocked(listClassRoster).mockResolvedValue([buildRosterEntry({ endDate: todayIsoDate() })]);
  });

  it("Then today is shown as the last day", async () => {
    await renderWithProviders(<ClassGroupDetailScreen classGroupId={classGroup.id} />);

    expect(await screen.findByText(translate("enrollments.detail.lastDayToday"))).toBeOnTheScreen();
  });
});
