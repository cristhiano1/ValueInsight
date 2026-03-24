import { ReactNode, useState } from "react";
import AppSidebar from "./AppSidebar";
import { Bell, HelpCircle, Menu, X, BookOpen, MessageCircle, ExternalLink } from "lucide-react";
import { useLanguage } from "@/context/LanguageContext";
import { Language } from "@/i18n/translations";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  DropdownMenuSeparator,
  DropdownMenuLabel,
} from "@/components/ui/dropdown-menu";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";

export default function AppLayout({ children }: { children: ReactNode }) {
  const [mobileOpen, setMobileOpen] = useState(false);
  const { language, setLanguage, t } = useLanguage();

  const MOCK_NOTIFICATIONS = [
    { id: 1, title: t("notifAssessmentComplete"), description: t("notifAssessmentCompleteDesc"), time: t("timeAgo2m"), read: false },
    { id: 2, title: t("notifTeamUpdate"), description: t("notifTeamUpdateDesc"), time: t("timeAgo1h"), read: false },
    { id: 3, title: t("notifWeeklyReport"), description: t("notifWeeklyReportDesc"), time: t("timeAgo3h"), read: true },
    { id: 4, title: t("notifNewFeature"), description: t("notifNewFeatureDesc"), time: t("timeAgo1d"), read: true },
  ];

  const [notifications, setNotifications] = useState(MOCK_NOTIFICATIONS);

  const markAllRead = () => {
    setNotifications((prev) => prev.map((n) => ({ ...n, read: true })));
  };

  const unreadCount = notifications.filter((n) => !n.read).length;

  const markAsRead = (id: number) => {
    setNotifications((prev) =>
      prev.map((n) => (n.id === id ? { ...n, read: true } : n))
    );
  };

  return (
    <div className="flex min-h-screen bg-background">
      <AppSidebar />
      <div className="flex-1 flex flex-col min-h-screen">
        {/* Top bar */}
        <header className="sticky top-0 z-30 flex items-center justify-between h-14 px-4 md:px-8 border-b border-border bg-background/80 backdrop-blur-sm">
          <button
            className="md:hidden p-2 rounded-inner hover:bg-secondary transition-colors"
            onClick={() => setMobileOpen(!mobileOpen)}
          >
            <Menu className="w-5 h-5 text-foreground" />
          </button>
          <div className="flex-1" />
          <div className="flex items-center gap-1">
            {/* Language switcher */}
            <div className="flex items-center gap-0.5 mr-1">
              {(["en", "sv"] as Language[]).map((lang) => (
                <button
                  key={lang}
                  onClick={() => setLanguage(lang)}
                  className={`px-2 py-1 rounded-inner text-xs font-medium transition-colors ${
                    language === lang
                      ? "bg-primary text-primary-foreground"
                      : "text-muted-foreground hover:bg-secondary/50"
                  }`}
                >
                  {lang === "en" ? "EN" : "SV"}
                </button>
              ))}
            </div>
            {/* Help dropdown */}
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <button className="p-2 rounded-inner hover:bg-secondary transition-colors press-effect">
                  <HelpCircle className="w-[18px] h-[18px] text-muted-foreground" />
                </button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-52">
                <DropdownMenuLabel>{t("helpSupport")}</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuItem className="gap-2 cursor-pointer">
                  <BookOpen className="w-4 h-4" />
                  {t("documentation")}
                </DropdownMenuItem>
                <DropdownMenuItem className="gap-2 cursor-pointer">
                  <MessageCircle className="w-4 h-4" />
                  {t("contactSupport")}
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem className="gap-2 cursor-pointer">
                  <ExternalLink className="w-4 h-4" />
                  {t("whatsNew")}
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>

            {/* Notifications popover */}
            <Popover>
              <PopoverTrigger asChild>
                <button className="relative p-2 rounded-inner hover:bg-secondary transition-colors press-effect">
                  <Bell className="w-[18px] h-[18px] text-muted-foreground" />
                  {unreadCount > 0 && (
                    <span className="absolute top-1.5 right-1.5 w-2 h-2 bg-primary rounded-full" />
                  )}
                </button>
              </PopoverTrigger>
              <PopoverContent align="end" className="w-80 p-0">
                <div className="flex items-center justify-between px-4 py-3 border-b border-border">
                  <h3 className="text-sm font-semibold text-foreground">{t("notifications")}</h3>
                  {unreadCount > 0 && (
                    <button
                      onClick={markAllRead}
                      className="text-xs text-primary hover:underline font-medium"
                     >
                      {t("markAllRead")}
                    </button>
                  )}
                </div>
                <div className="max-h-72 overflow-y-auto">
                  {notifications.map((n) => (
                    <button
                      key={n.id}
                      onClick={() => markAsRead(n.id)}
                      className={`w-full text-left px-4 py-3 border-b border-border last:border-0 hover:bg-secondary/50 transition-colors ${
                        !n.read ? "bg-primary/5" : ""
                      }`}
                    >
                      <div className="flex items-start gap-2">
                        {!n.read && (
                          <span className="mt-1.5 w-2 h-2 rounded-full bg-primary shrink-0" />
                        )}
                        <div className={!n.read ? "" : "pl-4"}>
                          <p className="text-sm font-medium text-foreground">{n.title}</p>
                          <p className="text-xs text-muted-foreground mt-0.5">{n.description}</p>
                          <p className="text-xs text-muted-foreground/60 mt-1">{n.time}</p>
                        </div>
                      </div>
                    </button>
                  ))}
                </div>
              </PopoverContent>
            </Popover>
          </div>
        </header>
        <main className="flex-1">{children}</main>
      </div>
    </div>
  );
}
