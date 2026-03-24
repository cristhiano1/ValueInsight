import { motion } from "framer-motion";
import { useLanguage } from "@/context/LanguageContext";

interface FitGaugeProps {
  score: number;
}

export default function FitGauge({ score }: FitGaugeProps) {
  const { t } = useLanguage();
  const clampedScore = Math.min(100, Math.max(0, score));
  
  const radius = 70;
  const strokeWidth = 10;
  const cx = 80;
  const cy = 80;
  
  const circumference = Math.PI * radius;
  const dashOffset = circumference - (clampedScore / 100) * circumference;
  
  let strokeColor = "hsl(38, 92%, 50%)"; // warning
  if (clampedScore >= 80) strokeColor = "hsl(142, 71%, 45%)"; // success
  else if (clampedScore >= 40) strokeColor = "hsl(217, 91%, 50%)"; // primary blue

  return (
    <div className="relative flex flex-col items-center">
      <svg width="160" height="90" viewBox="0 0 160 90">
        <path
          d={`M ${cx - radius} ${cy} A ${radius} ${radius} 0 0 1 ${cx + radius} ${cy}`}
          fill="none"
          stroke="hsl(215, 18%, 22%)"
          strokeWidth={strokeWidth}
          strokeLinecap="round"
        />
        <motion.path
          d={`M ${cx - radius} ${cy} A ${radius} ${radius} 0 0 1 ${cx + radius} ${cy}`}
          fill="none"
          stroke={strokeColor}
          strokeWidth={strokeWidth}
          strokeLinecap="round"
          strokeDasharray={circumference}
          initial={{ strokeDashoffset: circumference }}
          animate={{ strokeDashoffset: dashOffset }}
          transition={{ duration: 1, ease: [0.16, 1, 0.3, 1], delay: 0.3 }}
        />
      </svg>
      <div className="absolute bottom-0 flex flex-col items-center">
        <span className="font-mono text-display font-semibold text-foreground leading-none">
          {clampedScore}
        </span>
        <span className="text-xs text-muted-foreground mt-1">{t("outOf100")}</span>
      </div>
    </div>
  );
}
