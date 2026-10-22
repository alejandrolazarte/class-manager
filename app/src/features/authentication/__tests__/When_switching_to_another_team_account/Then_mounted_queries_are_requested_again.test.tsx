import { fireEvent, screen } from "@testing-library/react-native";
import { refreshSession, switchBranch } from "@/features/authentication/authenticationApi";
import { buildTokenResponse, storedRefreshToken } from "@/testing/authenticationFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";
import { SessionKindQueriesProbe } from "@/testing/SessionKindQueriesProbe";
import { SessionProbe, switchToTeamAccountProbeLabel } from "@/testing/SessionProbe";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const loadTeamData = jest.fn();
const loadStudentData = jest.fn();

describe("When switching to another team account", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(storedRefreshToken);
    jest.mocked(refreshSession).mockResolvedValue(buildTokenResponse());
    jest
      .mocked(switchBranch)
      .mockResolvedValue(buildTokenResponse({ refreshToken: "team-refresh-token" }));
    loadTeamData
      .mockReset()
      .mockResolvedValueOnce("team data")
      .mockResolvedValueOnce("other team data");
    loadStudentData.mockReset().mockResolvedValue("student data");
  });

  it("Then mounted queries are requested again", async () => {
    await renderWithSession(
      <>
        <SessionProbe />
        <SessionKindQueriesProbe loadTeamData={loadTeamData} loadStudentData={loadStudentData} />
      </>,
    );
    await screen.findByText("team data");

    await fireEvent.press(screen.getByRole("button", { name: switchToTeamAccountProbeLabel }));

    expect(await screen.findByText("other team data")).toBeOnTheScreen();
  });
});
