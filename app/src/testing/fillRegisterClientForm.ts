import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";

export async function fillRegisterClientForm(fullName: string, phoneNumber: string): Promise<void> {
  await fireEvent.changeText(
    screen.getByLabelText(translate("clients.register.fullName")),
    fullName,
  );
  await fireEvent.changeText(
    screen.getByLabelText(translate("clients.register.phoneNumber")),
    phoneNumber,
  );
}

export async function submitRegisterClientForm(): Promise<void> {
  await fireEvent.press(screen.getByRole("button", { name: translate("clients.register.submit") }));
}
