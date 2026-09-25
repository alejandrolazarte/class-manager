import { screen, waitFor } from "@testing-library/react-native";
import { createClassGroup } from "@/features/classGroups/classGroupsApi";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { buildClassGroup, buildInstructor } from "@/testing/classGroupFactory";
import { fillClassGroupForm, submitClassGroupForm } from "@/testing/fillClassGroupForm";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");

const instructor = buildInstructor();

describe("When class is created", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([instructor]);
    jest.mocked(createClassGroup).mockResolvedValue(buildClassGroup());
  });

  it("Then request has weekdays and start time", async () => {
    await renderWithProviders(<ClassGroupFormScreen />);
    await screen.findByRole("button", { name: instructor.fullName });
    await fillClassGroupForm({ weekdays: ["Thursday", "Tuesday"], startTime: "1800" });
    await submitClassGroupForm();

    await waitFor(() =>
      expect(createClassGroup).toHaveBeenCalledWith({
        name: "Natación inicial",
        instructorId: instructor.id,
        weekdays: ["Tuesday", "Thursday"],
        startTime: "18:00",
        durationMinutes: 45,
        capacity: 8,
        location: null,
      }),
    );
  });
});
