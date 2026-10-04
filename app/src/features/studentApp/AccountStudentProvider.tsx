import { createContext, PropsWithChildren, useContext, useMemo, useState } from "react";
import { AccountStudent } from "@/features/studentApp/types";

interface AccountStudentContextValue {
  selectedStudentId: string | null;
  selectStudent: (studentId: string) => void;
}

const AccountStudentContext = createContext<AccountStudentContextValue | null>(null);

export function AccountStudentProvider({ children }: PropsWithChildren) {
  const [selectedStudentId, setSelectedStudentId] = useState<string | null>(null);
  const contextValue = useMemo(
    () => ({ selectedStudentId, selectStudent: setSelectedStudentId }),
    [selectedStudentId],
  );
  return (
    <AccountStudentContext.Provider value={contextValue}>{children}</AccountStudentContext.Provider>
  );
}

export function useSelectedStudent(students: readonly AccountStudent[]) {
  const contextValue = useContext(AccountStudentContext);
  if (contextValue === null) {
    throw new Error("useSelectedStudent must be used inside AccountStudentProvider");
  }
  const student =
    students.find((candidate) => candidate.id === contextValue.selectedStudentId) ??
    students[0] ??
    null;
  return { student, selectStudent: contextValue.selectStudent };
}
