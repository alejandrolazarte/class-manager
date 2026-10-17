import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { instructorErrorCodes } from "@/features/instructors/instructorErrorCodes";
import { separatePerson } from "@/features/users/separationApi";
import { SeparatePersonScreen } from "@/features/users/screens/SeparatePersonScreen";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/users/separationApi");

const conflictStatus = 409;

describe("When the separated instructor is on the team", () => {
  beforeEach(() => {
    jest
      .mocked(separatePerson)
      .mockRejectedValue(new ApiError(conflictStatus, { code: instructorErrorCodes.onTheTeam }));
  });

  it("Then the reason is shown", async () => {
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

    await fireEvent.press(
      screen.getAllByRole("button", { name: translate("people.separate.confirm") })[0]!,
    );

    expect(await screen.findByText(translate("people.separate.onTheTeam"))).toBeOnTheScreen();
    expect(routerMock.back).not.toHaveBeenCalled();
  });
});
