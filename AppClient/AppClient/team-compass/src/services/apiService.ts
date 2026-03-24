// API Service - Replace mock data with real API calls
// Each function maps to an ASP.NET Core controller endpoint

import { apiFetch } from './apiConfig';
import type { Value, TeamMember, CultureType, AIInsight, ValueCategory } from '@/data/mockData';

// ─── Values ───
export const ValuesApi = {
  getAll: () => apiFetch<Value[]>('/values'),
  getByCategory: (category: ValueCategory) => apiFetch<Value[]>(`/values?category=${category}`),
};

// ─── Categories ───
export const CategoriesApi = {
  getAll: () => apiFetch<{ id: ValueCategory; label: string; color: string; description: string }[]>('/categories'),
};

// ─── User Assessment ───
export interface UserAssessment {
  userId: string;
  selectedValues: string[];    // value IDs (10)
  narrowedValues: string[];    // value IDs (5)
  rankedValues: string[];      // value IDs (top 3)
  reflections: string[];
  concretizations: {
    valueId: string;
    meaning: string;
    behavior: string;
    violated: string;
  }[];
}

export const AssessmentApi = {
  get: (userId: string) => apiFetch<UserAssessment>(`/assessment/${userId}`),
  save: (data: UserAssessment) => apiFetch<UserAssessment>('/assessment', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  update: (userId: string, data: Partial<UserAssessment>) => apiFetch<UserAssessment>(`/assessment/${userId}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
};

// ─── Team ───
export const TeamApi = {
  getMembers: (teamId: string) => apiFetch<TeamMember[]>(`/teams/${teamId}/members`),
  getTeamValues: (teamId: string) => apiFetch<{ name: string; count: number; percentage: number }[]>(`/teams/${teamId}/values`),
  getCategoryScores: (teamId: string) => apiFetch<Record<ValueCategory, number>>(`/teams/${teamId}/category-scores`),
  getCultureType: (teamId: string) => apiFetch<CultureType>(`/teams/${teamId}/culture-type`),
};

// ─── AI Insights ───
export const InsightsApi = {
  getForTeam: (teamId: string) => apiFetch<AIInsight[]>(`/teams/${teamId}/insights`),
};

// ─── Cultural Fit Score ───
export interface CFSResult {
  score: number;
  categoryAlignment: number;
  topOverlap: number;
  dominanceMatch: number;
  tensionScore: number;
}

export const FitScoreApi = {
  calculate: (userId: string, teamId: string) => apiFetch<CFSResult>(`/fit-score?userId=${userId}&teamId=${teamId}`),
};

// ─── Auth (if needed) ───
export const AuthApi = {
  login: (credentials: { email: string; password: string }) => apiFetch<{ token: string; userId: string }>('/auth/login', {
    method: 'POST',
    body: JSON.stringify(credentials),
  }),
  changePassword: (data: { currentPassword: string; newPassword: string }) => apiFetch('/auth/change-password', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
};

// ─── Settings / Profile ───
export interface UserProfile {
  name: string;
  email: string;
  role: string;
  title: string;
}

export interface NotificationSettings {
  email: boolean;
  assessmentReminders: boolean;
  teamUpdates: boolean;
  weeklyDigest: boolean;
}

export const ProfileApi = {
  get: (userId: string) => apiFetch<UserProfile>(`/profile/${userId}`),
  update: (userId: string, data: UserProfile) => apiFetch<UserProfile>(`/profile/${userId}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
};

export const SettingsApi = {
  getNotifications: (userId: string) => apiFetch<NotificationSettings>(`/settings/${userId}/notifications`),
  updateNotifications: (userId: string, data: NotificationSettings) => apiFetch<NotificationSettings>(`/settings/${userId}/notifications`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
};
