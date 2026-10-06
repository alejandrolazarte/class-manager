import { screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { InstructorFormScreen } from "@/features/instructors/screens/InstructorFormScreen";
import { getTeam } from "@/features/members/membersApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { buildMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");
jest.mock("@/features/members/membersApi");

const instructor = buildInstructor();
const member = buildMember({ email: "laura@example.com", instructorId: instructor.id });

describe("When instructor uses the app", () => {
  beforeEach(() => {
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([instructor]);
    jest.mocked(getTeam).mockResolvedValue({ members: [member], invitations: [] });
  });

  it("Then the email cannot be edited", async () => {
    await renderWithProviders(<InstructorFormScreen instructorId={instructor.id} />);

    const emailField = await screen.findByLabelText(translate("instructors.form.email"));

    expect(emailField).toHaveProp("editable", false);
    expect(emailField).toHaveDisplayValue(member.email);
  });
});
