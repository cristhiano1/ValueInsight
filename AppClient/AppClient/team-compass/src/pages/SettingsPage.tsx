import { Settings, User, Bell, Sun, Moon, Shield, ChevronRight } from "lucide-react";
import { useState, useEffect } from "react";
import { Switch } from "@/components/ui/switch";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { toast } from "sonner";
import { useLanguage } from "@/context/LanguageContext";

export default function SettingsPage() {
  const { t } = useLanguage();

  const [profile, setProfile] = useState({
    name: "Jane Doe",
    email: "jane.doe@company.com",
    role: "Product Team",
    title: "Product Designer",
  });

  const [notifications, setNotifications] = useState({
    email: true,
    assessmentReminders: true,
    teamUpdates: false,
    weeklyDigest: true,
  });

  const [darkMode, setDarkMode] = useState(() => {
    return localStorage.getItem("theme") === "dark";
  });

  useEffect(() => {
    if (darkMode) {
      document.documentElement.classList.add("dark");
      localStorage.setItem("theme", "dark");
    } else {
      document.documentElement.classList.remove("dark");
      localStorage.setItem("theme", "light");
    }
  }, [darkMode]);

  const [saving, setSaving] = useState(false);

  const handleSaveProfile = async () => {
    setSaving(true);
    try {
      await new Promise((r) => setTimeout(r, 400));
      toast.success(t("profileUpdated") as string);
    } catch {
      toast.error(t("profileSaveFailed") as string);
    } finally {
      setSaving(false);
    }
  };

  const handleNotificationChange = async (key: keyof typeof notifications, checked: boolean) => {
    const updated = { ...notifications, [key]: checked };
    setNotifications(updated);
    try {
      // TODO: Replace with real API call
    } catch {
      setNotifications(notifications);
      toast.error(t("notifUpdateFailed") as string);
    }
  };

  const notificationItems = [
    { key: "email" as const, label: t("emailNotifications"), desc: t("receiveUpdatesEmail") },
    { key: "assessmentReminders" as const, label: t("assessmentReminders"), desc: t("getRemindedAssessments") },
    { key: "teamUpdates" as const, label: t("teamUpdatesNotif"), desc: t("notifyTeamAssessments") },
    { key: "weeklyDigest" as const, label: t("weeklyDigest"), desc: t("receiveWeeklySummary") },
  ];

  const securityItems = [
    { label: t("changePassword"), desc: t("updatePassword") },
    { label: t("twoFactorAuth"), desc: t("addExtraSecurity") },
    { label: t("activeSessions"), desc: t("manageDevices") },
  ];

  return (
    <div className="p-6 md:p-10 max-w-3xl">
      <div className="flex items-center gap-3 mb-8">
        <Settings className="w-6 h-6 text-primary" />
        <h1 className="text-2xl font-semibold text-foreground">{t("settingsTitle")}</h1>
      </div>

      {/* Profile Section */}
      <div className="rounded-lg border border-border bg-card p-6 mb-6">
        <div className="flex items-center gap-3 mb-5">
          <User className="w-5 h-5 text-primary" />
          <h2 className="text-base font-semibold text-foreground">{t("profile")}</h2>
        </div>

        <div className="grid gap-4 sm:grid-cols-2">
          <div className="space-y-1.5">
            <Label htmlFor="name" className="text-sm text-muted-foreground">{t("fullName")}</Label>
            <Input id="name" value={profile.name} onChange={(e) => setProfile({ ...profile, name: e.target.value })} />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="email" className="text-sm text-muted-foreground">{t("emailLabel")}</Label>
            <Input id="email" type="email" value={profile.email} onChange={(e) => setProfile({ ...profile, email: e.target.value })} />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="title" className="text-sm text-muted-foreground">{t("jobTitle")}</Label>
            <Input id="title" value={profile.title} onChange={(e) => setProfile({ ...profile, title: e.target.value })} />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="team" className="text-sm text-muted-foreground">{t("teamLabel")}</Label>
            <Input id="team" value={profile.role} onChange={(e) => setProfile({ ...profile, role: e.target.value })} />
          </div>
        </div>

        <button
          onClick={handleSaveProfile}
          disabled={saving}
          className="mt-5 px-5 py-2.5 rounded-md bg-primary text-primary-foreground text-sm font-medium hover:bg-primary/90 transition-colors press-effect disabled:opacity-50"
        >
          {saving ? t("saving") : t("saveChanges")}
        </button>
      </div>

      {/* Notifications Section */}
      <div className="rounded-lg border border-border bg-card p-6 mb-6">
        <div className="flex items-center gap-3 mb-5">
          <Bell className="w-5 h-5 text-primary" />
          <h2 className="text-base font-semibold text-foreground">{t("notifications")}</h2>
        </div>

        <div className="space-y-4">
          {notificationItems.map((item) => (
            <div key={item.key} className="flex items-center justify-between py-1">
              <div>
                <p className="text-sm font-medium text-foreground">{item.label}</p>
                <p className="text-xs text-muted-foreground">{item.desc}</p>
              </div>
              <Switch
                checked={notifications[item.key]}
                onCheckedChange={(checked) => handleNotificationChange(item.key, checked)}
              />
            </div>
          ))}
        </div>
      </div>

      {/* Appearance Section */}
      <div className="rounded-lg border border-border bg-card p-6 mb-6">
        <div className="flex items-center gap-3 mb-5">
          <Sun className="w-5 h-5 text-primary" />
          <h2 className="text-base font-semibold text-foreground">{t("appearance")}</h2>
        </div>

        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            {darkMode ? <Moon className="w-4 h-4 text-muted-foreground" /> : <Sun className="w-4 h-4 text-muted-foreground" />}
            <div>
              <p className="text-sm font-medium text-foreground">{t("darkMode")}</p>
              <p className="text-xs text-muted-foreground">{t("toggleDarkTheme")}</p>
            </div>
          </div>
          <Switch checked={darkMode} onCheckedChange={setDarkMode} />
        </div>
      </div>

      {/* Security Section */}
      <div className="rounded-lg border border-border bg-card p-6">
        <div className="flex items-center gap-3 mb-5">
          <Shield className="w-5 h-5 text-primary" />
          <h2 className="text-base font-semibold text-foreground">{t("security")}</h2>
        </div>

        <div className="space-y-3">
          {securityItems.map((item) => (
            <button
              key={String(item.label)}
              onClick={() => toast.info(`${item.label} — ${t("availableWhenConnected")}`)}
              className="w-full flex items-center justify-between py-3 px-1 rounded-md hover:bg-secondary/50 transition-colors press-effect"
            >
              <div className="text-left">
                <p className="text-sm font-medium text-foreground">{item.label}</p>
                <p className="text-xs text-muted-foreground">{item.desc}</p>
              </div>
              <ChevronRight className="w-4 h-4 text-muted-foreground" />
            </button>
          ))}
        </div>
      </div>
    </div>
  );
}
