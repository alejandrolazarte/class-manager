import { NetworkError, httpClient } from "@/api/httpClient";

describe("When request cannot reach server", () => {
  beforeEach(() => {
    globalThis.fetch = jest.fn().mockRejectedValue(new TypeError("Network request failed"));
  });

  it("Then network error is thrown", async () => {
    await expect(httpClient.get("/api/clients")).rejects.toBeInstanceOf(NetworkError);
  });
});
