import { useState } from "react";
import { motion } from "framer-motion";
import { Layers, Mail, Lock, ArrowRight, CheckCircle2, Sparkles, Heart, BarChart3, Target } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useApp } from "@/context/AppContext";
import { useLanguage } from "@/context/LanguageContext";
import { fadeInUp, fadeInUpDelay } from "@/lib/motion";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showLogin, setShowLogin] = useState(false);
  const navigate = useNavigate();
  const { setIsAuthenticated } = useApp();
  const { t } = useLanguage();

  const handleLogin = (e: React.FormEvent) => {
    e.preventDefault();
    setIsAuthenticated(true);
    navigate("/dashboard");
  };

  const benefits = [
    t("benefitDecision"),
    t("benefitCommunication"),
    t("benefitTeams"),
    t("benefitGrowth"),
  ];

  const badges = [
    { icon: Sparkles, label: t("badgeSelfInsight") },
    { icon: Heart, label: t("badgeCollaboration") },
    { icon: BarChart3, label: t("badgeLeadership") },
    { icon: Target, label: t("badgeFocus") },
  ];

  return (
    <div className="min-h-screen bg-background">
      {/* Navbar */}
      <nav className="flex items-center justify-between px-6 md:px-12 py-4 bg-card/80 backdrop-blur-sm border-b border-border sticky top-0 z-50">
        <div className="flex items-center gap-2">
          <div className="w-8 h-8 rounded-inner gradient-hero flex items-center justify-center">
            <Layers className="w-4 h-4 text-primary-foreground" />
          </div>
          <span className="text-body font-bold text-foreground">
            Value<span className="text-primary">Insight</span>
          </span>
        </div>
        <div className="flex items-center gap-4">
          <button className="text-ui-sm text-muted-foreground hover:text-foreground transition-colors">{t("home")}</button>
          <button
            onClick={() => setShowLogin(!showLogin)}
            className="text-ui-sm text-muted-foreground hover:text-foreground transition-colors"
          >
            {t("signIn")}
          </button>
          <button
            onClick={() => setShowLogin(!showLogin)}
            className="h-9 px-4 rounded-lg bg-primary text-primary-foreground text-ui-sm font-medium hover:opacity-90 transition-opacity press-effect"
          >
            {t("register")}
          </button>
        </div>
      </nav>

      {/* Hero Section */}
      <section className="px-6 md:px-12 py-16 md:py-24 max-w-[1200px] mx-auto">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-12 items-start">
          {/* Left column */}
          <motion.div {...fadeInUp}>
            <div className="inline-flex items-center gap-2 px-4 py-1.5 rounded-full bg-primary/10 border border-primary/20 mb-6">
              <div className="w-2 h-2 rounded-full bg-primary animate-pulse" />
              <span className="text-xs font-medium text-primary">{t("heroTagline")}</span>
            </div>

            <h1 className="text-display font-bold text-foreground leading-tight mb-6">
              {t("heroTitle")}{" "}
              <span className="gradient-text">{t("heroTitleHighlight")}</span>
            </h1>

            <p className="text-body text-muted-foreground mb-8 max-w-lg">
              {t("heroDescription")}
            </p>

            <button
              onClick={() => {
                setIsAuthenticated(true);
                navigate("/onboarding");
              }}
              className="h-12 px-8 rounded-xl bg-primary text-primary-foreground text-body font-semibold hover:opacity-90 transition-opacity press-effect flex items-center gap-2 shadow-surface-lg mb-10"
            >
              {t("startMapping")}
              <ArrowRight className="w-5 h-5" />
            </button>

            <div className="mb-4">
              <p className="text-ui-sm font-semibold text-foreground mb-3">{t("trustedToSupport")}</p>
              <div className="flex flex-wrap gap-2">
                {badges.map((b) => (
                  <div
                    key={b.label as string}
                    className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-sky/10 border border-sky/20 text-xs font-medium text-sky"
                  >
                    <b.icon className="w-3.5 h-3.5" />
                    {b.label as string}
                  </div>
                ))}
              </div>
            </div>
          </motion.div>

          {/* Right column — Why it matters card OR Login form */}
          {!showLogin ? (
            <motion.div {...fadeInUpDelay(0.15)} className="card-surface p-8 relative overflow-hidden">
              <div className="absolute inset-0 bg-gradient-to-br from-primary/5 via-sky/5 to-transparent pointer-events-none" />
              <div className="relative">
                <div className="flex items-center gap-3 mb-6">
                  <div className="w-10 h-10 rounded-xl gradient-hero flex items-center justify-center">
                    <Layers className="w-5 h-5 text-primary-foreground" />
                  </div>
                  <div>
                    <h3 className="text-body font-semibold text-foreground">{t("whyMatters")}</h3>
                    <p className="text-xs text-muted-foreground">{t("whyMattersSubtitle")}</p>
                  </div>
                </div>

                <div className="space-y-4 mb-6">
                  {benefits.map((b) => (
                    <div key={b as string} className="flex items-center gap-3">
                      <CheckCircle2 className="w-5 h-5 text-sky flex-shrink-0" />
                      <span className="text-ui-sm text-foreground font-medium">{b as string}</span>
                    </div>
                  ))}
                </div>

                <div className="pt-5 border-t border-border">
                  <p className="text-xs text-muted-foreground leading-relaxed">
                    {t("heroFooterNote")}
                  </p>
                </div>
              </div>
            </motion.div>
          ) : (
            <motion.div {...fadeInUp} className="card-surface p-8">
              <h2 className="text-body font-semibold text-foreground mb-6">{t("signInTitle")}</h2>
              <form onSubmit={handleLogin} className="space-y-4">
                <div>
                  <label className="block text-ui-sm font-medium text-foreground mb-1.5">{t("email")}</label>
                  <div className="relative">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                    <input
                      type="email"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      placeholder="you@company.com"
                      className="w-full h-10 pl-10 pr-4 rounded-inner border border-input bg-background text-ui-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring/20 focus:border-primary transition-colors"
                    />
                  </div>
                </div>
                <div>
                  <label className="block text-ui-sm font-medium text-foreground mb-1.5">{t("password")}</label>
                  <div className="relative">
                    <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
                    <input
                      type="password"
                      value={password}
                      onChange={(e) => setPassword(e.target.value)}
                      placeholder="••••••••"
                      className="w-full h-10 pl-10 pr-4 rounded-inner border border-input bg-background text-ui-sm text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring/20 focus:border-primary transition-colors"
                    />
                  </div>
                </div>
                <div className="flex justify-end">
                  <button type="button" className="text-ui-sm text-primary hover:underline">{t("forgotPassword")}</button>
                </div>
                <button
                  type="submit"
                  className="w-full h-10 bg-primary text-primary-foreground rounded-inner text-ui-sm font-medium hover:opacity-90 transition-opacity press-effect flex items-center justify-center gap-2"
                >
                  {t("signIn")}
                  <ArrowRight className="w-4 h-4" />
                </button>
              </form>
              <div className="mt-6 pt-6 border-t border-border">
                <button className="w-full h-10 border border-border rounded-inner text-ui-sm font-medium text-foreground hover:bg-secondary transition-colors press-effect">
                  {t("signInSSO")}
                </button>
              </div>
            </motion.div>
          )}
        </div>
      </section>

      {/* Footer */}
      <footer className="border-t border-border py-6 text-center">
        <p className="text-xs text-muted-foreground">© 2026 — ValueInsight</p>
      </footer>
    </div>
  );
}
