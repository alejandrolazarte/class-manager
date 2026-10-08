import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { InviteStudentAppSection } from "@/features/studentApp/components/InviteStudentAppSection";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/studentApp/studentAppApi");

const badRequestStatus = 400;
const contactAsStudent = buildStudent({ fullName: "Juan Chico", birthDate: "2011-03-10" });
const client = buildClient({
  fullName: "Juan Chico",
  email: "juan@example.com",
  students: [contactAsStudent],
});

describe("When a contact who attends is a minor", () => {
  beforeEach(() => {
    jest
      .mocked(inviteStudentApp)
      .mockRejectedValue(new ApiError(badRequestStatus, { code: "client.contact_must_be_adult" }));
  });

  it("Then the invitation is refused with an explanation", async () => {
    await renderWithProviders(<InviteStudentAppSection client={client} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.app.inviteAccessibility") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("student.invite.send") }));

    expect(
      await screen.findByText(
        translate("clients.validation.contactMustBeAdult", { name: client.fullName }),
      ),
    ).toBeOnTheScreen();
  });
});
