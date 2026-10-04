import { authenticationSession } from "@/api/authenticationSession";
import { httpClient, isNetworkError } from "@/api/httpClient";

const longerThanAnyRequestMilliseconds = 120_000;

function fetchThatNeverAnswers(_url: string, requestInit: RequestInit): Promise<Response> {
  return new Promise((_resolve, reject) => {
    requestInit.signal?.addEventListener("abort", () => reject(requestInit.signal?.reason));
  });
}

describe("When the server does not answer", () => {
  beforeEach(() => {
    jest.useFakeTimers();
    authenticationSession.startSession("access-token");
    globalThis.fetch = jest.fn(fetchThatNeverAnswers) as unknown as typeof fetch;
  });

  afterEach(() => {
    authenticationSession.reset();
    jest.useRealTimers();
  });

  it("Then the request fails as a network error", async () => {
    const request = httpClient.post("/api/student-app/orders", {}).catch((error: unknown) => error);

    await jest.advanceTimersByTimeAsync(longerThanAnyRequestMilliseconds);

    expect(isNetworkError(await request)).toBe(true);
  });
});
