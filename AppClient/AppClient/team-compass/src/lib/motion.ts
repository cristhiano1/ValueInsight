export const EASE = [0.2, 0, 0, 1] as [number, number, number, number];

export const fadeInUp = {
  initial: { opacity: 0, y: 20 },
  animate: { opacity: 1, y: 0 },
  transition: { duration: 0.4, ease: EASE },
};

export const fadeInUpDelay = (delay: number) => ({
  ...fadeInUp,
  transition: { ...fadeInUp.transition, delay },
});
