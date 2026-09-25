import { ApiError, NetworkError } from "@/api/apiErrors";
import { authenticationSession } from "@/api/authenticationSession";
import { ProblemDetails } from "@/api/problemDetails";
import { appConfiguration } from "@/config/appConfiguration";

export { ApiError, isApiError, isNetworkError, NetworkError } from "@/api/apiErrors";

const jsonContentType = "application/json";
const noContentStatus = 204;
const unauthorizedStatus = 401;
const authorizationHeader = "Authorization";
const bearerScheme = "Bearer";

type QueryParameters = Record<string, string | number | undefined>;

interface RequestOptions {
  isAnonymous: boolean;
}

const authenticatedRequest: RequestOptions = { isAnonymous: false };
const anonymousRequest: RequestOptions = { isAnonymous: true };

function buildUrl(path: string, queryParameters?: QueryParameters): string {
  const url = new URL(path, appConfiguration.apiBaseUrl);
  for (const [parameterName, parameterValue] of Object.entries(queryParameters ?? {})) {
    if (parameterValue !== undefined && parameterValue !== "") {
      url.searchParams.set(parameterName, String(parameterValue));
    }
  }
  return url.toString();
}

async function readProblem(response: Response): Promise<ProblemDetails> {
  try {
    const problem = (await response.json()) as ProblemDetails;
    return { ...problem, status: problem.status ?? response.status };
  } catch {
    return { status: response.status, title: response.statusText };
  }
}

async function fetchWithHeaders(
  url: string,
  requestInit: RequestInit,
  accessToken: string | null,
): Promise<Response> {
  try {
    return await fetch(url, {
      ...requestInit,
      headers: {
        Accept: jsonContentType,
        "Content-Type": jsonContentType,
        ...(accessToken !== null
          ? { [authorizationHeader]: `${bearerScheme} ${accessToken}` }
          : {}),
      },
    });
  } catch (fetchError) {
    throw new NetworkError(fetchError);
  }
}

async function resolveAccessTokenForRetry(
  rejectedAccessToken: string | null,
): Promise<string | null> {
  const currentAccessToken = authenticationSession.getAccessToken();
  if (currentAccessToken !== null && currentAccessToken !== rejectedAccessToken) {
    return currentAccessToken;
  }
  return authenticationSession.refreshAccessToken();
}

async function fetchAuthenticated(url: string, requestInit: RequestInit): Promise<Response> {
  const accessToken = authenticationSession.getAccessToken();
  const response = await fetchWithHeaders(url, requestInit, accessToken);
  if (response.status !== unauthorizedStatus) {
    return response;
  }
  const retryAccessToken = await resolveAccessTokenForRetry(accessToken);
  return retryAccessToken === null
    ? response
    : fetchWithHeaders(url, requestInit, retryAccessToken);
}

async function send<TResponse>(
  url: string,
  requestInit: RequestInit,
  { isAnonymous }: RequestOptions,
): Promise<TResponse> {
  const response = isAnonymous
    ? await fetchWithHeaders(url, requestInit, null)
    : await fetchAuthenticated(url, requestInit);

  if (!response.ok) {
    throw new ApiError(response.status, await readProblem(response));
  }
  if (response.status === noContentStatus) {
    return undefined as TResponse;
  }
  return (await response.json()) as TResponse;
}

export const httpClient = {
  get<TResponse>(path: string, queryParameters?: QueryParameters): Promise<TResponse> {
    return send<TResponse>(
      buildUrl(path, queryParameters),
      { method: "GET" },
      authenticatedRequest,
    );
  },
  post<TResponse>(path: string, body: unknown): Promise<TResponse> {
    return send<TResponse>(
      buildUrl(path),
      { method: "POST", body: JSON.stringify(body) },
      authenticatedRequest,
    );
  },
  put<TResponse>(path: string, body: unknown): Promise<TResponse> {
    return send<TResponse>(
      buildUrl(path),
      { method: "PUT", body: JSON.stringify(body) },
      authenticatedRequest,
    );
  },
  delete<TResponse>(path: string): Promise<TResponse> {
    return send<TResponse>(buildUrl(path), { method: "DELETE" }, authenticatedRequest);
  },
};

export const anonymousHttpClient = {
  post<TResponse>(path: string, body: unknown): Promise<TResponse> {
    return send<TResponse>(
      buildUrl(path),
      { method: "POST", body: JSON.stringify(body) },
      anonymousRequest,
    );
  },
};
