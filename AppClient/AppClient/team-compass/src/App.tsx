import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { Toaster } from "@/components/ui/toaster";
import { TooltipProvider } from "@/components/ui/tooltip";
import { AppProvider } from "@/context/AppContext";
import { LanguageProvider } from "@/context/LanguageContext";
import LoginPage from "./pages/LoginPage";
import OnboardingPage from "./pages/OnboardingPage";
import ReflectionPage from "./pages/ReflectionPage";
import DashboardPage from "./pages/DashboardPage";
import ValuesSelectionPage from "./pages/ValuesSelectionPage";
import NarrowValuesPage from "./pages/NarrowValuesPage";
import PrioritizationPage from "./pages/PrioritizationPage";
import ConcretizationPage from "./pages/ConcretizationPage";
import ProfilePage from "./pages/ProfilePage";
import TeamDashboardPage from "./pages/TeamDashboardPage";
import SettingsPage from "./pages/SettingsPage";
import ReportsPage from "./pages/ReportsPage";
import AppLayout from "./components/AppLayout";
import NotFound from "./pages/NotFound";

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <LanguageProvider>
        <AppProvider>
          <Toaster />
          <Sonner />
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<LoginPage />} />
              <Route path="/onboarding" element={<OnboardingPage />} />
              <Route path="/reflection" element={<ReflectionPage />} />
              <Route path="/assessment" element={<AppLayout><ValuesSelectionPage /></AppLayout>} />
              <Route path="/narrow" element={<AppLayout><NarrowValuesPage /></AppLayout>} />
              <Route path="/prioritize" element={<AppLayout><PrioritizationPage /></AppLayout>} />
              <Route path="/concretize" element={<AppLayout><ConcretizationPage /></AppLayout>} />
              <Route path="/profile" element={<AppLayout><ProfilePage /></AppLayout>} />
              <Route path="/dashboard" element={<AppLayout><DashboardPage /></AppLayout>} />
              <Route path="/team" element={<AppLayout><TeamDashboardPage /></AppLayout>} />
              <Route path="/reports" element={<AppLayout><ReportsPage /></AppLayout>} />
              <Route path="/settings" element={<AppLayout><SettingsPage /></AppLayout>} />
              <Route path="*" element={<NotFound />} />
            </Routes>
          </BrowserRouter>
        </AppProvider>
      </LanguageProvider>
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
