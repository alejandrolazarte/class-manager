import { authenticationSession } from "@/api/authenticationSession";
import { ApiError, httpClient } from "@/api/httpClient";
import { unauthorizedResponse } from "@/testing/fetchResponses";

const expiredAccessToken = "expired-access-token";
const unauthorizedStatus = 401;

describe("When refresh fails", () => {
  const onSessionEnded = jest.fn();

  beforeEach(() => {
    const refreshAccessToken = jest
      .fn()
      .mockRejectedValue(new ApiError(unauthorizedStatus, { code: "auth.invalid_refresh_token" }));
    authenticationSession.configure({ refreshAccessToken, onSessionEnded });
    authenticationSession.startSession(expiredAccessToken);
    globalThis.fetch = jest.fn().mockImplementation(async () => unauthorizedResponse());
  });

  afterEach(() => {
    authenticationSession.reset();
  });

  it("Then session is cleared", async () => {
    await expect(httpClient.get("/api/employees")).rejects.toMatchObject({
      status: unauthorizedStatus,
    });

    expect(onSessionEnded).toHaveBeenCalledTimes(1);
    expect(authenticationSession.getAccessToken()).toBeNull();
  });
});
