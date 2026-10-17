import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";

export async function confirmContactIsAdult(contactFullName: string): Promise<void> {
  await fireEvent.press(
    screen.getByRole("checkbox", {
      name: translate("student.invite.confirmsAdult", { name: contactFullName }),
    }),
  );
}
