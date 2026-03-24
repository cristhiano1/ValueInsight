import { motion } from "framer-motion";
import { Radar, RadarChart, PolarGrid, PolarAngleAxis, ResponsiveContainer, BarChart, Bar, XAxis, YAxis, Tooltip, Cell } from "recharts";
import { useApp } from "@/context/AppContext";
import { useLanguage } from "@/context/LanguageContext";
import { useTranslatedData } from "@/hooks/useTranslatedData";
import { calculateCFS, getTeamCategoryScores, ValueCategory } from "@/data/mockData";
import FitGauge from "@/components/FitGauge";
import { fadeInUp, fadeInUpDelay } from "@/lib/motion";
import { Star, Sparkles, ArrowRight } from "lucide-react";
import { useNavigate } from "react-router-dom";

export default function ProfilePage() {
  const { rankedValues, selectedValues, narrowedValues, concretizations } = useApp();
  const navigate = useNavigate();
  const { t } = useLanguage();
  const { categories: CATEGORIES, values: ALL_VALUES, cfsInterpretation, determineCultureType } = useTranslatedData();
  const displayTop3 = rankedValues.length > 0 ? rankedValues : [];
  const displayAll = narrowedValues.length > 0 ? narrowedValues : selectedValues.slice(0, 5);

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
  const cultureType = determineCultureType(userCategoryScores);
  const interpretation = cfsInterpretation.find((i) => cfs.score >= i.min && cfs.score <= i.max);

  const radarData = CATEGORIES.map((cat) => ({
    category: cat.label.split("&")[0].trim(),
    user: userCategoryScores[cat.id as ValueCategory],
    team: teamScores[cat.id as ValueCategory],
  }));

  const getTranslated = (v: typeof displayAll[0]) => ALL_VALUES.find((tv) => tv.id === v.id) ?? v;

  const barData = displayAll.map((v, i) => {
    const cat = CATEGORIES.find((c) => c.id === v.category);
    const tv = getTranslated(v);
    return { name: tv.name, weight: 100 - i * 15, color: cat?.color || "hsl(187, 72%, 48%)" };
  });

  return (
    <div className="workspace-container">
      <motion.div {...fadeInUp}>
        <h1 className="text-subhead font-semibold text-foreground">{t("yourValueSignature")}</h1>
        <p className="text-ui-sm text-muted-foreground mt-1 mb-8">{t("profileStep")}</p>
      </motion.div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <motion.div {...fadeInUpDelay(0.05)} className="card-surface p-6 flex flex-col items-center">
          <h3 className="text-ui-sm font-medium text-muted-foreground mb-4">{t("culturalFitScore")}</h3>
          <FitGauge score={cfs.score} />
          <p className="text-xs text-muted-foreground mt-4 text-center max-w-[200px]">
            {interpretation?.description}
          </p>
        </motion.div>

        <motion.div {...fadeInUpDelay(0.1)} className="card-surface p-6">
          <h3 className="text-ui-sm font-medium text-muted-foreground mb-3">{t("yourCultureType")}</h3>
          <div className="flex items-center gap-2 mb-3">
            <div className="w-3 h-3 rounded-full" style={{ backgroundColor: cultureType.color }} />
            <span className="text-body font-semibold text-foreground">{cultureType.name}</span>
          </div>
          <div className="space-y-2 mb-3">
            {cultureType.characteristics.map((c, i) => (
              <p key={i} className="text-xs text-muted-foreground">• {c}</p>
            ))}
          </div>
          <div className="pt-3 border-t border-border">
            <p className="text-xs font-medium text-success mb-1">{t("strengths")}</p>
            {cultureType.strengths.slice(0, 2).map((s, i) => (
              <p key={i} className="text-xs text-muted-foreground">+ {s}</p>
            ))}
            <p className="text-xs font-medium text-destructive mt-2 mb-1">{t("risks")}</p>
            {cultureType.risks.slice(0, 2).map((r, i) => (
              <p key={i} className="text-xs text-muted-foreground">⚠ {r}</p>
            ))}
          </div>
        </motion.div>

        <motion.div {...fadeInUpDelay(0.15)} className="card-surface p-6">
          <h3 className="text-ui-sm font-medium text-muted-foreground mb-4">{t("cfsBreakdown")}</h3>
          <div className="space-y-3">
            {[
              { label: t("categoryAlignment"), value: cfs.categoryAlignment, weight: "40%" },
              { label: t("topOverlap"), value: cfs.topOverlap, weight: "30%" },
              { label: t("dominanceMatch"), value: cfs.dominanceMatch, weight: "15%" },
              { label: t("tensionScore"), value: cfs.tensionScore, weight: "15%" },
            ].map((comp) => (
              <div key={comp.label as string}>
                <div className="flex justify-between text-xs mb-1">
                  <span className="text-muted-foreground">{comp.label as string} <span className="text-muted-foreground/50">({comp.weight})</span></span>
                  <span className="font-mono text-foreground">{comp.value}%</span>
                </div>
                <div className="h-1.5 bg-border rounded-full overflow-hidden">
                  <div className="h-full bg-primary rounded-full transition-all" style={{ width: `${comp.value}%` }} />
                </div>
              </div>
            ))}
          </div>
        </motion.div>
      </div>

      <motion.div {...fadeInUpDelay(0.2)} className="card-surface p-6 mb-8">
        <h3 className="text-ui-sm font-medium text-muted-foreground mb-4">{t("youVsTeam")}</h3>
        <ResponsiveContainer width="100%" height={280}>
          <RadarChart data={radarData} cx="50%" cy="50%" outerRadius="70%">
            <PolarGrid stroke="hsl(220, 13%, 89%)" />
            <PolarAngleAxis dataKey="category" tick={{ fontSize: 12, fill: "hsl(220, 9%, 46%)" }} />
            <Radar name={t("you") as string} dataKey="user" stroke="hsl(217, 91%, 50%)" fill="hsl(217, 91%, 50%)" fillOpacity={0.15} strokeWidth={2} />
            <Radar name={t("team") as string} dataKey="team" stroke="hsl(199, 89%, 48%)" fill="hsl(199, 89%, 48%)" fillOpacity={0.1} strokeWidth={2} strokeDasharray="4 4" />
            <Tooltip contentStyle={{ background: "hsl(0, 0%, 100%)", border: "1px solid hsl(220, 13%, 89%)", borderRadius: 8, fontSize: 13, color: "hsl(222, 47%, 11%)" }} />
          </RadarChart>
        </ResponsiveContainer>
        <div className="flex items-center justify-center gap-6 mt-2">
          <div className="flex items-center gap-2 text-xs text-muted-foreground">
            <div className="w-3 h-0.5 bg-primary rounded" /> {t("you")}
          </div>
          <div className="flex items-center gap-2 text-xs text-muted-foreground">
            <div className="w-3 h-0.5 bg-success rounded" /> {t("team")}
          </div>
        </div>
      </motion.div>

      <motion.div {...fadeInUpDelay(0.25)} className="card-surface p-6 mb-8">
        <h3 className="text-ui-sm font-medium text-muted-foreground mb-4">{t("topValuesByPriority")}</h3>
        <ResponsiveContainer width="100%" height={200}>
          <BarChart data={barData} layout="vertical" margin={{ left: 100 }}>
            <XAxis type="number" hide />
            <YAxis type="category" dataKey="name" tick={{ fontSize: 13, fill: "hsl(222, 47%, 11%)" }} axisLine={false} tickLine={false} />
            <Tooltip contentStyle={{ background: "hsl(0, 0%, 100%)", border: "1px solid hsl(220, 13%, 89%)", borderRadius: 8, fontSize: 13, color: "hsl(222, 47%, 11%)" }} />
            <Bar dataKey="weight" radius={[0, 6, 6, 0]} barSize={22}>
              {barData.map((entry, i) => (
                <Cell key={i} fill={entry.color} />
              ))}
            </Bar>
          </BarChart>
        </ResponsiveContainer>
      </motion.div>

      {displayTop3.length > 0 && (
        <motion.div {...fadeInUpDelay(0.3)} className="card-surface p-6 mb-8">
          <div className="flex items-center gap-2 mb-4">
            <Sparkles className="w-4 h-4 text-primary" />
            <h3 className="text-body font-semibold text-foreground">{t("coreValuesInPractice")}</h3>
          </div>
          <div className="space-y-4">
            {displayTop3.map((v, i) => {
              const tv = getTranslated(v);
              const cat = CATEGORIES.find((c) => c.id === v.category);
              const conc = concretizations.find((c) => c.valueId === v.id);
              return (
                <div key={v.id} className="flex gap-3">
                  <div className="flex-shrink-0 w-1 rounded-full" style={{ background: cat?.color }} />
                  <div>
                    <div className="flex items-center gap-2 mb-1">
                      <Star className="w-3.5 h-3.5 text-warning fill-warning" />
                      <span className="text-ui-sm font-semibold text-foreground">#{i + 1} {tv.name}</span>
                    </div>
                    <p className="text-xs text-muted-foreground italic">"{tv.description}"</p>
                    {conc?.meaning && <p className="text-xs text-foreground">💡 {conc.meaning}</p>}
                    {conc?.behavior && <p className="text-xs text-muted-foreground">🔍 {conc.behavior}</p>}
                  </div>
                </div>
              );
            })}
          </div>
        </motion.div>
      )}

      <motion.div {...fadeInUpDelay(0.35)} className="flex justify-center">
        <button
          onClick={() => navigate("/dashboard")}
          className="flex items-center gap-2 h-11 px-6 bg-primary text-primary-foreground rounded-card text-ui-sm font-medium shadow-surface-lg hover:opacity-90 transition-opacity press-effect"
        >
          {t("goToDashboard")}
          <ArrowRight className="w-4 h-4" />
        </button>
      </motion.div>
    </div>
  );
}
