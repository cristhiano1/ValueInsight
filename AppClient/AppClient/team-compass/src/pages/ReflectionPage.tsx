import { motion } from "framer-motion";
import { ArrowRight, ArrowLeft, MessageSquare, Home } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useApp } from "@/context/AppContext";
import { useLanguage } from "@/context/LanguageContext";
import { useTranslatedData } from "@/hooks/useTranslatedData";
import { fadeInUp, fadeInUpDelay } from "@/lib/motion";

export default function ReflectionPage() {
  const { reflections, setReflections } = useApp();
  const navigate = useNavigate();
  const { t } = useLanguage();
  const { reflectionQuestions } = useTranslatedData();

  const updateReflection = (index: number, value: string) => {
    const updated = [...reflections];
    updated[index] = value;
    setReflections(updated);
  };

  const filledCount = reflections.filter((r) => r.trim().length > 0).length;

  return (
    <div className="min-h-screen flex items-center justify-center bg-background px-4 py-12 relative">
      <button
        onClick={() => navigate("/dashboard")}
        className="absolute top-4 left-4 w-8 h-8 rounded-full bg-primary/10 hover:bg-primary/20 flex items-center justify-center transition-colors"
      >
        <Home className="w-4 h-4 text-primary" />
      </button>
      <div className="w-full max-w-[640px]">
        <motion.div {...fadeInUp} className="mb-8">
          <div className="flex items-center gap-3 mb-4">
            <div className="w-10 h-10 rounded-card bg-primary/10 flex items-center justify-center">
              <MessageSquare className="w-5 h-5 text-primary" />
            </div>
            <div>
              <h1 className="text-subhead font-semibold text-foreground">{t("reflection")}</h1>
              <p className="text-ui-sm text-muted-foreground">{t("reflectionStep")}</p>
            </div>
          </div>
          <p className="text-body text-muted-foreground leading-relaxed">{t("reflectionIntro")}</p>
        </motion.div>

        <div className="space-y-5">
          {reflectionQuestions.map((question, i) => (
            <motion.div key={i} {...fadeInUpDelay(0.05 * (i + 1))} className="card-surface p-5">
              <label className="block text-ui-sm font-semibold text-foreground mb-3">
                {i + 1}. {question}
              </label>
              <textarea
                value={reflections[i] || ""}
                onChange={(e) => updateReflection(i, e.target.value)}
                placeholder={t("writeThoughts") as string}
                rows={3}
                className="w-full p-3 rounded-inner border border-input bg-background text-ui-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring/20 focus:border-primary transition-colors resize-none"
              />
            </motion.div>
          ))}
        </div>

        <div className="flex items-center justify-between mt-8">
          <button
            onClick={() => navigate("/onboarding")}
            className="flex items-center gap-2 text-ui-sm font-medium text-muted-foreground hover:text-foreground transition-colors press-effect"
          >
            <ArrowLeft className="w-4 h-4" />
            {t("back")}
          </button>
          <span className="text-xs text-muted-foreground">{filledCount} / {reflectionQuestions.length} {t("answered")}</span>
          <button
            onClick={() => navigate("/assessment")}
            className="flex items-center gap-2 h-10 px-5 bg-primary text-primary-foreground rounded-inner text-ui-sm font-medium hover:opacity-90 transition-opacity press-effect"
          >
            {t("selectValues")}
            <ArrowRight className="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>
  );
}
