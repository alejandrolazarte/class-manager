import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientScreen } from "@/features/clients/screens/RegisterClientScreen";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import {
  fillRegisterClientEmail,
  fillRegisterClientForm,
  setSendAppInvitation,
  submitRegisterClientForm,
} from "@/testing/fillRegisterClientForm";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSamePersonError } from "@/testing/samePersonError";

jest.mock("@/features/clients/clientsApi");

const instructorUserId = "0192f0c4-0000-7000-8000-000000000010";
const registeredClient = buildClient({ fullName: "Ana Pérez", email: "ana@example.com" });

describe("When a contact is registered with the email of an instructor", () => {
  beforeEach(() => {
    jest
      .mocked(registerClient)
      .mockRejectedValueOnce(buildSamePersonError(instructorUserId, "Ana Pérez"))
      .mockResolvedValueOnce(registeredClient);
  });

  it("Then the contact joins the instructor once confirmed", async () => {
    await renderWithProviders(<RegisterClientScreen />);
    await fillRegisterClientForm("Ana Pérez", "11 2233-4455");
    await fillRegisterClientEmail("ana@example.com");
    await setSendAppInvitation(false);
    await submitRegisterClientForm();

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("people.samePerson.confirm") }),
    );

    await waitFor(() =>
      expect(registerClient).toHaveBeenLastCalledWith(
        expect.objectContaining({ email: "ana@example.com", samePersonUserId: instructorUserId }),
      ),
    );
  });
});
