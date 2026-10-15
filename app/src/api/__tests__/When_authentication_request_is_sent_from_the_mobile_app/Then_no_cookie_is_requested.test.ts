import { anonymousHttpClient } from "@/api/httpClient";

describe("When authentication request is sent from the mobile app", () => {
  beforeEach(() => {
    globalThis.fetch = jest
      .fn()
      .mockResolvedValue(new Response(JSON.stringify({}), { status: 200 }));
  });

  it("Then no cookie is requested", async () => {
    await anonymousHttpClient.post("/api/auth/refresh", { refreshToken: "stored-refresh-token" });

    const [, requestInit] = jest.mocked(globalThis.fetch).mock.calls[0];
    expect(requestInit?.credentials).toBeUndefined();
    expect(requestInit?.headers).not.toHaveProperty("X-Refresh-Token-Cookie");
  });
});
