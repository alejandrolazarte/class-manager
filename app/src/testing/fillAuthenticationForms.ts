import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";

export const ownerEmail = "laura@example.com";
export const ownerPassword = "a long passphrase";

export async function fillSignInForm(
  email: string = ownerEmail,
  password: string = ownerPassword,
): Promise<void> {
  await fireEvent.changeText(
    await screen.findByLabelText(translate("authentication.signIn.email")),
    email,
  );
  await fireEvent.changeText(
    screen.getByLabelText(translate("authentication.signIn.password")),
    password,
  );
}

export async function submitSignInForm(): Promise<void> {
  await fireEvent.press(
    screen.getByRole("button", { name: translate("authentication.signIn.submit") }),
  );
}

export async function fillSignUpForm(): Promise<void> {
  await fireEvent.changeText(
    await screen.findByLabelText(translate("authentication.signUp.ownerFullName")),
    "Laura Gómez",
  );
  await fireEvent.changeText(
    screen.getByLabelText(translate("authentication.signUp.email")),
    ownerEmail,
  );
  await fireEvent.changeText(
    screen.getByLabelText(translate("authentication.signUp.password")),
    ownerPassword,
  );
  await fireEvent.changeText(
    screen.getByLabelText(translate("authentication.signUp.businessName")),
    "Panadería Laura",
  );
}

export async function submitSignUpForm(): Promise<void> {
  await fireEvent.press(
    screen.getByRole("button", { name: translate("authentication.signUp.submit") }),
  );
}
