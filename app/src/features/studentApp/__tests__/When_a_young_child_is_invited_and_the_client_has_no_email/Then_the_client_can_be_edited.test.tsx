import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildClient } from "@/testing/clientFactory";
import { routerMock, segmentsMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/studentApp/studentAppApi");

const badRequestStatus = 400;
const child = buildStudent({ fullName: "Mía Sosa", birthDate: "2019-02-28" });
const client = buildClient({ fullName: "Carla Sosa", email: null, students: [child] });

describe("When a young child is invited and the client has no email", () => {
  beforeEach(() => {
    segmentsMock.current = ["(tabs)", "students", "clients", "[clientId]"];
    jest
      .mocked(inviteStudentApp)
      .mockRejectedValue(
        new ApiError(badRequestStatus, { code: "student_invitation.guardian_email_required" }),
      );
  });

  it("Then the client can be edited", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} student={child} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.inviteAccessibility") }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("student.invite.studentEmail", { name: child.fullName })),
      "mia@example.com",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("student.invite.send") }));

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("student.invite.addGuardianEmail", { guardian: client.fullName }),
      }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.editClient("students", client.id));
  });
});
