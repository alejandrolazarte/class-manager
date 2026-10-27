import {
  parseSeenMedals,
  SeenMedalsStorage,
  seenMedalsStorageKey,
} from "@/features/studentApp/seenMedals";

export const seenMedalsStorage: SeenMedalsStorage = {
  read: async (studentId) =>
    parseSeenMedals(globalThis.localStorage?.getItem(seenMedalsStorageKey(studentId)) ?? null),
  write: async (studentId, medals) => {
    globalThis.localStorage?.setItem(seenMedalsStorageKey(studentId), JSON.stringify(medals));
  },
};
