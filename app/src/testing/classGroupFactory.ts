import { ClassGroup } from "@/features/classGroups/types";
import { Instructor } from "@/features/instructors/types";

export function buildInstructor(overrides: Partial<Instructor> = {}): Instructor {
  return {
    id: "0192f0d1-0000-7000-8000-000000000001",
    fullName: "Laura Gómez",
    email: null,
    isActive: true,
    ...overrides,
  };
}

export function buildClassGroup(overrides: Partial<ClassGroup> = {}): ClassGroup {
  return {
    id: "0192f0d2-0000-7000-8000-000000000001",
    name: "Natación inicial",
    instructorId: "0192f0d1-0000-7000-8000-000000000001",
    instructorFullName: "Laura Gómez",
    weekdays: ["Tuesday", "Thursday"],
    startTime: "18:00",
    endTime: "18:45",
    durationMinutes: 45,
    capacity: 8,
    location: "Pileta chica",
    materialUrl: null,
    isActive: true,
    enrolledCount: 0,
    ...overrides,
  };
}
