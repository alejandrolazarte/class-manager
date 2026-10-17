import { fireEvent, screen } from "@testing-library/react-native";
import { separatePerson } from "@/features/users/separationApi";
import { SeparatePersonScreen } from "@/features/users/screens/SeparatePersonScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/users/separationApi");

describe("When the separation is not confirmed", () => {
  it("Then nothing is separated", async () => {
    await renderWithProviders(
      <SeparatePersonScreen role="instructor" id="instructor-1" currentFullName="Ana Gómez" />,
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("people.separate.fullName")),
      "Martina Gómez",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("people.separate.submit") }),
    );

    await fireEvent.press(screen.getAllByRole("button", { name: translate("common.back") })[1]!);

    expect(separatePerson).not.toHaveBeenCalled();
  });
});
