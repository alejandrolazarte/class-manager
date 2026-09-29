import { ReactElement } from "react";
import { FamilyCartProvider } from "@/features/family/FamilyCartProvider";
import { FamilyStudentProvider } from "@/features/family/FamilyStudentProvider";
import { renderWithSession } from "@/testing/renderWithSession";

export function renderFamilyScreen(element: ReactElement) {
  return renderWithSession(
    <FamilyStudentProvider>
      <FamilyCartProvider>{element}</FamilyCartProvider>
    </FamilyStudentProvider>,
  );
}
