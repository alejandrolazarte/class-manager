export interface MyAccount {
  email: string;
  fullName: string;
  birthDate?: string | null;
}

export interface UpdateMyProfileRequest {
  fullName: string;
  birthDate: string;
}

export interface RequestEmailChangeRequest {
  newEmail: string;
  currentPassword: string;
}

export interface RequestEmailChangeResponse {
  newEmail: string;
}

export interface ConfirmEmailChangeResponse {
  newEmail: string;
}
