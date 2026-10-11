import { screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { getTeam } from "@/features/members/membersApi";
import { TeamScreen } from "@/features/members/screens/TeamScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/members/membersApi");
jest.mock("@/features/instructors/instructorsApi");

describe("When team is loading", () => {
  beforeEach(() => {
    jest.mocked(getTeam).mockImplementation(() => new Promise(() => undefined));
    jest.mocked(listInstructorsIncludingInactive).mockResolvedValue([]);
  });

  it("Then a skeleton list is shown", async () => {
    await renderWithProviders(<TeamScreen />);

    expect(
      screen.getByRole("progressbar", { name: translate("common.loading") }),
    ).toBeOnTheScreen();
  });
});
