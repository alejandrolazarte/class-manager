export interface Instructor {
  id: string;
  fullName: string;
  isActive: boolean;
}

export interface SaveInstructorRequest {
  fullName: string;
}
