export interface MyPhoto {
  url: string | null;
  awaitsGuardian: boolean;
  needsGuardian: boolean;
}

export interface PhotoApproval {
  id: string;
  studentFullName: string;
  photoUrl: string;
}
