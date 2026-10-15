import { anonymousHttpClient } from "@/api/httpClient";

jest.mock("@/api/refreshTokenCookieRequest", () =>
  jest.requireActual("@/api/refreshTokenCookieRequest.web"),
);

describe("When authentication request is sent from the web", () => {
  beforeEach(() => {
    globalThis.fetch = jest
      .fn()
      .mockResolvedValue(new Response(JSON.stringify({}), { status: 200 }));
  });

  it("Then the refresh token cookie is included", async () => {
    await anonymousHttpClient.post("/api/auth/refresh", {});

    const [, requestInit] = jest.mocked(globalThis.fetch).mock.calls[0];
    expect(requestInit?.credentials).toBe("include");
    expect(requestInit?.headers).toMatchObject({ "X-Refresh-Token-Cookie": "true" });
  });
});
