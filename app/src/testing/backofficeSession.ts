import {
  getCurrentOperator,
  refreshBackofficeSession,
} from "@/backoffice/session/backofficeSessionApi";

export function signInToBackofficeAs(role: "PlatformOperator" | "PlatformReader"): void {
  jest.mocked(refreshBackofficeSession).mockResolvedValue("access-token");
  jest.mocked(getCurrentOperator).mockResolvedValue({ email: "operator@example.com", role });
}
