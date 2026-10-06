export interface Instructor {
  id: string;
  fullName: string;
  email: string | null;
  isActive: boolean;
}

export interface SaveInstructorRequest {
  fullName: string;
  email: string | null;
}

export type InstructorAppAccessStatus = "NotInvited" | "Invited" | "Active";
