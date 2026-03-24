import { useState, useMemo } from "react";
import { motion } from "framer-motion";
import { Search, Check, ArrowRight, ArrowLeft } from "lucide-react";
import { Value } from "@/data/mockData";
import { useApp } from "@/context/AppContext";
import { useLanguage } from "@/context/LanguageContext";
import { useTranslatedData } from "@/hooks/useTranslatedData";
import { useNavigate } from "react-router-dom";

export default function ValuesSelectionPage() {
  const { selectedValues, toggleValue } = useApp();
  const navigate = useNavigate();
  const { t } = useLanguage();
  const { values: VALUES, categories: CATEGORIES } = useTranslatedData();
  const [search, setSearch] = useState("");
  const [activeCategory, setActiveCategory] = useState<string | null>(null);

  const filtered = useMemo(() => {
    return VALUES.filter((v) => {
      const matchSearch = v.name.toLowerCase().includes(search.toLowerCase()) ||
        v.description.toLowerCase().includes(search.toLowerCase());
      const matchCategory = !activeCategory || v.category === activeCategory;
      return matchSearch && matchCategory;
    });
  }, [search, activeCategory, VALUES]);

  const isSelected = (v: Value) => selectedValues.some((s) => s.id === v.id);
  const progress = selectedValues.length / 10;

  // Get translated name for selected values
  const getTranslatedName = (v: Value) => {
    const translated = VALUES.find((tv) => tv.id === v.id);
    return translated?.name ?? v.name;
  };

  return (
    <div className="workspace-narrow">
      <div className="sticky top-14 z-20 bg-background/95 backdrop-blur-sm pb-6 -mx-6 md:-mx-12 px-6 md:px-12 pt-2">
        <div className="flex items-center justify-between mb-2">
          <div>
            <h1 className="text-subhead font-semibold text-foreground">{t("selectYourValues")}</h1>
            <p className="text-ui-sm text-muted-foreground mt-1">{t("selectValuesStep")}</p>
          </div>
          <div className="flex items-center gap-3">
            <span className="font-mono text-body font-semibold text-foreground">
              {selectedValues.length}
              <span className="text-muted-foreground font-normal"> / 10</span>
            </span>
          </div>
        </div>

        <div className="h-1.5 bg-border rounded-full overflow-hidden mb-4">
          <motion.div
            className="h-full bg-primary rounded-full"
            animate={{ width: `${progress * 100}%` }}
            transition={{ type: "spring", stiffness: 300, damping: 30 }}
          />
        </div>

        <div className="flex flex-col sm:flex-row gap-3">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder={t("searchValues") as string}
              className="w-full h-9 pl-10 pr-4 rounded-inner border border-input bg-background text-ui-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring/20 focus:border-primary transition-colors"
            />
          </div>
          <div className="flex gap-1.5 flex-wrap">
            <button
              onClick={() => setActiveCategory(null)}
              className={`px-3 py-1.5 rounded-full text-xs font-medium transition-colors press-effect ${
                !activeCategory ? "bg-primary text-primary-foreground" : "bg-secondary text-muted-foreground hover:text-foreground"
              }`}
            >
              {t("all")}
            </button>
            {CATEGORIES.map((cat) => (
              <button
                key={cat.id}
                onClick={() => setActiveCategory(activeCategory === cat.id ? null : cat.id)}
                className={`px-3 py-1.5 rounded-full text-xs font-medium transition-colors press-effect ${
                  activeCategory === cat.id ? "bg-primary text-primary-foreground" : "bg-secondary text-muted-foreground hover:text-foreground"
                }`}
              >
                {cat.label.split("&")[0].trim()}
              </button>
            ))}
          </div>
        </div>
      </div>

      {filtered.length === 0 ? (
        <p className="text-center text-muted-foreground py-12">{t("noValuesMatch")}</p>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          {filtered.map((value) => {
            const selected = isSelected(value);
            const disabled = !selected && selectedValues.length >= 10;
            const cat = CATEGORIES.find((c) => c.id === value.category);
            return (
              <motion.button
                key={value.id}
                initial={{ opacity: 0, y: 12 }}
                animate={{ opacity: 1, y: 0 }}
                onClick={() => !disabled && toggleValue(value)}
                disabled={disabled}
                className={`relative text-left p-4 rounded-card border-2 transition-all duration-200 press-effect ${
                  selected
                    ? "border-primary bg-primary/5 shadow-surface-md"
                    : disabled
                    ? "border-border bg-card opacity-40 cursor-not-allowed"
                    : "border-border bg-card hover:border-primary/30 hover:shadow-surface-md cursor-pointer"
                }`}
              >
                {selected && (
                  <div className="absolute top-3 right-3 w-5 h-5 rounded-full bg-primary flex items-center justify-center">
                    <Check className="w-3 h-3 text-primary-foreground" />
                  </div>
                )}
                <h3 className="text-ui-sm font-semibold text-foreground mb-1">{value.name}</h3>
                <p className="text-xs text-muted-foreground leading-relaxed line-clamp-2">{value.description}</p>
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
      )}

      <div className="sticky bottom-6 flex items-center justify-between mt-8">
        <button
          onClick={() => navigate("/reflection")}
          className="flex items-center gap-2 text-ui-sm font-medium text-muted-foreground hover:text-foreground transition-colors press-effect"
        >
          <ArrowLeft className="w-4 h-4" />
          {t("back")}
        </button>
        {selectedValues.length === 10 && (
          <motion.button
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            onClick={() => navigate("/narrow")}
            className="flex items-center gap-2 h-11 px-6 bg-primary text-primary-foreground rounded-card text-ui-sm font-medium shadow-surface-lg hover:opacity-90 transition-opacity press-effect"
          >
            {t("narrowTo5")}
            <ArrowRight className="w-4 h-4" />
          </motion.button>
        )}
      </div>
    </div>
  );
}
