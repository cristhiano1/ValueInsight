import { useState } from "react";
import { motion } from "framer-motion";
import { GripVertical, ArrowRight, ArrowLeft, Star } from "lucide-react";
import { useApp } from "@/context/AppContext";
import { useLanguage } from "@/context/LanguageContext";
import { useTranslatedData } from "@/hooks/useTranslatedData";
import { useNavigate } from "react-router-dom";
import { Value } from "@/data/mockData";
import {
  DndContext, closestCenter, PointerSensor, useSensor, useSensors, DragEndEvent,
} from "@dnd-kit/core";
import {
  arrayMove, SortableContext, useSortable, verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";

function SortableItem({ value, index, translatedName, translatedDesc, catLabel, catColor }: {
  value: Value; index: number; translatedName: string; translatedDesc: string; catLabel?: string; catColor?: string;
}) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({ id: value.id });
  const style = { transform: CSS.Transform.toString(transform), transition, zIndex: isDragging ? 10 : 0 };

  return (
    <motion.div
      ref={setNodeRef}
      style={style}
      initial={{ opacity: 0, x: -20 }}
      animate={{ opacity: 1, x: 0, scale: isDragging ? 1.02 : 1 }}
      transition={{ delay: index * 0.05 }}
      className={`flex items-center gap-4 p-4 rounded-card border-2 bg-card transition-shadow ${
        isDragging ? "shadow-surface-lg border-primary" : "border-border shadow-surface"
      } ${index < 3 ? "ring-2 ring-primary/20" : ""}`}
    >
      <button
        {...attributes}
        {...listeners}
        className="cursor-grab active:cursor-grabbing p-1 text-muted-foreground hover:text-foreground transition-colors"
      >
        <GripVertical className="w-4 h-4" />
      </button>
      <div
        className="w-8 h-8 rounded-inner flex items-center justify-center font-mono text-ui-sm font-semibold"
        style={{ backgroundColor: `${catColor}15`, color: catColor }}
      >
        {index + 1}
      </div>
      <div className="flex-1">
        <h3 className="text-ui-sm font-semibold text-foreground">{translatedName}</h3>
        <p className="text-xs text-muted-foreground line-clamp-1">{translatedDesc}</p>
      </div>
      {index < 3 && <Star className="w-4 h-4 text-warning fill-warning" />}
    </motion.div>
  );
}

export default function PrioritizationPage() {
  const { narrowedValues, setRankedValues } = useApp();
  const navigate = useNavigate();
  const { t } = useLanguage();
  const { values: ALL_VALUES, categories: CATEGORIES } = useTranslatedData();
  const [items, setItems] = useState<Value[]>(narrowedValues.length > 0 ? narrowedValues : []);

  const sensors = useSensors(useSensor(PointerSensor, { activationConstraint: { distance: 5 } }));

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;
    if (over && active.id !== over.id) {
      setItems((prev) => {
        const oldIndex = prev.findIndex((v) => v.id === active.id);
        const newIndex = prev.findIndex((v) => v.id === over.id);
        return arrayMove(prev, oldIndex, newIndex);
      });
    }
  };

  const getTranslated = (v: Value) => ALL_VALUES.find((tv) => tv.id === v.id) ?? v;
  const top3 = items.slice(0, 3);

  return (
    <div className="workspace-narrow">
      <div className="mb-8">
        <h1 className="text-subhead font-semibold text-foreground">{t("rankTitle")}</h1>
        <p className="text-ui-sm text-muted-foreground mt-1">{t("rankStep")}</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-[1fr_260px] gap-8">
        <div>
          <div className="text-xs font-medium text-muted-foreground uppercase tracking-wider mb-3">
            {t("yourRanking")}
          </div>
          <DndContext sensors={sensors} collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
            <SortableContext items={items.map((v) => v.id)} strategy={verticalListSortingStrategy}>
              <div className="space-y-2">
                {items.map((value, index) => {
                  const tv = getTranslated(value);
                  const cat = CATEGORIES.find((c) => c.id === value.category);
                  return (
                    <SortableItem
                      key={value.id}
                      value={value}
                      index={index}
                      translatedName={tv.name}
                      translatedDesc={tv.description}
                      catLabel={cat?.label}
                      catColor={cat?.color}
                    />
                  );
                })}
              </div>
            </SortableContext>
          </DndContext>
        </div>

        <div className="lg:sticky lg:top-20 lg:self-start">
          <div className="card-surface p-5">
            <h3 className="text-ui-sm font-semibold text-foreground mb-4">{t("yourTop3")}</h3>
            <div className="space-y-2">
              {[0, 1, 2].map((i) => {
                const tv = top3[i] ? getTranslated(top3[i]) : null;
                return (
                  <div
                    key={i}
                    className={`flex items-center gap-2 p-2.5 rounded-inner text-ui-sm ${
                      tv ? "bg-primary/5 text-foreground font-medium" : "bg-secondary text-muted-foreground"
                    }`}
                  >
                    <Star className={`w-3.5 h-3.5 ${tv ? "text-warning fill-warning" : "text-muted-foreground/30"}`} />
                    <span className="font-mono text-xs text-primary w-5">{i + 1}.</span>
                    {tv?.name || "—"}
                  </div>
                );
              })}
            </div>
          </div>
        </div>
      </div>

      <div className="flex items-center justify-between mt-8">
        <button
          onClick={() => navigate("/narrow")}
          className="flex items-center gap-2 text-ui-sm font-medium text-muted-foreground hover:text-foreground transition-colors press-effect"
        >
          <ArrowLeft className="w-4 h-4" />
          {t("back")}
        </button>
        <button
          onClick={() => {
            setRankedValues(items.slice(0, 3));
            navigate("/concretize");
          }}
          disabled={items.length < 3}
          className="flex items-center gap-2 h-10 px-5 bg-primary text-primary-foreground rounded-inner text-ui-sm font-medium hover:opacity-90 transition-opacity press-effect disabled:opacity-40"
        >
          {t("defineValues")}
          <ArrowRight className="w-4 h-4" />
        </button>
      </div>
    </div>
  );
}
