import { motion } from "framer-motion";
import { ArrowRight, ArrowLeft, Check, X } from "lucide-react";
import { useApp } from "@/context/AppContext";
import { useLanguage } from "@/context/LanguageContext";
import { useTranslatedData } from "@/hooks/useTranslatedData";
import { useNavigate } from "react-router-dom";
import { fadeInUp, fadeInUpDelay } from "@/lib/motion";

export default function NarrowValuesPage() {
  const { selectedValues, narrowedValues, setNarrowedValues } = useApp();
  const navigate = useNavigate();
  const { t } = useLanguage();
  const { categories: CATEGORIES, values: ALL_VALUES } = useTranslatedData();

  const isNarrowed = (id: string) => narrowedValues.some((v) => v.id === id);

  const toggleNarrowed = (value: typeof selectedValues[0]) => {
    if (isNarrowed(value.id)) {
      setNarrowedValues(narrowedValues.filter((v) => v.id !== value.id));
    } else if (narrowedValues.length < 5) {
      setNarrowedValues([...narrowedValues, value]);
    }
  };

  // Get translated version of selected values
  const getTranslated = (v: typeof selectedValues[0]) => {
    const found = ALL_VALUES.find((tv) => tv.id === v.id);
    return found ?? v;
  };

  return (
    <div className="workspace-narrow">
      <motion.div {...fadeInUp} className="mb-8">
        <h1 className="text-subhead font-semibold text-foreground">{t("narrowTitle")}</h1>
        <p className="text-ui-sm text-muted-foreground mt-1">{t("narrowStep")}</p>
        <div className="h-1.5 bg-border rounded-full overflow-hidden mt-4">
          <motion.div
            className="h-full bg-primary rounded-full"
            animate={{ width: `${(narrowedValues.length / 5) * 100}%` }}
            transition={{ type: "spring", stiffness: 300, damping: 30 }}
          />
        </div>
        <div className="flex justify-end mt-2">
          <span className="font-mono text-ui-sm text-foreground">
            {narrowedValues.length}<span className="text-muted-foreground"> / 5</span>
          </span>
        </div>
      </motion.div>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
        {selectedValues.map((value, i) => {
          const selected = isNarrowed(value.id);
          const disabled = !selected && narrowedValues.length >= 5;
          const cat = CATEGORIES.find((c) => c.id === value.category);
          const translated = getTranslated(value);
          return (
            <motion.button
              key={value.id}
              {...fadeInUpDelay(0.03 * i)}
              onClick={() => !disabled && toggleNarrowed(value)}
              disabled={disabled}
              className={`relative text-left p-4 rounded-card border-2 transition-all duration-200 press-effect ${
                selected
                  ? "border-primary bg-primary/5 shadow-surface-md"
                  : disabled
                  ? "border-border bg-card opacity-40 cursor-not-allowed"
                  : "border-border bg-card hover:border-primary/30 hover:shadow-surface-md cursor-pointer"
              }`}
            >
              <div className="absolute top-3 right-3">
                {selected ? (
                  <div className="w-5 h-5 rounded-full bg-primary flex items-center justify-center">
                    <Check className="w-3 h-3 text-primary-foreground" />
                  </div>
                ) : disabled ? (
                  <X className="w-4 h-4 text-muted-foreground/30" />
                ) : null}
              </div>
              <h3 className="text-ui-sm font-semibold text-foreground mb-1">{translated.name}</h3>
              <p className="text-xs text-muted-foreground line-clamp-2">{translated.description}</p>
              <span
                className="inline-block mt-2 px-2 py-0.5 rounded-full text-xs font-medium"
                style={{ backgroundColor: `${cat?.color}15`, color: cat?.color }}
              >
                {cat?.label.split("&")[0].trim()}
              </span>
            </motion.button>
          );
        })}
      </div>

      <div className="flex items-center justify-between mt-8">
        <button
          onClick={() => navigate("/assessment")}
          className="flex items-center gap-2 text-ui-sm font-medium text-muted-foreground hover:text-foreground transition-colors press-effect"
        >
          <ArrowLeft className="w-4 h-4" />
          {t("back")}
        </button>
        {narrowedValues.length === 5 && (
          <motion.button
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            onClick={() => navigate("/prioritize")}
            className="flex items-center gap-2 h-11 px-6 bg-primary text-primary-foreground rounded-card text-ui-sm font-medium shadow-surface-lg hover:opacity-90 transition-opacity press-effect"
          >
            {t("rankTop3")}
            <ArrowRight className="w-4 h-4" />
          </motion.button>
        )}
      </div>
    </div>
  );
}
