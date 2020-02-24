export interface UserRegistration {
  username: string;
  password: string;
  email: string;
  emailConfirmationUrl: string;
}

export interface EmailConfirmation {
  confirmationId: string
}
