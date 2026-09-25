import { authenticationSession } from "@/api/authenticationSession";
import { httpClient } from "@/api/httpClient";
import { respondByBearerToken } from "@/testing/fetchResponses";

const expiredAccessToken = "expired-access-token";
const refreshedAccessToken = "refreshed-access-token";
const employees = [{ id: "0192f0c5-0000-7000-8000-000000000001", fullName: "Laura Gómez" }];

describe("When request returns 401", () => {
  const refreshAccessToken = jest.fn();

  beforeEach(() => {
    refreshAccessToken.mockResolvedValue(refreshedAccessToken);
    authenticationSession.configure({ refreshAccessToken, onSessionEnded: jest.fn() });
    authenticationSession.startSession(expiredAccessToken);
    globalThis.fetch = respondByBearerToken(refreshedAccessToken, employees) as typeof fetch;
  });

  afterEach(() => {
    authenticationSession.reset();
  });

  it("Then token is refreshed once and request retried", async () => {
    const response = await httpClient.get("/api/employees");

    expect(response).toEqual(employees);
    expect(refreshAccessToken).toHaveBeenCalledTimes(1);
    expect(globalThis.fetch).toHaveBeenCalledTimes(2);
  });
});
