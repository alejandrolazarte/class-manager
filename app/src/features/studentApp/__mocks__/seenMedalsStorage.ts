import { SeenMedalsStorage } from "@/features/studentApp/seenMedals";

export const seenMedalsStorage: SeenMedalsStorage = {
  read: jest.fn(async () => null),
  write: jest.fn(async () => undefined),
};
