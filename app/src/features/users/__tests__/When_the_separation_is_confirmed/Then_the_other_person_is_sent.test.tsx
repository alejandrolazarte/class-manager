import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { separatePerson } from "@/features/users/separationApi";
import { SeparatePersonScreen } from "@/features/users/screens/SeparatePersonScreen";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/users/separationApi");

describe("When the separation is confirmed", () => {
  beforeEach(() => {
    jest.mocked(separatePerson).mockResolvedValue(undefined);
  });

  it("Then the other person is sent", async () => {
    await renderWithProviders(
      <SeparatePersonScreen role="student" id="student-1" currentFullName="Ana Gómez" />,
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("people.separate.fullName")),
      " Martina Gómez ",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("people.separate.submit") }),
    );

    await fireEvent.press(
      screen.getAllByRole("button", { name: translate("people.separate.confirm") })[0]!,
    );

    await waitFor(() =>
      expect(separatePerson).toHaveBeenCalledWith("student", "student-1", {
        fullName: "Martina Gómez",
        email: null,
      }),
    );
    await waitFor(() => expect(routerMock.back).toHaveBeenCalled());
  });
});
