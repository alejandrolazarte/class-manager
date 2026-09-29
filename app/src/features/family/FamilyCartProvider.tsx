import {
  createContext,
  PropsWithChildren,
  useCallback,
  useContext,
  useMemo,
  useState,
} from "react";
import { cartUnits, FamilyCart, withUnits } from "@/features/family/familyCart";

interface FamilyCartContextValue {
  cart: FamilyCart;
  units: number;
  setUnits: (itemId: string, units: number) => void;
  addUnits: (itemId: string, units: number) => void;
  clear: () => void;
}

const emptyCart: FamilyCart = {};

const FamilyCartContext = createContext<FamilyCartContextValue | null>(null);

export function FamilyCartProvider({ children }: PropsWithChildren) {
  const [cart, setCart] = useState<FamilyCart>(emptyCart);

  const setUnits = useCallback(
    (itemId: string, units: number) => setCart((current) => withUnits(current, itemId, units)),
    [],
  );
  const addUnits = useCallback(
    (itemId: string, units: number) =>
      setCart((current) => withUnits(current, itemId, (current[itemId] ?? 0) + units)),
    [],
  );
  const clear = useCallback(() => setCart(emptyCart), []);

  const contextValue = useMemo(
    () => ({ cart, units: cartUnits(cart), setUnits, addUnits, clear }),
    [cart, setUnits, addUnits, clear],
  );

  return <FamilyCartContext.Provider value={contextValue}>{children}</FamilyCartContext.Provider>;
}

export function useFamilyCart(): FamilyCartContextValue {
  const contextValue = useContext(FamilyCartContext);
  if (contextValue === null) {
    throw new Error("useFamilyCart must be used inside FamilyCartProvider");
  }
  return contextValue;
}
