import { fireEvent, screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { InstructorDetailScreen } from "@/features/instructors/screens/InstructorDetailScreen";
import { getTeam } from "@/features/members/membersApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildInstructor } from "@/testing/classGroupFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");

const instructor = buildInstructor();

describe("When instructor edit is pressed", () => {
  beforeEach(() => {
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([instructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [], invitations: [] });
  });

  it("Then the edit form opens", async () => {
    await renderWithProviders(<InstructorDetailScreen instructorId={instructor.id} />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("instructors.detail.editAccessibility", { name: instructor.fullName }),
      }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.editInstructor(instructor.id));
  });
});
