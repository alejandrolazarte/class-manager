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

export async function setClientAttends(clientAttends: boolean): Promise<void> {
  const clientAttendsSwitch = screen.getByRole("switch", {
    name: translate("clients.register.clientAttends"),
  });
  if (Boolean(clientAttendsSwitch.props.accessibilityState?.checked) !== clientAttends) {
    await fireEvent.press(clientAttendsSwitch);
  }
}

export async function addAdditionalStudent(fullName: string, birthDate = ""): Promise<void> {
  await fireEvent.press(
    screen.getByRole("button", { name: translate("clients.register.addAttendee") }),
  );
  const fullNameFields = screen.getAllByLabelText(translate("students.fields.fullName"));
  const birthDateFields = screen.getAllByLabelText(translate("students.fields.birthDate"));
  await fireEvent.changeText(fullNameFields[fullNameFields.length - 1], fullName);
  await fireEvent.changeText(birthDateFields[birthDateFields.length - 1], birthDate);
}

export async function submitRegisterClientForm(): Promise<void> {
  await fireEvent.press(screen.getByRole("button", { name: translate("clients.register.submit") }));
}

export async function fillRegisterClientEmail(email: string): Promise<void> {
  await fireEvent.changeText(screen.getByLabelText(translate("clients.register.email")), email);
}

export async function setSendAppInvitation(sendAppInvitation: boolean): Promise<void> {
  const sendAppInvitationSwitch = screen.getByRole("switch", {
    name: translate("clients.register.sendAppInvitation"),
  });
  if (Boolean(sendAppInvitationSwitch.props.accessibilityState?.checked) !== sendAppInvitation) {
    await fireEvent.press(sendAppInvitationSwitch);
  }
}
