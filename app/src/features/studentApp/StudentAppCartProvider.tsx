import {
  createContext,
  PropsWithChildren,
  useCallback,
  useContext,
  useMemo,
  useState,
} from "react";
import { cartUnits, StudentAppCart, withUnits } from "@/features/studentApp/studentAppCart";

interface StudentAppCartContextValue {
  cart: StudentAppCart;
  units: number;
  setUnits: (itemId: string, units: number) => void;
  addUnits: (itemId: string, units: number) => void;
  clear: () => void;
}

const emptyCart: StudentAppCart = {};

const StudentAppCartContext = createContext<StudentAppCartContextValue | null>(null);

export function StudentAppCartProvider({ children }: PropsWithChildren) {
  const [cart, setCart] = useState<StudentAppCart>(emptyCart);

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

  return (
    <StudentAppCartContext.Provider value={contextValue}>{children}</StudentAppCartContext.Provider>
  );
}

export function useStudentAppCart(): StudentAppCartContextValue {
  const contextValue = useContext(StudentAppCartContext);
  if (contextValue === null) {
    throw new Error("useStudentAppCart must be used inside StudentAppCartProvider");
  }
  return contextValue;
}
