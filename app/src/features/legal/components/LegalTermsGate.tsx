import { PropsWithChildren } from "react";
import { useMyAccount } from "@/features/account/useMyAccount";
import { LegalTermsUpdateScreen } from "@/features/legal/screens/LegalTermsUpdateScreen";

export function LegalTermsGate({ children }: PropsWithChildren) {
  const { data: account } = useMyAccount();
  if (account?.hasAcceptedCurrentLegalTerms === false) {
    return <LegalTermsUpdateScreen />;
  }
  return children;
}
