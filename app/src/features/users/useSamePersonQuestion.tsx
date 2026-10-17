import { useState } from "react";
import { SamePersonDialog } from "@/features/users/components/SamePersonDialog";
import { SamePersonQuestion, toSamePersonQuestion } from "@/features/users/samePersonQuestion";

interface PendingQuestion {
  question: SamePersonQuestion;
  join: (samePersonUserId: string) => Promise<void>;
}

export function useSamePersonQuestion() {
  const [pendingQuestion, setPendingQuestion] = useState<PendingQuestion | null>(null);
  const [isJoining, setIsJoining] = useState(false);

  const askIfSamePerson = (
    error: unknown,
    join: (samePersonUserId: string) => Promise<void>,
  ): boolean => {
    const question = toSamePersonQuestion(error);
    if (question === null) {
      return false;
    }
    setPendingQuestion({ question, join });
    return true;
  };

  const confirm = async () => {
    if (pendingQuestion === null) {
      return;
    }
    setIsJoining(true);
    try {
      await pendingQuestion.join(pendingQuestion.question.userId);
    } finally {
      setIsJoining(false);
      setPendingQuestion((current) => (current === pendingQuestion ? null : current));
    }
  };

  const samePersonDialog =
    pendingQuestion === null ? null : (
      <SamePersonDialog
        question={pendingQuestion.question}
        onCancel={() => setPendingQuestion(null)}
        onConfirm={() => void confirm()}
        isConfirming={isJoining}
      />
    );

  return { askIfSamePerson, samePersonDialog };
}
