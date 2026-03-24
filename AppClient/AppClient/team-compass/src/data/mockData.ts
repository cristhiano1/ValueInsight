// Values Mapping Platform - Complete Data Model
// Based on 6 categories, 60 values, 6 culture types

export type ValueCategory =
  | "relation"
  | "result"
  | "structure"
  | "autonomy"
  | "development"
  | "meaning";

export interface Value {
  id: string;
  name: string;
  description: string;
  category: ValueCategory;
}

export interface TeamMember {
  id: string;
  name: string;
  avatar: string;
  role: string;
  topValues: string[];
  fitScore: number;
  categoryScores: Record<ValueCategory, number>;
}

export interface CultureType {
  id: string;
  name: string;
  dominantCategories: [ValueCategory, ValueCategory];
  characteristics: string[];
  strengths: string[];
  risks: string[];
  color: string;
}

export interface AIInsight {
  id: string;
  title: string;
  description: string;
  type: "strength" | "risk" | "coaching" | "intervention" | "warning";
}

export interface PolarityPair {
  categoryA: ValueCategory;
  categoryB: ValueCategory;
  labelA: string;
  labelB: string;
}

// ─── 6 Categories ───
export const CATEGORIES: { id: ValueCategory; label: string; color: string; description: string }[] = [
  { id: "relation", label: "Relation & Trust", color: "hsl(225, 60%, 45%)", description: "Interaction, psychological safety, respect" },
  { id: "result", label: "Results & Performance", color: "hsl(0, 72%, 51%)", description: "Delivery, efficiency, ambition" },
  { id: "structure", label: "Structure & Stability", color: "hsl(38, 92%, 50%)", description: "Order, security, predictability" },
  { id: "autonomy", label: "Autonomy & Freedom", color: "hsl(152, 55%, 42%)", description: "Independence and freedom of action" },
  { id: "development", label: "Development & Innovation", color: "hsl(280, 55%, 55%)", description: "Learning and future-orientation" },
  { id: "meaning", label: "Meaning & Purpose", color: "hsl(200, 65%, 45%)", description: "Existential direction and impact" },
];

// ─── 60 Values (10 per category) ───
export const VALUES: Value[] = [
  // 1. Relation & Trust
  { id: "r1", name: "Trust", description: "I assume that others mean well", category: "relation" },
  { id: "r2", name: "Respect", description: "I treat others with dignity", category: "relation" },
  { id: "r3", name: "Empathy", description: "I try to understand others' perspectives", category: "relation" },
  { id: "r4", name: "Care", description: "I care about people's well-being", category: "relation" },
  { id: "r5", name: "Collaboration", description: "I value shared success", category: "relation" },
  { id: "r6", name: "Openness", description: "I share thoughts and feelings honestly", category: "relation" },
  { id: "r7", name: "Transparency", description: "I am clear about my intentions", category: "relation" },
  { id: "r8", name: "Loyalty", description: "I stand behind my team", category: "relation" },
  { id: "r9", name: "Inclusion", description: "Everyone should feel welcome", category: "relation" },
  { id: "r10", name: "Fairness", description: "Decisions should be impartial", category: "relation" },

  // 2. Results & Performance
  { id: "p1", name: "Results Focus", description: "I prioritize delivery", category: "result" },
  { id: "p2", name: "Efficiency", description: "Resources are used wisely", category: "result" },
  { id: "p3", name: "Quality", description: "Work should meet high standards", category: "result" },
  { id: "p4", name: "Accountability", description: "I own my commitments", category: "result" },
  { id: "p5", name: "Drive", description: "I want to achieve high goals", category: "result" },
  { id: "p6", name: "Decisiveness", description: "I prefer action over hesitation", category: "result" },
  { id: "p7", name: "Clarity", description: "Expectations should be clear", category: "result" },
  { id: "p8", name: "Discipline", description: "I do what is required", category: "result" },
  { id: "p9", name: "Competitiveness", description: "We should be the best", category: "result" },
  { id: "p10", name: "Reliability", description: "I keep my promises", category: "result" },

  // 3. Structure & Stability
  { id: "s1", name: "Structure", description: "Clear frameworks are needed", category: "structure" },
  { id: "s2", name: "Planning", description: "Preparation is important", category: "structure" },
  { id: "s3", name: "Predictability", description: "Stability provides security", category: "structure" },
  { id: "s4", name: "Security", description: "Risks should be minimized", category: "structure" },
  { id: "s5", name: "Consistency", description: "Rules should be followed", category: "structure" },
  { id: "s6", name: "Order", description: "System over chaos", category: "structure" },
  { id: "s7", name: "Long-term Thinking", description: "Think several steps ahead", category: "structure" },
  { id: "s8", name: "Stability", description: "Avoid unnecessary changes", category: "structure" },
  { id: "s9", name: "Control", description: "Maintain oversight", category: "structure" },
  { id: "s10", name: "Role Clarity", description: "Roles should be well-defined", category: "structure" },

  // 4. Autonomy & Freedom
  { id: "a1", name: "Freedom", description: "I want room to act", category: "autonomy" },
  { id: "a2", name: "Independence", description: "I make my own decisions", category: "autonomy" },
  { id: "a3", name: "Flexibility", description: "Adaptation is important", category: "autonomy" },
  { id: "a4", name: "Courage", description: "I dare to take risks", category: "autonomy" },
  { id: "a5", name: "Integrity", description: "I stand by my principles", category: "autonomy" },
  { id: "a6", name: "Self-expression", description: "I get to be myself", category: "autonomy" },
  { id: "a7", name: "Self-reliance", description: "Minimal dependence on others", category: "autonomy" },
  { id: "a8", name: "Initiative", description: "I act without being told", category: "autonomy" },
  { id: "a9", name: "Authenticity", description: "I am genuine", category: "autonomy" },
  { id: "a10", name: "Drive to Act", description: "I push things forward", category: "autonomy" },

  // 5. Development & Innovation
  { id: "d1", name: "Learning", description: "I want to continuously develop", category: "development" },
  { id: "d2", name: "Innovation", description: "New ideas are important", category: "development" },
  { id: "d3", name: "Creativity", description: "I think outside the box", category: "development" },
  { id: "d4", name: "Change Readiness", description: "I embrace change", category: "development" },
  { id: "d5", name: "Curiosity", description: "I explore", category: "development" },
  { id: "d6", name: "Improvement", description: "Small steps forward every day", category: "development" },
  { id: "d7", name: "Vision", description: "I think long-term", category: "development" },
  { id: "d8", name: "Growth", description: "Expansion is positive", category: "development" },
  { id: "d9", name: "Experimentation", description: "Test and adjust", category: "development" },
  { id: "d10", name: "Challenge", description: "I seek to stretch myself", category: "development" },

  // 6. Meaning & Purpose
  { id: "m1", name: "Purpose", description: "Work should have meaning", category: "meaning" },
  { id: "m2", name: "Social Responsibility", description: "We contribute to something greater", category: "meaning" },
  { id: "m3", name: "Sustainability", description: "Long-term impact matters", category: "meaning" },
  { id: "m4", name: "Ethics", description: "Decisions should be morally justifiable", category: "meaning" },
  { id: "m5", name: "Servant Leadership", description: "I support others", category: "meaning" },
  { id: "m6", name: "Impact", description: "I want to make a difference", category: "meaning" },
  { id: "m7", name: "Passion", description: "I'm driven by engagement", category: "meaning" },
  { id: "m8", name: "Commitment", description: "I invest energy", category: "meaning" },
  { id: "m9", name: "Value-driven Action", description: "Acting from core beliefs", category: "meaning" },
  { id: "m10", name: "Balance", description: "Sustainable life and work", category: "meaning" },
];

// ─── 6 Culture Types ───
export const CULTURE_TYPES: CultureType[] = [
  {
    id: "performance",
    name: "Performance Culture",
    dominantCategories: ["result", "structure"],
    characteristics: ["High pace", "Clear goals", "Strong delivery expectations", "Efficiency prioritized"],
    strengths: ["High productivity", "Clear direction", "Strong accountability"],
    risks: ["Low psychological safety", "Stress", "Short-term perspective"],
    color: "hsl(0, 72%, 51%)",
  },
  {
    id: "relational",
    name: "Relational Culture",
    dominantCategories: ["relation", "meaning"],
    characteristics: ["Care", "Inclusion", "Purpose-driven"],
    strengths: ["High trust", "Strong team cohesion", "Loyalty"],
    risks: ["Conflict avoidance", "Unclear expectations", "Lower performance focus"],
    color: "hsl(225, 60%, 45%)",
  },
  {
    id: "innovation",
    name: "Innovation Culture",
    dominantCategories: ["development", "autonomy"],
    characteristics: ["Experimentation", "Creative freedom", "Change-driven"],
    strengths: ["Adaptability", "Rich ideas", "Entrepreneurial spirit"],
    risks: ["Lack of structure", "Scattered focus", "Implementation issues"],
    color: "hsl(280, 55%, 55%)",
  },
  {
    id: "stability",
    name: "Stability Culture",
    dominantCategories: ["structure", "relation"],
    characteristics: ["Predictability", "Security", "Clear roles"],
    strengths: ["Stable work environment", "Low conflict", "Controlled processes"],
    risks: ["Change resistance", "Inertia", "Lack of innovation"],
    color: "hsl(38, 92%, 50%)",
  },
  {
    id: "entrepreneurial",
    name: "Entrepreneurial Culture",
    dominantCategories: ["result", "autonomy"],
    characteristics: ["High performance", "Independent individuals", "Competition"],
    strengths: ["Fast execution", "Individual drive"],
    risks: ["Internal rivalry", "Weak cohesion", "Burnout"],
    color: "hsl(152, 55%, 42%)",
  },
  {
    id: "purpose_driven",
    name: "Purpose-Driven Culture",
    dominantCategories: ["meaning", "development"],
    characteristics: ["Vision", "Long-term orientation", "Values-driven change"],
    strengths: ["Engagement", "Sustainable development"],
    risks: ["Idealism without execution", "Lack of short-term delivery discipline"],
    color: "hsl(200, 65%, 45%)",
  },
];

// ─── Polarity Pairs (Tension Detection) ───
export const POLARITY_PAIRS: PolarityPair[] = [
  { categoryA: "autonomy", categoryB: "structure", labelA: "Freedom", labelB: "Structure" },
  { categoryA: "result", categoryB: "relation", labelA: "Results", labelB: "Relationships" },
  { categoryA: "development", categoryB: "structure", labelA: "Innovation", labelB: "Stability" },
  { categoryA: "autonomy", categoryB: "relation", labelA: "Control", labelB: "Trust" },
];

// ─── CFS Score Interpretation ───
export const CFS_INTERPRETATION = [
  { min: 80, max: 100, label: "Strong Cultural Match", description: "Your values closely align with your team's culture. You likely feel energized and at home." },
  { min: 60, max: 79, label: "Good Match", description: "Solid alignment with some areas of natural difference that can bring valuable diversity." },
  { min: 40, max: 59, label: "Moderate Tension", description: "Noticeable differences between your values and the team culture. Consider dialogue strategies." },
  { min: 20, max: 39, label: "Significant Tension", description: "Your values differ substantially from the team. Proactive communication is important." },
  { min: 0, max: 19, label: "High Risk", description: "Major misalignment that may cause stress. Seek support and open dialogue with leadership." },
];

// ─── Reflection Questions (Step 2) ───
export const REFLECTION_QUESTIONS = [
  "When do you feel most proud in your work?",
  "What really frustrates you at work?",
  "What needs to be in place for you to perform at your best?",
  "What behaviors do you find hard to tolerate?",
];

// ─── Concretization Prompts (Step 7) ───
export const CONCRETIZATION_PROMPTS = [
  "What does this value mean to you personally?",
  "How is this value visible in your behavior?",
  "What happens when this value is not respected?",
];

// ─── Mock Team Data ───
export const TEAM_MEMBERS: TeamMember[] = [
  {
    id: "1", name: "Sarah Chen", avatar: "SC", role: "Product Designer",
    topValues: ["Innovation", "Collaboration", "Creativity", "Empathy", "Growth"],
    fitScore: 88,
    categoryScores: { relation: 30, result: 10, structure: 5, autonomy: 15, development: 30, meaning: 10 },
  },
  {
    id: "2", name: "Marcus Johnson", avatar: "MJ", role: "Engineering Lead",
    topValues: ["Results Focus", "Accountability", "Quality", "Discipline", "Structure"],
    fitScore: 74,
    categoryScores: { relation: 5, result: 35, structure: 30, autonomy: 10, development: 10, meaning: 10 },
  },
  {
    id: "3", name: "Elena Rodriguez", avatar: "ER", role: "Project Manager",
    topValues: ["Collaboration", "Transparency", "Trust", "Accountability", "Planning"],
    fitScore: 92,
    categoryScores: { relation: 30, result: 20, structure: 25, autonomy: 5, development: 10, meaning: 10 },
  },
  {
    id: "4", name: "David Park", avatar: "DP", role: "Data Analyst",
    topValues: ["Integrity", "Quality", "Learning", "Curiosity", "Ethics"],
    fitScore: 81,
    categoryScores: { relation: 10, result: 15, structure: 15, autonomy: 15, development: 25, meaning: 20 },
  },
  {
    id: "5", name: "Amira Patel", avatar: "AP", role: "UX Researcher",
    topValues: ["Empathy", "Inclusion", "Respect", "Curiosity", "Purpose"],
    fitScore: 85,
    categoryScores: { relation: 35, result: 5, structure: 5, autonomy: 10, development: 20, meaning: 25 },
  },
];

export const TEAM_VALUES = [
  { name: "Collaboration", count: 4, percentage: 80 },
  { name: "Trust", count: 3, percentage: 60 },
  { name: "Accountability", count: 3, percentage: 60 },
  { name: "Quality", count: 2, percentage: 40 },
  { name: "Empathy", count: 2, percentage: 40 },
  { name: "Curiosity", count: 2, percentage: 40 },
  { name: "Innovation", count: 2, percentage: 40 },
];

export const AI_INSIGHTS: AIInsight[] = [
  {
    id: "1",
    title: "Strength: High relational foundation",
    description: "Your team scores highest in Relation & Trust. This means strong psychological safety and open communication. Leverage this by using team values in feedback conversations.",
    type: "strength",
  },
  {
    id: "2",
    title: "Risk: Low innovation focus",
    description: "Development & Innovation scores are moderate. Consider creating 'exploration sprints' where team members experiment with new approaches relevant to current projects.",
    type: "risk",
  },
  {
    id: "3",
    title: "Coaching: Bridge the autonomy-structure tension",
    description: "Marcus values Structure while Sarah values Freedom. Define clear 'deep work' hours vs. 'sync' hours. Use shared definitions of done to satisfy both perspectives.",
    type: "coaching",
  },
  {
    id: "4",
    title: "Intervention: Team values workshop",
    description: "With 60% alignment, schedule a facilitated workshop asking: 'How do we want to work together?' Use polarity mapping to address Freedom ↔ Structure tension constructively.",
    type: "intervention",
  },
  {
    id: "5",
    title: "Warning: Polarization detected",
    description: "Results & Performance vs. Relation & Trust shows notable spread. Monitor for silent disengagement from members who feel their values aren't represented.",
    type: "warning",
  },
];

// ─── Helper: Calculate team category scores ───
export function getTeamCategoryScores(): Record<ValueCategory, number> {
  const totals: Record<ValueCategory, number> = {
    relation: 0, result: 0, structure: 0, autonomy: 0, development: 0, meaning: 0,
  };
  TEAM_MEMBERS.forEach((m) => {
    (Object.keys(totals) as ValueCategory[]).forEach((cat) => {
      totals[cat] += m.categoryScores[cat];
    });
  });
  const sum = Object.values(totals).reduce((a, b) => a + b, 0);
  const normalized: Record<string, number> = {};
  (Object.keys(totals) as ValueCategory[]).forEach((cat) => {
    normalized[cat] = Math.round((totals[cat] / sum) * 100);
  });
  return normalized as Record<ValueCategory, number>;
}

// ─── Helper: Determine culture type from top 2 categories ───
export function determineCultureType(scores: Record<ValueCategory, number>): CultureType {
  const sorted = (Object.entries(scores) as [ValueCategory, number][])
    .sort((a, b) => b[1] - a[1]);
  const top2 = new Set([sorted[0][0], sorted[1][0]]);

  for (const ct of CULTURE_TYPES) {
    if (top2.has(ct.dominantCategories[0]) && top2.has(ct.dominantCategories[1])) {
      return ct;
    }
  }
  // Fallback: closest match
  return CULTURE_TYPES.find((ct) => top2.has(ct.dominantCategories[0])) || CULTURE_TYPES[0];
}

// ─── Helper: Calculate Cultural Fit Score ───
export function calculateCFS(
  userScores: Record<ValueCategory, number>,
  teamScores: Record<ValueCategory, number>
): { score: number; categoryAlignment: number; topOverlap: number; dominanceMatch: number; tensionScore: number } {
  const cats = Object.keys(userScores) as ValueCategory[];

  // Component 1: Category Alignment (40%)
  let diffSum = 0;
  cats.forEach((c) => {
    diffSum += Math.abs((userScores[c] / 100) - (teamScores[c] / 100));
  });
  const categoryAlignment = 1 - (diffSum / (cats.length * 2));

  // Component 2: Top Overlap (30%)
  const userTop3 = cats.sort((a, b) => userScores[b] - userScores[a]).slice(0, 3);
  const teamTop3 = [...cats].sort((a, b) => teamScores[b] - teamScores[a]).slice(0, 3);
  const topOverlap = userTop3.filter((c) => teamTop3.includes(c)).length / 3;

  // Component 3: Dominance Match (15%)
  const dominant = [...cats].sort((a, b) => teamScores[b] - teamScores[a])[0];
  const dominanceMatch = userTop3.includes(dominant) ? 1 : 0;

  // Component 4: Tension Score (15%)
  let totalTension = 0;
  POLARITY_PAIRS.forEach((pair) => {
    const diff = Math.abs(
      (userScores[pair.categoryA] - teamScores[pair.categoryA]) -
      (userScores[pair.categoryB] - teamScores[pair.categoryB])
    );
    totalTension += diff / 100;
  });
  const tensionScore = Math.max(0, 1 - (totalTension / POLARITY_PAIRS.length));

  const score = Math.round(
    (0.4 * categoryAlignment + 0.3 * topOverlap + 0.15 * dominanceMatch + 0.15 * tensionScore) * 100
  );

  return {
    score: Math.min(100, Math.max(0, score)),
    categoryAlignment: Math.round(categoryAlignment * 100),
    topOverlap: Math.round(topOverlap * 100),
    dominanceMatch: dominanceMatch * 100,
    tensionScore: Math.round(tensionScore * 100),
  };
}
