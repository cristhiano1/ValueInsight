import { useMemo } from "react";
import { useLanguage } from "@/context/LanguageContext";
import {
  VALUES, CATEGORIES, CULTURE_TYPES, POLARITY_PAIRS, CFS_INTERPRETATION,
  AI_INSIGHTS, REFLECTION_QUESTIONS, CONCRETIZATION_PROMPTS,
  Value, CultureType, PolarityPair, AIInsight,
} from "@/data/mockData";

export function useTranslatedData() {
  const { t } = useLanguage();

  return useMemo(() => {
    const valueNames = t("valueNames") as Record<string, string>;
    const valueDescs = t("valueDescriptions") as Record<string, string>;
    const catLabels = t("categoryLabels") as Record<string, string>;
    const catDescs = t("categoryDescriptions") as Record<string, string>;
    const ctNames = t("cultureTypeNames") as Record<string, string>;
    const ctChars = t("cultureTypeCharacteristics") as Record<string, string[]>;
    const ctStrengths = t("cultureTypeStrengths") as Record<string, string[]>;
    const ctRisks = t("cultureTypeRisks") as Record<string, string[]>;
    const polLabels = t("polarityLabels") as Record<string, string>;
    const cfsInterp = t("cfsInterpretation") as Array<{ label: string; description: string }>;
    const aiTitles = t("aiInsightTitles") as Record<string, string>;
    const aiDescs = t("aiInsightDescriptions") as Record<string, string>;
    const reflQuestions = t("reflectionQuestions") as string[];
    const concPrompts = t("concretizationPrompts") as string[];

    const translatedValues: Value[] = VALUES.map((v) => ({
      ...v,
      name: valueNames?.[v.id] ?? v.name,
      description: valueDescs?.[v.id] ?? v.description,
    }));

    const translatedCategories = CATEGORIES.map((c) => ({
      ...c,
      label: catLabels?.[c.id] ?? c.label,
      description: catDescs?.[c.id] ?? c.description,
    }));

    const translatedCultureTypes: CultureType[] = CULTURE_TYPES.map((ct) => ({
      ...ct,
      name: ctNames?.[ct.id] ?? ct.name,
      characteristics: ctChars?.[ct.id] ?? ct.characteristics,
      strengths: ctStrengths?.[ct.id] ?? ct.strengths,
      risks: ctRisks?.[ct.id] ?? ct.risks,
    }));

    const polarityLabelMap: Record<string, string> = {
      Freedom: polLabels?.freedom ?? "Freedom",
      Structure: polLabels?.structureLabel ?? "Structure",
      Results: polLabels?.results ?? "Results",
      Relationships: polLabels?.relationships ?? "Relationships",
      Innovation: polLabels?.innovation ?? "Innovation",
      Stability: polLabels?.stability ?? "Stability",
      Control: polLabels?.control ?? "Control",
      Trust: polLabels?.trust ?? "Trust",
    };

    const translatedPolarityPairs: PolarityPair[] = POLARITY_PAIRS.map((p) => ({
      ...p,
      labelA: polarityLabelMap[p.labelA] ?? p.labelA,
      labelB: polarityLabelMap[p.labelB] ?? p.labelB,
    }));

    const translatedCfsInterpretation = CFS_INTERPRETATION.map((item, i) => ({
      ...item,
      label: cfsInterp?.[i]?.label ?? item.label,
      description: cfsInterp?.[i]?.description ?? item.description,
    }));

    const translatedAiInsights: AIInsight[] = AI_INSIGHTS.map((ins) => ({
      ...ins,
      title: aiTitles?.[ins.id] ?? ins.title,
      description: aiDescs?.[ins.id] ?? ins.description,
    }));

    const translatedReflectionQuestions: string[] = reflQuestions ?? REFLECTION_QUESTIONS;
    const translatedConcretizationPrompts: string[] = concPrompts ?? CONCRETIZATION_PROMPTS;

    // Helper: determineCultureType using translated types
    const determineCultureTypeTranslated = (scores: Record<string, number>): CultureType => {
      const sorted = (Object.entries(scores) as [string, number][]).sort((a, b) => b[1] - a[1]);
      const top2 = new Set([sorted[0][0], sorted[1][0]]);
      for (const ct of translatedCultureTypes) {
        if (top2.has(ct.dominantCategories[0]) && top2.has(ct.dominantCategories[1])) {
          return ct;
        }
      }
      return translatedCultureTypes.find((ct) => top2.has(ct.dominantCategories[0])) || translatedCultureTypes[0];
    };

    return {
      values: translatedValues,
      categories: translatedCategories,
      cultureTypes: translatedCultureTypes,
      polarityPairs: translatedPolarityPairs,
      cfsInterpretation: translatedCfsInterpretation,
      aiInsights: translatedAiInsights,
      reflectionQuestions: translatedReflectionQuestions,
      concretizationPrompts: translatedConcretizationPrompts,
      determineCultureType: determineCultureTypeTranslated,
    };
  }, [t]);
}
