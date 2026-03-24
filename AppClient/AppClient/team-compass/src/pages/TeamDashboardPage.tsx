import { motion } from "framer-motion";
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, Cell, RadarChart, Radar, PolarGrid, PolarAngleAxis } from "recharts";
import {
  TEAM_MEMBERS, TEAM_VALUES,
  getTeamCategoryScores, ValueCategory,
} from "@/data/mockData";
import { useLanguage } from "@/context/LanguageContext";
import { useTranslatedData } from "@/hooks/useTranslatedData";
import FitGauge from "@/components/FitGauge";
import { Users, AlertTriangle, Sparkles, Shield, TrendingUp, Layers, BarChart3 } from "lucide-react";
import { fadeInUp, fadeInUpDelay } from "@/lib/motion";

const INSIGHT_ICONS: Record<string, typeof Sparkles> = {
  strength: Shield,
  risk: AlertTriangle,
  coaching: Sparkles,
  intervention: Users,
  warning: AlertTriangle,
};

export default function TeamDashboardPage() {
  const { t } = useLanguage();
  const { categories: CATEGORIES, polarityPairs: POLARITY_PAIRS, aiInsights: AI_INSIGHTS, determineCultureType } = useTranslatedData();
  const avgFit = Math.round(TEAM_MEMBERS.reduce((s, m) => s + m.fitScore, 0) / TEAM_MEMBERS.length);
  const teamScores = getTeamCategoryScores();
  const teamCulture = determineCultureType(teamScores);

  const radarData = CATEGORIES.map((cat) => ({
    category: cat.label.split("&")[0].trim(),
    value: teamScores[cat.id as ValueCategory],
  }));

  const mean = avgFit;
  const variance = TEAM_MEMBERS.reduce((sum, m) => sum + Math.pow(m.fitScore - mean, 2), 0) / TEAM_MEMBERS.length;
  const polarization = Math.round(Math.sqrt(variance));
  const alignmentScore = Math.round(100 - polarization * 2);

  return (
    <div className="workspace-container">
      <motion.div {...fadeInUp}>
        <h1 className="text-subhead font-semibold text-foreground">{t("teamPulseTitle")}</h1>
        <p className="text-ui-sm text-muted-foreground mt-1 mb-8">{t("teamSubtitle")}</p>
      </motion.div>

      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        {[
          { label: t("teamCohesion"), value: `${avgFit}%`, icon: Users, color: "text-primary" },
          { label: t("cultureType"), value: teamCulture.name.split(" ")[0], icon: Layers, color: "text-primary" },
          { label: t("alignment"), value: `${alignmentScore}%`, icon: TrendingUp, color: alignmentScore >= 60 ? "text-success" : "text-warning" },
          { label: t("polarization"), value: `${polarization}`, icon: BarChart3, color: polarization > 10 ? "text-destructive" : "text-success" },
        ].map((stat, i) => (
          <motion.div key={stat.label as string} {...fadeInUpDelay(0.05 * (i + 1))} className="card-surface p-5">
            <div className="flex items-center gap-2 mb-2">
              <stat.icon className={`w-4 h-4 ${stat.color}`} />
              <span className="text-xs text-muted-foreground font-medium">{stat.label as string}</span>
            </div>
            <span className="text-body font-semibold text-foreground">{stat.value}</span>
          </motion.div>
        ))}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <motion.div {...fadeInUpDelay(0.1)} className="card-surface p-6 flex flex-col items-center">
          <h3 className="text-ui-sm font-medium text-muted-foreground mb-4">{t("teamCohesionScore")}</h3>
          <FitGauge score={avgFit} />
          <p className="text-xs text-muted-foreground mt-4 text-center">
            {t("avgAlignment")} {TEAM_MEMBERS.length} {t("members")}
          </p>
        </motion.div>

        <motion.div {...fadeInUpDelay(0.15)} className="card-surface p-6">
          <h3 className="text-ui-sm font-medium text-muted-foreground mb-3">{t("teamCultureType")}</h3>
          <div className="flex items-center gap-2 mb-3">
            <div className="w-3 h-3 rounded-full" style={{ backgroundColor: teamCulture.color }} />
            <span className="text-body font-semibold text-foreground">{teamCulture.name}</span>
          </div>
          <div className="space-y-1 mb-3">
            {teamCulture.characteristics.map((c, i) => (
              <p key={i} className="text-xs text-muted-foreground">• {c}</p>
            ))}
          </div>
          <div className="pt-3 border-t border-border">
            <p className="text-xs font-medium text-success mb-1">{t("strengths")}</p>
            {teamCulture.strengths.map((s, i) => (
              <p key={i} className="text-xs text-muted-foreground">+ {s}</p>
            ))}
            <p className="text-xs font-medium text-destructive mt-2 mb-1">{t("risks")}</p>
            {teamCulture.risks.map((r, i) => (
              <p key={i} className="text-xs text-muted-foreground">⚠ {r}</p>
            ))}
          </div>
        </motion.div>

        <motion.div {...fadeInUpDelay(0.2)} className="card-surface p-6">
          <div className="flex items-center gap-2 mb-4">
            <Users className="w-4 h-4 text-muted-foreground" />
            <h3 className="text-ui-sm font-medium text-muted-foreground">{t("teamMembers")}</h3>
          </div>
          <div className="space-y-3">
            {TEAM_MEMBERS.map((m) => (
              <div key={m.id} className="flex items-center gap-3">
                <div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center text-xs font-semibold text-primary">{m.avatar}</div>
                <div className="flex-1 min-w-0">
                  <p className="text-ui-sm font-medium text-foreground truncate">{m.name}</p>
                  <p className="text-xs text-muted-foreground">{m.role}</p>
                </div>
                <span className={`font-mono text-ui-sm font-medium ${m.fitScore >= 80 ? "text-success" : m.fitScore >= 60 ? "text-foreground" : "text-warning"}`}>
                  {m.fitScore}%
                </span>
              </div>
            ))}
          </div>
        </motion.div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <motion.div {...fadeInUpDelay(0.25)} className="card-surface p-6">
          <h3 className="text-ui-sm font-medium text-muted-foreground mb-4">{t("teamCategoryProfile")}</h3>
          <ResponsiveContainer width="100%" height={240}>
            <RadarChart data={radarData} cx="50%" cy="50%" outerRadius="70%">
              <PolarGrid stroke="hsl(220, 13%, 89%)" />
              <PolarAngleAxis dataKey="category" tick={{ fontSize: 11, fill: "hsl(220, 9%, 46%)" }} />
              <Radar dataKey="value" stroke="hsl(217, 91%, 50%)" fill="hsl(217, 91%, 50%)" fillOpacity={0.15} strokeWidth={2} />
            </RadarChart>
          </ResponsiveContainer>
        </motion.div>

        <motion.div {...fadeInUpDelay(0.3)} className="card-surface p-6">
          <h3 className="text-ui-sm font-medium text-muted-foreground mb-4">{t("mostCommonValues")}</h3>
          <ResponsiveContainer width="100%" height={240}>
            <BarChart data={TEAM_VALUES} margin={{ left: 80 }} layout="vertical">
              <XAxis type="number" domain={[0, 100]} tick={{ fontSize: 12, fill: "hsl(220, 9%, 46%)" }} tickFormatter={(v) => `${v}%`} />
              <YAxis type="category" dataKey="name" tick={{ fontSize: 13, fill: "hsl(222, 47%, 11%)" }} axisLine={false} tickLine={false} />
              <Tooltip contentStyle={{ background: "hsl(0, 0%, 100%)", border: "1px solid hsl(220, 13%, 89%)", borderRadius: 8, fontSize: 13, color: "hsl(222, 47%, 11%)" }} formatter={(value: number) => [`${value}%`, t("teamAgreement")]} />
              <Bar dataKey="percentage" radius={[0, 6, 6, 0]} barSize={18}>
                {TEAM_VALUES.map((_, i) => (
                  <Cell key={i} fill={i < 2 ? "hsl(217, 91%, 50%)" : "hsl(199, 89%, 48%)"} />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </motion.div>
      </div>

      <motion.div {...fadeInUpDelay(0.35)} className="card-surface p-6 mb-8">
        <div className="flex items-center gap-2 mb-4">
          <AlertTriangle className="w-4 h-4 text-warning" />
          <h3 className="text-ui-sm font-medium text-muted-foreground">{t("valueTensions")}</h3>
        </div>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
          {POLARITY_PAIRS.map((pair) => {
            const scoreA = teamScores[pair.categoryA];
            const scoreB = teamScores[pair.categoryB];
            const diff = Math.abs(scoreA - scoreB);
            const isTense = diff > 15;
            return (
              <div
                key={`${pair.categoryA}-${pair.categoryB}`}
                className={`p-4 rounded-inner border ${isTense ? "border-warning/30 bg-warning/5" : "border-border bg-secondary/30"}`}
              >
                <div className="flex items-center justify-between mb-2">
                  <span className="text-ui-sm font-medium text-foreground">{pair.labelA}</span>
                  <span className="text-xs text-muted-foreground">↔</span>
                  <span className="text-ui-sm font-medium text-foreground">{pair.labelB}</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="flex-1 h-2 rounded-full bg-border overflow-hidden">
                    <div className="h-full rounded-full bg-primary" style={{ width: `${scoreA}%` }} />
                  </div>
                  <span className="font-mono text-xs text-muted-foreground w-8">{scoreA}%</span>
                </div>
                <div className="flex items-center gap-2 mt-1">
                  <div className="flex-1 h-2 rounded-full bg-border overflow-hidden">
                    <div className="h-full rounded-full bg-success" style={{ width: `${scoreB}%` }} />
                  </div>
                  <span className="font-mono text-xs text-muted-foreground w-8">{scoreB}%</span>
                </div>
                {isTense && (
                  <p className="text-xs text-warning mt-2 font-medium">⚠ {t("notableTension")} ({diff}% {t("gap")})</p>
                )}
              </div>
            );
          })}
        </div>
      </motion.div>

      <motion.div {...fadeInUpDelay(0.4)}>
        <div className="flex items-center gap-2 mb-4">
          <Sparkles className="w-4 h-4 text-primary" />
          <h3 className="text-body font-semibold text-foreground">{t("aiCoachingInsights")}</h3>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {AI_INSIGHTS.map((insight) => {
            const Icon = INSIGHT_ICONS[insight.type] || Sparkles;
            const borderColor = insight.type === "warning" || insight.type === "risk"
              ? "border-warning/20 hover:border-warning/40"
              : "hover:border-primary/30";
            return (
              <div key={insight.id} className={`card-surface p-5 border transition-colors ${borderColor}`}>
                <div className="w-8 h-8 rounded-inner bg-primary/10 flex items-center justify-center mb-3">
                  <Icon className={`w-4 h-4 ${insight.type === "warning" || insight.type === "risk" ? "text-warning" : "text-primary"}`} />
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
