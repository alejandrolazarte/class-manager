import { useState } from "react";
import { BirthDateField } from "@/forms/BirthDateField";
import { DateField } from "@/forms/DateField";
import { TimeField } from "@/forms/TimeField";

export const harnessDateLabel = "Fecha";
export const harnessTimeLabel = "Hora";

interface FieldHarnessProps {
  initialValue?: string;
}

export function DateFieldHarness({ initialValue = "" }: FieldHarnessProps) {
  const [typedDate, setTypedDate] = useState(initialValue);
  return <DateField label={harnessDateLabel} value={typedDate} onChangeText={setTypedDate} />;
}

export function BirthDateFieldHarness({ initialValue = "" }: FieldHarnessProps) {
  const [typedDate, setTypedDate] = useState(initialValue);
  return <BirthDateField label={harnessDateLabel} value={typedDate} onChangeText={setTypedDate} />;
}

export function TimeFieldHarness({ initialValue = "" }: FieldHarnessProps) {
  const [typedTime, setTypedTime] = useState(initialValue);
  return <TimeField label={harnessTimeLabel} value={typedTime} onChangeText={setTypedTime} />;
}
