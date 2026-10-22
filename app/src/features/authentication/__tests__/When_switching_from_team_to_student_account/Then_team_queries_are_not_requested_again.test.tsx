import { fireEvent, screen } from "@testing-library/react-native";
import { refreshSession, switchBranch } from "@/features/authentication/authenticationApi";
import { buildTokenResponse, storedRefreshToken } from "@/testing/authenticationFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";
import { SessionKindQueriesProbe } from "@/testing/SessionKindQueriesProbe";
import { SessionProbe, switchToStudentAccountProbeLabel } from "@/testing/SessionProbe";
import { buildStudentAccessToken } from "@/testing/studentAppFactory";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const loadTeamData = jest.fn();
const loadStudentData = jest.fn();

describe("When switching from team to student account", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(storedRefreshToken);
    jest.mocked(refreshSession).mockResolvedValue(buildTokenResponse());
    jest.mocked(switchBranch).mockResolvedValue(
      buildTokenResponse({
        accessToken: buildStudentAccessToken(),
        refreshToken: "student-refresh-token",
      }),
    );
    loadTeamData.mockReset().mockResolvedValue("team data");
    loadStudentData.mockReset().mockResolvedValue("student data");
  });

  it("Then team queries are not requested again", async () => {
    await renderWithSession(
      <>
        <SessionProbe />
        <SessionKindQueriesProbe loadTeamData={loadTeamData} loadStudentData={loadStudentData} />
      </>,
    );
    await screen.findByText("team data");

    await fireEvent.press(screen.getByRole("button", { name: switchToStudentAccountProbeLabel }));

    expect(await screen.findByText("student data")).toBeOnTheScreen();
    expect(loadTeamData).toHaveBeenCalledTimes(1);
  });
});
