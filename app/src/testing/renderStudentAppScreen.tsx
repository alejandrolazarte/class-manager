import { ReactElement } from "react";
import { StudentAppCartProvider } from "@/features/studentApp/StudentAppCartProvider";
import { AccountStudentProvider } from "@/features/studentApp/AccountStudentProvider";
import { renderWithSession } from "@/testing/renderWithSession";

export function renderStudentAppScreen(element: ReactElement) {
  return renderWithSession(
    <AccountStudentProvider>
      <StudentAppCartProvider>{element}</StudentAppCartProvider>
    </AccountStudentProvider>,
  );
}
