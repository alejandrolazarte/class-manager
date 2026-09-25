const unauthorizedStatus = 401;
const okStatus = 200;

export function unauthorizedResponse(): Response {
  return new Response(JSON.stringify({ status: unauthorizedStatus, title: "Unauthorized" }), {
    status: unauthorizedStatus,
  });
}

export function okResponse(body: unknown): Response {
  return new Response(JSON.stringify(body), { status: okStatus });
}

export function respondByBearerToken(acceptedAccessToken: string, body: unknown) {
  return jest.fn(async (_url: string, requestInit?: RequestInit) => {
    const headers = (requestInit?.headers ?? {}) as Record<string, string>;
    return headers.Authorization === `Bearer ${acceptedAccessToken}`
      ? okResponse(body)
      : unauthorizedResponse();
  });
}
