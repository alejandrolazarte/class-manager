import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createInstructor } from "@/features/instructors/instructorsApi";
import { AddTeamMemberScreen } from "@/features/members/screens/AddTeamMemberScreen";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");

describe("When an instructor is added with a padded email", () => {
  beforeEach(() => {
    jest.mocked(createInstructor).mockResolvedValue(buildInstructor());
  });

  it("Then request has the email", async () => {
    await renderWithProviders(<AddTeamMemberScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("instructors.form.fullName")),
      "Laura Gómez",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("team.invite.email")),
      " laura@example.com ",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createInstructor).toHaveBeenCalledWith({
        fullName: "Laura Gómez",
        email: "laura@example.com",
      }),
    );
  });
});
