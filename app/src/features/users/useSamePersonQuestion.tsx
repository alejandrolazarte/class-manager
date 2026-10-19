import { useState } from "react";
import { SamePersonDialog } from "@/features/users/components/SamePersonDialog";
import { SamePerson, samePersonOf } from "@/features/users/samePerson";
import { TranslationKey } from "@/i18n/translate";

interface PendingQuestion {
  person: SamePerson;
  join: (samePersonUserId: string) => Promise<void>;
  goBack: () => void;
}

export function useSamePersonQuestion(messageKey: TranslationKey) {
  const [pendingQuestion, setPendingQuestion] = useState<PendingQuestion | null>(null);
  const [isJoining, setIsJoining] = useState(false);

  const askIfSamePerson = (
    requestError: unknown,
    email: string,
    join: (samePersonUserId: string) => Promise<void>,
    goBack: () => void,
  ): boolean => {
    const person = samePersonOf(requestError, email);
    if (person === null) {
      return false;
    }
    setPendingQuestion({ person, join, goBack });
    return true;
  };

  const confirm = async () => {
    if (pendingQuestion === null) {
      return;
    }
    setIsJoining(true);
    try {
      await pendingQuestion.join(pendingQuestion.person.userId);
    } finally {
      setIsJoining(false);
      setPendingQuestion((current) => (current === pendingQuestion ? null : current));
    }
  };

  const samePersonDialog =
    pendingQuestion === null ? null : (
      <SamePersonDialog
        person={pendingQuestion.person}
        messageKey={messageKey}
        onCancel={() => {
          setPendingQuestion(null);
          pendingQuestion.goBack();
        }}
        onConfirm={() => void confirm()}
        isConfirming={isJoining}
      />
    );

  return { askIfSamePerson, samePersonDialog };
}
