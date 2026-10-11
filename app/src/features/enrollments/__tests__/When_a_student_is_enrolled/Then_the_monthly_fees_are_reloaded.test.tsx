import { QueryClientProvider } from "@tanstack/react-query";
import { act, renderHook } from "@testing-library/react-native";
import { ReactNode } from "react";
import { enrollStudent } from "@/features/enrollments/enrollmentsApi";
import { useEnrollStudent } from "@/features/enrollments/useEnrollmentMutations";
import { feeQueryKeys } from "@/features/fees/feeQueryKeys";
import { createTestQueryClient } from "@/testing/testQueryClients";

jest.mock("@/features/enrollments/enrollmentsApi");

const classGroupId = "class-group";
const studentId = "student";

describe("When a student is enrolled", () => {
  it("Then the monthly fees are reloaded", async () => {
    jest.mocked(enrollStudent).mockResolvedValue({
      id: "enrollment",
      classGroupId,
      studentId,
      startDate: "2026-10-17",
      endDate: null,
    });
    const queryClient = createTestQueryClient();
    const invalidateQueries = jest.spyOn(queryClient, "invalidateQueries");
    const wrapper = ({ children }: { children: ReactNode }) => (
      <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    );
    const { result } = await renderHook(() => useEnrollStudent(classGroupId), { wrapper });

    await act(() => result.current.mutateAsync({ studentId }));

    expect(invalidateQueries).toHaveBeenCalledWith({ queryKey: feeQueryKeys.all });
  });
});
