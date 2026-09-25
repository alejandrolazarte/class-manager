import { useContext } from "react";
import { SessionContext, SessionContextValue } from "@/features/authentication/SessionProvider";

export function useSession(): SessionContextValue {
  const sessionContextValue = useContext(SessionContext);
  if (sessionContextValue === null) {
    throw new Error("useSession must be used inside SessionProvider");
  }
  return sessionContextValue;
}
