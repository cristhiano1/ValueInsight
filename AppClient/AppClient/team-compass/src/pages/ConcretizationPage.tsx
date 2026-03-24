import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { ArrowRight, ArrowLeft, Pen, Star } from "lucide-react";
import { useApp } from "@/context/AppContext";
import { useLanguage } from "@/context/LanguageContext";
import { useTranslatedData } from "@/hooks/useTranslatedData";
import { useNavigate } from "react-router-dom";
import { fadeInUp, fadeInUpDelay } from "@/lib/motion";

interface ConcretizationEntry {
  valueId: string;
  meaning: string;
  behavior: string;
  violated: string;
}

export default function ConcretizationPage() {
  const { rankedValues, setConcretizations } = useApp();
  const navigate = useNavigate();
  const { t } = useLanguage();
  const { values: ALL_VALUES, categories: CATEGORIES, concretizationPrompts } = useTranslatedData();
  const top3 = rankedValues.slice(0, 3);

  const [entries, setEntries] = useState<ConcretizationEntry[]>(
    top3.map((v) => ({ valueId: v.id, meaning: "", behavior: "", violated: "" }))
  );

  useEffect(() => {
    if (top3.length === 0) navigate("/prioritize");
  }, [top3, navigate]);

  const updateEntry = (index: number, field: keyof Omit<ConcretizationEntry, "valueId">, value: string) => {
    setEntries((prev) => {
      const updated = [...prev];
      updated[index] = { ...updated[index], [field]: value };
      return updated;
    });
  };

  const filledCount = entries.reduce((acc, e) => {
    return acc + (e.meaning.trim() ? 1 : 0) + (e.behavior.trim() ? 1 : 0) + (e.violated.trim() ? 1 : 0);
  }, 0);

  const getTranslated = (v: typeof rankedValues[0]) => ALL_VALUES.find((tv) => tv.id === v.id) ?? v;

  return (
    <div className="workspace-narrow">
      <motion.div {...fadeInUp} className="mb-8">
        <div className="flex items-center gap-3 mb-2">
          <Pen className="w-5 h-5 text-primary" />
          <h1 className="text-subhead font-semibold text-foreground">{t("concretizeTitle")}</h1>
        </div>
        <p className="text-ui-sm text-muted-foreground">{t("concretizeStep")}</p>
        <div className="flex items-center gap-2 mt-4">
          <div className="h-1.5 flex-1 bg-border rounded-full overflow-hidden">
            <motion.div
              className="h-full bg-primary rounded-full"
              animate={{ width: `${(filledCount / 9) * 100}%` }}
              transition={{ type: "spring", stiffness: 300, damping: 30 }}
            />
          </div>
          <span className="text-xs text-muted-foreground font-mono">{filledCount}/9</span>
        </div>
      </motion.div>

      <div className="space-y-6">
        {top3.map((value, vi) => {
          const tv = getTranslated(value);
          const cat = CATEGORIES.find((c) => c.id === value.category);
          return (
            <motion.div key={value.id} {...fadeInUpDelay(0.1 * vi)} className="card-surface p-6">
              <div className="flex items-center gap-3 mb-4">
                <Star className="w-4 h-4 text-warning fill-warning" />
                <h2 className="text-body font-semibold text-foreground">{tv.name}</h2>
                <span
                  className="px-2 py-0.5 rounded-full text-xs font-medium"
                  style={{ backgroundColor: `${cat?.color}15`, color: cat?.color }}
                >
                  {cat?.label.split("&")[0].trim()}
                </span>
              </div>
              <p className="text-ui-sm text-muted-foreground mb-4 italic">"{tv.description}"</p>

              {concretizationPrompts.map((prompt, pi) => {
                const field = (["meaning", "behavior", "violated"] as const)[pi];
                return (
                  <div key={pi} className="mb-4 last:mb-0">
                    <label className="block text-ui-sm font-medium text-foreground mb-2">
                      {prompt}
                    </label>
                    <textarea
                      value={entries[vi]?.[field] || ""}
                      onChange={(e) => updateEntry(vi, field, e.target.value)}
                      placeholder={t("writeYourThoughts") as string}
                      rows={2}
                      className="w-full p-3 rounded-inner border border-input bg-background text-ui-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring/20 focus:border-primary transition-colors resize-none"
                    />
                  </div>
                );
              })}
            </motion.div>
          );
        })}
      </div>

      <div className="flex items-center justify-between mt-8">
        <button
          onClick={() => navigate("/prioritize")}
          className="flex items-center gap-2 text-ui-sm font-medium text-muted-foreground hover:text-foreground transition-colors press-effect"
        >
          <ArrowLeft className="w-4 h-4" />
          {t("back")}
        </button>
        <button
          onClick={() => {
            setConcretizations(entries);
            navigate("/profile");
          }}
          className="flex items-center gap-2 h-10 px-5 bg-primary text-primary-foreground rounded-inner text-ui-sm font-medium hover:opacity-90 transition-opacity press-effect"
        >
          {t("viewMyProfile")}
          <ArrowRight className="w-4 h-4" />
        </button>
      </div>
    </div>
  );
}
