import { useLocation, useNavigate } from "react-router-dom";
import { motion } from "framer-motion";
import {
  LayoutDashboard, Heart, Users, FileBarChart, Settings, LogOut, Compass, Layers, Globe,
} from "lucide-react";
import { useLanguage } from "@/context/LanguageContext";
import { Language } from "@/i18n/translations";

export default function AppSidebar() {
  const location = useLocation();
  const navigate = useNavigate();
  const { t, language, setLanguage } = useLanguage();

  const NAV_ITEMS = [
    { path: "/dashboard", label: t("dashboard"), icon: LayoutDashboard },
    { path: "/assessment", label: t("assessment"), icon: Compass },
    { path: "/profile", label: t("myValues"), icon: Heart },
    { path: "/team", label: t("teamPulse"), icon: Users },
    { path: "/reports", label: t("reports"), icon: FileBarChart },
    { path: "/settings", label: t("settings"), icon: Settings },
  ];

  const isActive = (path: string) => {
    if (path === "/assessment") {
      return ["/assessment", "/narrow", "/prioritize", "/concretize"].includes(location.pathname);
    }
    return location.pathname === path;
  };

  return (
    <aside className="hidden md:flex flex-col w-[260px] min-h-screen border-r border-border bg-sidebar p-4">
      <div className="flex items-center gap-3 px-3 py-4 mb-6">
        <div className="w-8 h-8 rounded-inner gradient-hero flex items-center justify-center">
          <Layers className="w-4 h-4 text-primary-foreground" />
        </div>
        <span className="text-body font-semibold text-foreground tracking-tight">Value<span className="text-primary">Insight</span></span>
      </div>

      <div className="flex items-center gap-3 px-3 py-3 mb-6 rounded-inner bg-secondary/50">
        <div className="w-9 h-9 rounded-full bg-primary/10 flex items-center justify-center text-ui-sm font-semibold text-primary">
          JD
        </div>
        <div className="flex flex-col">
          <span className="text-ui-sm font-medium text-foreground">Jane Doe</span>
          <span className="text-xs text-muted-foreground">Product Team</span>
        </div>
      </div>

      <nav className="flex-1 space-y-1">
        {NAV_ITEMS.map((item) => {
          const active = isActive(item.path);
          return (
            <button
              key={item.path}
              onClick={() => navigate(item.path)}
              className={`relative w-full flex items-center gap-3 px-3 py-2.5 rounded-inner text-ui-sm font-medium transition-colors press-effect ${
                active
                  ? "text-primary bg-primary/5"
                  : "text-muted-foreground hover:text-foreground hover:bg-secondary/50"
              }`}
            >
              {active && (
                <motion.div
                  layoutId="sidebar-indicator"
                  className="absolute left-0 top-1/2 -translate-y-1/2 w-[3px] h-5 rounded-full bg-primary"
                  transition={{ type: "spring", stiffness: 300, damping: 30 }}
                />
              )}
              <item.icon className="w-[18px] h-[18px]" />
              {item.label}
            </button>
          );
        })}
      </nav>



      <button
        onClick={() => navigate("/")}
        className="flex items-center gap-3 px-3 py-2.5 rounded-inner text-ui-sm font-medium text-muted-foreground hover:text-foreground hover:bg-secondary/50 transition-colors press-effect"
      >
        <LogOut className="w-[18px] h-[18px]" />
        {t("signOut")}
      </button>
    </aside>
  );
}
