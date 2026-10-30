import { screen } from "@testing-library/react-native";
import { listClassGroupsIncludingInactive } from "@/features/classGroups/classGroupsApi";
import { listClassRoster } from "@/features/enrollments/enrollmentsApi";
import { ClassGroupDetailScreen } from "@/features/enrollments/screens/ClassGroupDetailScreen";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/enrollments/enrollmentsApi");

const instructorPhotoUrl = "https://files.example.com/private/laura.webp?sig=abc";
const classGroup = buildClassGroup({ instructorPhotoUrl });

describe("When the class instructor has a photo", () => {
  beforeEach(() => {
    jest.mocked(listClassGroupsIncludingInactive).mockResolvedValue([classGroup]);
    jest.mocked(listClassRoster).mockResolvedValue([]);
  });

  it("Then the instructor card shows it", async () => {
    await renderWithProviders(<ClassGroupDetailScreen classGroupId={classGroup.id} />);

    expect(await screen.findByText(translate("enrollments.detail.instructor"))).toBeOnTheScreen();
    expect(screen.getAllByText(classGroup.instructorFullName)).toHaveLength(1);
    expect(screen.getByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      { uri: instructorPhotoUrl },
    );
  });
});
