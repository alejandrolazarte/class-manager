import * as SecureStore from "expo-secure-store";
import {
  parseSeenMedals,
  SeenMedalsStorage,
  seenMedalsStorageKey,
} from "@/features/studentApp/seenMedals";

export const seenMedalsStorage: SeenMedalsStorage = {
  read: async (studentId) =>
    parseSeenMedals(await SecureStore.getItemAsync(seenMedalsStorageKey(studentId))),
  write: (studentId, medals) =>
    SecureStore.setItemAsync(seenMedalsStorageKey(studentId), JSON.stringify(medals)),
};
