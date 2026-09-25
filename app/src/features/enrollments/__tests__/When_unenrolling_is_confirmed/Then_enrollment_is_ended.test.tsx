import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listClassGroupsIncludingInactive } from "@/features/classGroups/classGroupsApi";
import { endEnrollment, listClassRoster } from "@/features/enrollments/enrollmentsApi";
import { ClassGroupDetailScreen } from "@/features/enrollments/screens/ClassGroupDetailScreen";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { buildRosterEntry } from "@/testing/enrollmentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/enrollments/enrollmentsApi");

const classGroup = buildClassGroup({ enrolledCount: 1 });
const rosterEntry = buildRosterEntry();

describe("When unenrolling is confirmed", () => {
  beforeEach(() => {
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([classGroup]);
    jest.mocked(listClassRoster).mockResolvedValue([rosterEntry]);
    jest.mocked(endEnrollment).mockResolvedValue(undefined);
  });

  it("Then enrollment is ended", async () => {
    await renderWithProviders(<ClassGroupDetailScreen classGroupId={classGroup.id} />);
    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("enrollments.detail.unenrollStudent", {
          name: rosterEntry.studentFullName,
        }),
      }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("enrollments.detail.confirmUnenroll") }),
    );

    await waitFor(() => expect(endEnrollment).toHaveBeenCalledWith(rosterEntry.enrollmentId, {}));
  });
});
