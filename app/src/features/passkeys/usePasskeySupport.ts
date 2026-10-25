import { useEffect, useState } from "react";
import { isPasskeySupported } from "@/features/passkeys/webAuthn";

export function usePasskeySupport(): boolean {
  const [isSupported, setIsSupported] = useState(false);

  useEffect(() => {
    let isMounted = true;
    isPasskeySupported().then((supported) => {
      if (isMounted) {
        setIsSupported(supported);
      }
    });
    return () => {
      isMounted = false;
    };
  }, []);

  return isSupported;
}
