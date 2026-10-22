import { screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppSettingsScreen } from "@/features/studentApp/screens/StudentAppSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const locationName = "Av. Costanera 1450, Ciudad";

describe("When the business location has no link", () => {
  beforeEach(() => {
    jest
      .mocked(getStudentAppHome)
      .mockResolvedValue(buildStudentAppHome({ location: { name: locationName, mapsUrl: null } }));
  });

  it("Then the address is shown without a button", async () => {
    await renderStudentAppScreen(<StudentAppSettingsScreen />);

    expect(await screen.findByText(locationName)).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: locationName })).toBeNull();
    expect(screen.queryByText(translate("student.location.openMaps"))).toBeNull();
  });
});
