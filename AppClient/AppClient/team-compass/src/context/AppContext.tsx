import React, { createContext, useContext, useState, ReactNode } from "react";
import { Value } from "@/data/mockData";

export type AssessmentStep =
  | "intro"
  | "reflection"
  | "select10"
  | "reduceTo5"
  | "rankTop3"
  | "concretize"
  | "profile"
  | "team";

interface Concretization {
  valueId: string;
  meaning: string;
  behavior: string;
  violated: string;
}

interface AppState {
  currentStep: AssessmentStep;
  selectedValues: Value[];       // up to 10
  narrowedValues: Value[];       // 5
  rankedValues: Value[];         // top 3
  reflections: string[];         // free-text reflection answers
  concretizations: Concretization[];
  onboardingStep: number;
  isAuthenticated: boolean;
  setCurrentStep: (step: AssessmentStep) => void;
  setSelectedValues: (values: Value[]) => void;
  toggleValue: (value: Value, maxCount?: number) => void;
  setNarrowedValues: (values: Value[]) => void;
  setRankedValues: (values: Value[]) => void;
  setReflections: (r: string[]) => void;
  setConcretizations: (c: Concretization[]) => void;
  setOnboardingStep: (step: number) => void;
  setIsAuthenticated: (auth: boolean) => void;
}

const AppContext = createContext<AppState | undefined>(undefined);

export function AppProvider({ children }: { children: ReactNode }) {
  const [currentStep, setCurrentStep] = useState<AssessmentStep>("intro");
  const [selectedValues, setSelectedValues] = useState<Value[]>([]);
  const [narrowedValues, setNarrowedValues] = useState<Value[]>([]);
  const [rankedValues, setRankedValues] = useState<Value[]>([]);
  const [reflections, setReflections] = useState<string[]>(["", "", "", ""]);
  const [concretizations, setConcretizations] = useState<Concretization[]>([]);
  const [onboardingStep, setOnboardingStep] = useState(0);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  const toggleValue = (value: Value, maxCount = 10) => {
    setSelectedValues((prev) => {
      const exists = prev.find((v) => v.id === value.id);
      if (exists) return prev.filter((v) => v.id !== value.id);
      if (prev.length >= maxCount) return prev;
      return [...prev, value];
    });
  };

  return (
    <AppContext.Provider
      value={{
        currentStep, selectedValues, narrowedValues, rankedValues,
        reflections, concretizations, onboardingStep, isAuthenticated,
        setCurrentStep, setSelectedValues, toggleValue,
        setNarrowedValues, setRankedValues, setReflections,
        setConcretizations, setOnboardingStep, setIsAuthenticated,
      }}
    >
      {children}
    </AppContext.Provider>
  );
}

export function useApp() {
  const context = useContext(AppContext);
  if (!context) throw new Error("useApp must be used within AppProvider");
  return context;
}
