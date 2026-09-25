import { screen } from "@testing-library/react-native";
import { listClassGroupsIncludingInactive } from "@/features/classGroups/classGroupsApi";
import { listClassRoster } from "@/features/enrollments/enrollmentsApi";
import { ClassGroupDetailScreen } from "@/features/enrollments/screens/ClassGroupDetailScreen";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { buildRosterEntry } from "@/testing/enrollmentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/enrollments/enrollmentsApi");

const fullClassGroup = buildClassGroup({ capacity: 1, enrolledCount: 1 });

describe("When class is full", () => {
  beforeEach(() => {
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([fullClassGroup]);
    jest.mocked(listClassRoster).mockResolvedValue([buildRosterEntry()]);
  });

  it("Then enroll button is disabled", async () => {
    await renderWithProviders(<ClassGroupDetailScreen classGroupId={fullClassGroup.id} />);

    expect(
      await screen.findByRole("button", { name: translate("enrollments.detail.classFull") }),
    ).toBeDisabled();
  });
});
