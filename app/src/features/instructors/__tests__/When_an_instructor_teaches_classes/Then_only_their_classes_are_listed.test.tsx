import { screen } from "@testing-library/react-native";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { InstructorDetailScreen } from "@/features/instructors/screens/InstructorDetailScreen";
import { getTeam } from "@/features/members/membersApi";
import { buildClassGroup, buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");
jest.mock("@/features/classGroups/classGroupsApi");

const instructor = buildInstructor();
const ownClassGroup = buildClassGroup({
  id: "own",
  name: "Grupo inicial",
  instructorId: instructor.id,
});
const otherClassGroup = buildClassGroup({
  id: "other",
  name: "Grupo avanzado",
  instructorId: "another-instructor",
});

describe("When an instructor teaches classes", () => {
  beforeEach(() => {
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([instructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
    jest.mocked(listActiveClassGroups).mockResolvedValue([ownClassGroup, otherClassGroup]);
  });

  it("Then only their classes are listed", async () => {
    await renderWithProviders(<InstructorDetailScreen instructorId={instructor.id} />);

    expect(await screen.findByText(ownClassGroup.name)).toBeOnTheScreen();
    expect(screen.queryByText(otherClassGroup.name)).toBeNull();
  });
});
