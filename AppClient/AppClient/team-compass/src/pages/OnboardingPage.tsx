import { motion, AnimatePresence } from "framer-motion";
import { ArrowRight, ArrowLeft, Heart, Users, BarChart3, Target, Layers, MessageSquare, Home } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useApp } from "@/context/AppContext";
import { useLanguage } from "@/context/LanguageContext";
import { EASE } from "@/lib/motion";

const ICONS = [Heart, Users, Layers, Target, BarChart3, MessageSquare];

export default function OnboardingPage() {
  const { onboardingStep, setOnboardingStep } = useApp();
  const navigate = useNavigate();
  const { t } = useLanguage();

  const steps = t("onboardingSteps") as Array<{ title: string; description: string; detail: string }>;
  const currentStep = Math.min(onboardingStep, steps.length - 1);
  const step = steps[currentStep];
  const Icon = ICONS[currentStep] || Heart;

  return (
    <div className="min-h-screen flex flex-col">
      {/* Gradient header */}
      <div className="gradient-hero px-4 py-10 text-center relative">
        <button
          onClick={() => navigate("/dashboard")}
          className="absolute top-4 left-4 w-8 h-8 rounded-full bg-white/20 hover:bg-white/30 flex items-center justify-center transition-colors"
        >
          <Home className="w-4 h-4 text-primary-foreground" />
        </button>
        <p className="text-xs font-medium tracking-widest uppercase text-primary-foreground/70 mb-2">
          {currentStep + 1} / {steps.length}
        </p>
        <div className="flex items-center gap-1.5 max-w-[560px] mx-auto">
          {steps.map((_: any, i: number) => (
            <div
              key={i}
              className={`h-1 flex-1 rounded-full transition-colors duration-300 ${
                i <= currentStep ? "bg-white" : "bg-white/25"
              }`}
            />
          ))}
        </div>
      </div>

      <div className="flex-1 flex items-start justify-center px-4 -mt-6">
        <div className="w-full max-w-[560px]">
          <AnimatePresence mode="wait">
            <motion.div
              key={currentStep}
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: -20 }}
              transition={{ duration: 0.4, ease: EASE }}
              className="card-surface p-10"
            >
              <div className="w-12 h-12 rounded-card gradient-subtle flex items-center justify-center mb-6">
                <Icon className="w-6 h-6 text-primary" />
              </div>
              <h1 className="text-subhead font-semibold text-foreground mb-3">{step.title}</h1>
              <p className="text-body text-muted-foreground leading-relaxed mb-4">{step.description}</p>
              <p className="text-ui-sm text-muted-foreground/70 italic">{step.detail}</p>
            </motion.div>
          </AnimatePresence>

          <div className="flex items-center justify-between mt-8">
            <button
              onClick={() => setOnboardingStep(Math.max(0, currentStep - 1))}
              disabled={currentStep === 0}
              className="flex items-center gap-2 text-ui-sm font-medium text-muted-foreground hover:text-foreground disabled:opacity-30 transition-colors press-effect"
            >
              <ArrowLeft className="w-4 h-4" />
              {t("back")}
            </button>
            <button
              onClick={() => {
                if (currentStep < steps.length - 1) {
                  setOnboardingStep(currentStep + 1);
                } else {
                  navigate("/reflection");
                }
              }}
              className="flex items-center gap-2 h-10 px-5 gradient-hero text-primary-foreground rounded-inner text-ui-sm font-medium hover:opacity-90 transition-opacity press-effect"
            >
              {currentStep < steps.length - 1 ? t("continue") : t("beginReflection")}
              <ArrowRight className="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
