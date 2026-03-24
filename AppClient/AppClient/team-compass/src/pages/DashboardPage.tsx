import { motion } from "framer-motion";
import { ArrowRight, Compass, Users, BarChart3, TrendingUp, Sparkles, Shield, AlertTriangle } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useApp } from "@/context/AppContext";
import { useLanguage } from "@/context/LanguageContext";
import { useTranslatedData } from "@/hooks/useTranslatedData";
import FitGauge from "@/components/FitGauge";
import { fadeInUp, fadeInUpDelay } from "@/lib/motion";
import { calculateCFS, getTeamCategoryScores, ValueCategory } from "@/data/mockData";

const INSIGHT_ICONS: Record<string, typeof Sparkles> = {
  strength: Shield,
  risk: AlertTriangle,
  coaching: Sparkles,
  intervention: Users,
  warning: AlertTriangle,
};

export default function DashboardPage() {
  const navigate = useNavigate();
  const { rankedValues, selectedValues } = useApp();
  const { t } = useLanguage();
  const { categories: CATEGORIES, cfsInterpretation, aiInsights: AI_INSIGHTS, determineCultureType } = useTranslatedData();

  const userCategoryScores: Record<ValueCategory, number> = {
    relation: 0, result: 0, structure: 0, autonomy: 0, development: 0, meaning: 0,
  };
  selectedValues.forEach((v) => { userCategoryScores[v.category] += 10; });
  const totalUser = Object.values(userCategoryScores).reduce((a, b) => a + b, 0) || 1;
  (Object.keys(userCategoryScores) as ValueCategory[]).forEach((k) => {
    userCategoryScores[k] = Math.round((userCategoryScores[k] / totalUser) * 100);
  });

  const teamScores = getTeamCategoryScores();
  const cfs = calculateCFS(userCategoryScores, teamScores);
  const userCulture = determineCultureType(userCategoryScores);
  const interpretation = cfsInterpretation.find((i) => cfs.score >= i.min && cfs.score <= i.max);

  const hasAssessment = selectedValues.length > 0;
  const topValue = rankedValues[0]?.name || "—";

  return (
    <div className="workspace-container">
      <motion.div {...fadeInUp}>
        <h1 className="text-subhead font-semibold text-foreground">{t("welcomeBack")}</h1>
        <p className="text-ui-sm text-muted-foreground mt-1 mb-8">{t("dashboardSubtitle")}</p>
      </motion.div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        {[
          { label: t("assessmentLabel"), value: hasAssessment ? t("complete") : t("pending"), icon: Compass, color: hasAssessment ? "text-success" : "text-warning", path: "/assessment" },
          { label: t("topValue"), value: topValue, icon: TrendingUp, color: "text-primary", path: "/profile" },
          { label: t("cultureType"), value: hasAssessment ? userCulture.name.split(" ")[0] : "—", icon: BarChart3, color: "text-primary", path: "/profile" },
          { label: t("fitScore"), value: hasAssessment ? `${cfs.score}%` : "—", icon: Users, color: cfs.score >= 60 ? "text-success" : "text-warning", path: "/team" },
        ].map((stat, i) => (
          <motion.div
            key={stat.label as string}
            {...fadeInUpDelay(0.05 * (i + 1))}
            onClick={() => navigate(stat.path)}
            className="card-surface p-5 cursor-pointer hover:border-primary/30 hover:shadow-md transition-all active:scale-[0.98]"
          >
            <div className="flex items-center gap-2 mb-2">
              <stat.icon className={`w-4 h-4 ${stat.color}`} />
              <span className="text-xs text-muted-foreground font-medium">{stat.label as string}</span>
            </div>
            <span className="text-body font-semibold text-foreground">{stat.value as string}</span>
          </motion.div>
        ))}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <motion.div
          {...fadeInUpDelay(0.25)}
          onClick={() => navigate("/team")}
          className="card-surface p-6 flex flex-col items-center cursor-pointer hover:border-primary/30 hover:shadow-md transition-all active:scale-[0.98]"
        >
          <h3 className="text-ui-sm font-medium text-muted-foreground mb-4">{t("yourCulturalFit")}</h3>
          <FitGauge score={hasAssessment ? cfs.score : 0} />
          <p className="text-ui-sm font-medium text-foreground mt-4">{interpretation?.label || t("completeAssessment")}</p>
          <p className="text-xs text-muted-foreground mt-1 text-center max-w-[260px]">
            {interpretation?.description || t("takeAssessment")}
          </p>
        </motion.div>

        <motion.div {...fadeInUpDelay(0.3)} className="card-surface p-6">
          <h3 className="text-ui-sm font-medium text-muted-foreground mb-4">{t("quickActions")}</h3>
          <div className="space-y-3">
            {[
              { label: hasAssessment ? t("retakeAssessment") : t("startAssessment"), desc: t("discoverSignature"), path: "/onboarding" },
              { label: t("viewTeamInsights"), desc: t("exploreTeam"), path: "/team" },
              { label: t("myValueProfile"), desc: t("reviewValues"), path: "/profile" },
            ].map((action) => (
              <button
                key={action.path}
                onClick={() => navigate(action.path)}
                className="w-full flex items-center justify-between p-3 rounded-inner border border-border hover:border-primary/30 hover:bg-secondary/30 transition-all press-effect text-left"
              >
                <div>
                  <p className="text-ui-sm font-medium text-foreground">{action.label as string}</p>
                  <p className="text-xs text-muted-foreground">{action.desc as string}</p>
                </div>
                <ArrowRight className="w-4 h-4 text-muted-foreground" />
              </button>
            ))}
          </div>
        </motion.div>
      </div>

      {hasAssessment && (
        <motion.div {...fadeInUpDelay(0.35)} className="card-surface p-6 mb-8">
          <h3 className="text-ui-sm font-medium text-muted-foreground mb-4">{t("yourCategoryDistribution")}</h3>
          <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
            {CATEGORIES.map((cat) => {
              const score = userCategoryScores[cat.id as ValueCategory];
              return (
                <div
                  key={cat.id}
                  onClick={() => navigate("/profile")}
                  className="p-3 rounded-inner bg-secondary/50 cursor-pointer hover:bg-secondary/80 transition-all active:scale-[0.97]"
                >
                  <div className="flex items-center gap-2 mb-2">
                    <div className="w-2.5 h-2.5 rounded-full" style={{ backgroundColor: cat.color }} />
                    <span className="text-xs font-medium text-foreground">{cat.label.split("&")[0].trim()}</span>
                  </div>
                  <div className="h-1.5 bg-border rounded-full overflow-hidden mb-1">
                    <div className="h-full rounded-full" style={{ width: `${score}%`, backgroundColor: cat.color }} />
                  </div>
                  <span className="font-mono text-xs text-muted-foreground">{score}%</span>
                </div>
              );
            })}
          </div>
        </motion.div>
      )}

      <motion.div {...fadeInUpDelay(0.4)}>
        <div className="flex items-center gap-2 mb-4">
          <Sparkles className="w-4 h-4 text-primary" />
          <h3 className="text-body font-semibold text-foreground">{t("aiCoachingInsights")}</h3>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {AI_INSIGHTS.slice(0, 3).map((insight) => {
            const Icon = INSIGHT_ICONS[insight.type] || Sparkles;
            return (
              <div
                key={insight.id}
                onClick={() => navigate("/team")}
                className="card-surface-hover p-5 cursor-pointer active:scale-[0.97] transition-transform"
              >
                <div className="w-8 h-8 rounded-inner bg-primary/10 flex items-center justify-center mb-3">
                  <Icon className="w-4 h-4 text-primary" />
                </div>
                <h4 className="text-ui-sm font-semibold text-foreground mb-2">{insight.title}</h4>
                <p className="text-xs text-muted-foreground leading-relaxed">{insight.description}</p>
              </div>
            );
          })}
        </div>
      </motion.div>
    </div>
  );
}
