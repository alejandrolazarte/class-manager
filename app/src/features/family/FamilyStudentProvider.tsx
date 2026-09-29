import { createContext, PropsWithChildren, useContext, useMemo, useState } from "react";
import { FamilyStudent } from "@/features/family/types";

interface FamilyStudentContextValue {
  selectedStudentId: string | null;
  selectStudent: (studentId: string) => void;
}

const FamilyStudentContext = createContext<FamilyStudentContextValue | null>(null);

export function FamilyStudentProvider({ children }: PropsWithChildren) {
  const [selectedStudentId, setSelectedStudentId] = useState<string | null>(null);
  const contextValue = useMemo(
    () => ({ selectedStudentId, selectStudent: setSelectedStudentId }),
    [selectedStudentId],
  );
  return (
    <FamilyStudentContext.Provider value={contextValue}>{children}</FamilyStudentContext.Provider>
  );
}

export function useSelectedStudent(students: readonly FamilyStudent[]) {
  const contextValue = useContext(FamilyStudentContext);
  if (contextValue === null) {
    throw new Error("useSelectedStudent must be used inside FamilyStudentProvider");
  }
  const student =
    students.find((candidate) => candidate.id === contextValue.selectedStudentId) ??
    students[0] ??
    null;
  return { student, selectStudent: contextValue.selectStudent };
}
