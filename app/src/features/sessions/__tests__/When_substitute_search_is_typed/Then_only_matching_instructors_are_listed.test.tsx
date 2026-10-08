import { fireEvent, screen, within } from "@testing-library/react-native";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession } from "@/features/sessions/sessionsApi";
import { translate } from "@/i18n/translate";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");
jest.mock("@/features/instructors/instructorsApi");

const session = buildSessionDetails();
const matchingInstructor = buildInstructor({
  id: "0192f0d1-0000-7000-8000-000000000002",
  fullName: "Valentina Ríos",
});
const otherInstructor = buildInstructor({
  id: "0192f0d1-0000-7000-8000-000000000003",
  fullName: "Marcos Díaz",
});

describe("When substitute search is typed", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest
      .mocked(listActiveInstructors)
      .mockResolvedValue([buildInstructor(), matchingInstructor, otherInstructor]);
  });

  it("Then only matching instructors are listed", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("sessions.substitute.open") }),
    );
    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("sessions.substitute.chooseInstructor"),
      }),
    );
    const picker = within(screen.getByTestId("substitute-instructor-picker"));
    await picker.findByRole("button", { name: otherInstructor.fullName });

    await fireEvent.changeText(
      picker.getByPlaceholderText(translate("instructors.picker.search")),
      "rios",
    );

    expect(picker.queryByRole("button", { name: otherInstructor.fullName })).toBeNull();
    expect(picker.getByRole("button", { name: matchingInstructor.fullName })).toBeOnTheScreen();
  });
});
