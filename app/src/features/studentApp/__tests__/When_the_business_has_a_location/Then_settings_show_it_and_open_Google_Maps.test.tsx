import { fireEvent, screen } from "@testing-library/react-native";
import { Linking } from "react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppSettingsScreen } from "@/features/studentApp/screens/StudentAppSettingsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const location = {
  name: "Av. Costanera 1450, Ciudad",
  mapsUrl: "https://maps.app.goo.gl/k3Hq8vPzR2",
};

describe("When the business has a location", () => {
  beforeEach(() => {
    jest.spyOn(Linking, "openURL").mockResolvedValue(true);
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome({ location }));
  });

  it("Then settings show it and open Google Maps", async () => {
    await renderStudentAppScreen(<StudentAppSettingsScreen />);

    expect(await screen.findByText(translate("settings.group.location"))).toBeOnTheScreen();
    await fireEvent.press(screen.getByRole("button", { name: location.name }));

    expect(Linking.openURL).toHaveBeenCalledWith(location.mapsUrl);
  });
});
