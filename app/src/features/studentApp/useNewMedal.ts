import { useEffect, useState } from "react";
import { seenMedalsStorage } from "@/features/studentApp/seenMedalsStorage";
import { firstNewMedal } from "@/features/studentApp/studentAppAchievements";
import { AccountStudent, Medal } from "@/features/studentApp/types";

const medalSeparator = ",";

function medalsFromKey(medalsKey: string): Medal[] {
  return medalsKey === "" ? [] : (medalsKey.split(medalSeparator) as Medal[]);
}

export function useNewMedal(student: AccountStudent | null) {
  const [newMedal, setNewMedal] = useState<Medal | null>(null);
  const studentId = student?.id ?? null;
  const medalsKey = student?.attendance.medals.join(medalSeparator) ?? "";

  useEffect(() => {
    if (studentId === null) {
      return;
    }
    const medals = medalsFromKey(medalsKey);
    let isCurrent = true;
    void seenMedalsStorage.read(studentId).then((seenMedals) => {
      if (!isCurrent) {
        return;
      }
      if (seenMedals === null) {
        void seenMedalsStorage.write(studentId, medals);
        return;
      }
      setNewMedal(firstNewMedal(medals, seenMedals));
    });
    return () => {
      isCurrent = false;
    };
  }, [studentId, medalsKey]);

  function dismissNewMedal() {
    if (studentId !== null) {
      void seenMedalsStorage.write(studentId, medalsFromKey(medalsKey));
    }
    setNewMedal(null);
  }

  return { newMedal, dismissNewMedal };
}
