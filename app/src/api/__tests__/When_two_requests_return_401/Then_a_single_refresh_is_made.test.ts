import { authenticationSession } from "@/api/authenticationSession";
import { httpClient } from "@/api/httpClient";
import { respondByBearerToken } from "@/testing/fetchResponses";

const expiredAccessToken = "expired-access-token";
const refreshedAccessToken = "refreshed-access-token";

describe("When two requests return 401", () => {
  const refreshAccessToken = jest.fn();

  beforeEach(() => {
    refreshAccessToken.mockResolvedValue(refreshedAccessToken);
    authenticationSession.configure({ refreshAccessToken, onSessionEnded: jest.fn() });
    authenticationSession.startSession(expiredAccessToken);
    globalThis.fetch = respondByBearerToken(refreshedAccessToken, []) as typeof fetch;
  });

  afterEach(() => {
    authenticationSession.reset();
  });

  it("Then a single refresh is made", async () => {
    await Promise.all([httpClient.get("/api/employees"), httpClient.get("/api/service-offerings")]);

    expect(refreshAccessToken).toHaveBeenCalledTimes(1);
  });
});
