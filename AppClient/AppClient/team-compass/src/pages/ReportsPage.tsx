import { FileBarChart, Download, TrendingUp, Users, Calendar, BarChart3, PieChart } from "lucide-react";
import { TEAM_MEMBERS, CATEGORIES, TEAM_VALUES, getTeamCategoryScores } from "@/data/mockData";
import { useTranslatedData } from "@/hooks/useTranslatedData";
import FitGauge from "@/components/FitGauge";
import { toast } from "sonner";
import { useLanguage } from "@/context/LanguageContext";

export default function ReportsPage() {
  const { t } = useLanguage();
  const { categories: TRANSLATED_CATEGORIES, determineCultureType } = useTranslatedData();
  const teamScores = getTeamCategoryScores();
  const cultureType = determineCultureType(teamScores);
  const avgFit = Math.round(TEAM_MEMBERS.reduce((s, m) => s + m.fitScore, 0) / TEAM_MEMBERS.length);

  const ASSESSMENT_HISTORY = [
    { id: 1, date: "2024-12-15", type: t("individualAssessment"), status: t("completed"), score: 85 },
    { id: 2, date: "2024-11-20", type: t("teamAssessmentType"), status: t("completed"), score: 78 },
    { id: 3, date: "2024-10-05", type: t("individualAssessment"), status: t("completed"), score: 72 },
    { id: 4, date: "2025-01-10", type: t("cultureAlignmentReview"), status: t("completed"), score: 88 },
  ];

  const handleExport = (type: string) => {
    toast.success(`${type} ${t("reportDownloaded")}`);
  };

  return (
    <div className="p-6 md:p-10 max-w-5xl">
      <div className="flex items-center justify-between mb-8">
        <div className="flex items-center gap-3">
          <FileBarChart className="w-6 h-6 text-primary" />
          <h1 className="text-2xl font-semibold text-foreground">{t("reportsTitle")}</h1>
        </div>
        <button
          onClick={() => handleExport("Full")}
          className="flex items-center gap-2 px-4 py-2.5 rounded-md bg-primary text-primary-foreground text-sm font-medium hover:bg-primary/90 transition-colors press-effect"
        >
          <Download className="w-4 h-4" />
          {t("exportAll")}
        </button>
      </div>

      {/* Summary KPIs */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-8">
        {[
          { label: t("teamFitScore"), value: `${avgFit}%`, icon: TrendingUp },
          { label: t("teamMembers"), value: TEAM_MEMBERS.length.toString(), icon: Users },
          { label: t("assessments"), value: ASSESSMENT_HISTORY.length.toString(), icon: BarChart3 },
          { label: t("cultureType"), value: cultureType.name.replace(" Culture", "").replace("kultur", ""), icon: PieChart },
        ].map((kpi) => (
          <div key={String(kpi.label)} className="rounded-lg border border-border bg-card p-4 hover:shadow-md transition-shadow press-effect cursor-default">
            <div className="flex items-center gap-2 mb-2">
              <kpi.icon className="w-4 h-4 text-primary" />
              <span className="text-xs text-muted-foreground font-medium">{kpi.label}</span>
            </div>
            <p className="text-xl font-bold text-foreground">{kpi.value}</p>
          </div>
        ))}
      </div>

      {/* Individual Report Card */}
      <div className="rounded-lg border border-border bg-card p-6 mb-6">
        <div className="flex items-center justify-between mb-5">
          <div>
            <h2 className="text-base font-semibold text-foreground">{t("individualValuesReport")}</h2>
            <p className="text-xs text-muted-foreground mt-0.5">{t("personalValuesInsights")}</p>
          </div>
          <button
            onClick={() => handleExport("Individual")}
            className="flex items-center gap-1.5 px-3 py-2 rounded-md border border-border text-sm font-medium text-foreground hover:bg-secondary transition-colors press-effect"
          >
            <Download className="w-3.5 h-3.5" />
            PDF
          </button>
        </div>

        <div className="grid md:grid-cols-2 gap-6">
          <div>
            <h3 className="text-sm font-medium text-foreground mb-3">{t("yourTopValues")}</h3>
            <div className="space-y-2">
              {["Trust", "Collaboration", "Innovation", "Empathy", "Growth"].map((v, i) => (
                <div key={v} className="flex items-center gap-3">
                  <span className="w-6 h-6 rounded-full bg-primary/10 text-primary text-xs font-bold flex items-center justify-center">
                    {i + 1}
                  </span>
                  <span className="text-sm text-foreground">{v}</span>
                </div>
              ))}
            </div>
          </div>

          <div>
            <h3 className="text-sm font-medium text-foreground mb-3">{t("categoryBreakdown")}</h3>
            <div className="space-y-2.5">
              {TRANSLATED_CATEGORIES.map((cat) => {
                const score = Math.floor(Math.random() * 40) + 10;
                return (
                  <div key={cat.id}>
                    <div className="flex justify-between text-xs mb-1">
                      <span className="text-muted-foreground">{cat.label}</span>
                      <span className="font-medium text-foreground">{score}%</span>
                    </div>
                    <div className="h-2 bg-secondary rounded-full overflow-hidden">
                      <div className="h-full bg-primary rounded-full transition-all" style={{ width: `${score}%` }} />
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        </div>
      </div>

      {/* Team Report Card */}
      <div className="rounded-lg border border-border bg-card p-6 mb-6">
        <div className="flex items-center justify-between mb-5">
          <div>
            <h2 className="text-base font-semibold text-foreground">{t("teamAlignmentReport")}</h2>
            <p className="text-xs text-muted-foreground mt-0.5">{t("teamAlignmentDesc")}</p>
          </div>
          <button
            onClick={() => handleExport("Team")}
            className="flex items-center gap-1.5 px-3 py-2 rounded-md border border-border text-sm font-medium text-foreground hover:bg-secondary transition-colors press-effect"
          >
            <Download className="w-3.5 h-3.5" />
            PDF
          </button>
        </div>

        <div className="grid md:grid-cols-3 gap-6">
          <div className="flex flex-col items-center justify-center py-4">
            <FitGauge score={avgFit} />
            <p className="text-sm font-medium text-foreground mt-3">{t("teamFitScore")}</p>
          </div>

          <div>
            <h3 className="text-sm font-medium text-foreground mb-3">{t("sharedValues")}</h3>
            <div className="space-y-2">
              {TEAM_VALUES.slice(0, 5).map((v) => (
                <div key={v.name} className="flex items-center justify-between">
                  <span className="text-sm text-foreground">{v.name}</span>
                  <span className="text-xs text-muted-foreground font-medium">{v.percentage}%</span>
                </div>
              ))}
            </div>
          </div>

          <div>
            <h3 className="text-sm font-medium text-foreground mb-3">{t("teamMembers")}</h3>
            <div className="space-y-2">
              {TEAM_MEMBERS.map((m) => (
                <div key={m.id} className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <div className="w-6 h-6 rounded-full bg-primary/10 text-primary text-[10px] font-bold flex items-center justify-center">
                      {m.avatar}
                    </div>
                    <span className="text-sm text-foreground">{m.name.split(" ")[0]}</span>
                  </div>
                  <span className={`text-xs font-medium ${m.fitScore >= 85 ? "text-success" : m.fitScore >= 70 ? "text-warning" : "text-destructive"}`}>
                    {m.fitScore}%
                  </span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      {/* Assessment History */}
      <div className="rounded-lg border border-border bg-card p-6">
        <div className="flex items-center justify-between mb-5">
          <div>
            <h2 className="text-base font-semibold text-foreground">{t("assessmentHistory")}</h2>
            <p className="text-xs text-muted-foreground mt-0.5">{t("allAssessmentsScores")}</p>
          </div>
          <button
            onClick={() => handleExport("History")}
            className="flex items-center gap-1.5 px-3 py-2 rounded-md border border-border text-sm font-medium text-foreground hover:bg-secondary transition-colors press-effect"
          >
            <Download className="w-3.5 h-3.5" />
            CSV
          </button>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-border">
                <th className="text-left text-xs font-medium text-muted-foreground pb-3 pr-4">{t("date")}</th>
                <th className="text-left text-xs font-medium text-muted-foreground pb-3 pr-4">{t("type")}</th>
                <th className="text-left text-xs font-medium text-muted-foreground pb-3 pr-4">{t("status")}</th>
                <th className="text-right text-xs font-medium text-muted-foreground pb-3">{t("score")}</th>
              </tr>
            </thead>
            <tbody>
              {ASSESSMENT_HISTORY.map((a) => (
                <tr key={a.id} className="border-b border-border last:border-0 hover:bg-secondary/30 transition-colors cursor-pointer">
                  <td className="py-3 pr-4">
                    <div className="flex items-center gap-2">
                      <Calendar className="w-3.5 h-3.5 text-muted-foreground" />
                      <span className="text-sm text-foreground">{a.date}</span>
                    </div>
                  </td>
                  <td className="py-3 pr-4 text-sm text-foreground">{a.type}</td>
                  <td className="py-3 pr-4">
                    <span className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-primary/10 text-primary">
                      {a.status}
                    </span>
                  </td>
                  <td className="py-3 text-right">
                    <span className="text-sm font-semibold text-foreground">{a.score}%</span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
