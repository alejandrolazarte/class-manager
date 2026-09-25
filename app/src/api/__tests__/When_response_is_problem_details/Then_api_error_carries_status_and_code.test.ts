import { ApiError, httpClient } from "@/api/httpClient";

const conflictStatus = 409;
const phoneNumberTakenCode = "client.phone_number_taken";
const existingClientId = "0192f0c4-0000-7000-8000-000000000001";

describe("When response is problem details", () => {
  beforeEach(() => {
    globalThis.fetch = jest.fn().mockResolvedValue(
      new Response(
        JSON.stringify({
          status: conflictStatus,
          title: "Conflict",
          code: phoneNumberTakenCode,
          clientId: existingClientId,
        }),
        { status: conflictStatus, headers: { "Content-Type": "application/problem+json" } },
      ),
    );
  });

  it("Then api error carries status and code", async () => {
    const request = httpClient.post("/api/clients", { fullName: "Ana" });

    await expect(request).rejects.toMatchObject({
      status: conflictStatus,
      problem: { code: phoneNumberTakenCode, clientId: existingClientId },
    });
    await expect(request).rejects.toBeInstanceOf(ApiError);
  });
});
