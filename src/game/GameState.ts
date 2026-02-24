import type { Bee } from "../types/Bee";

export type CellType = "empty" | "flower" | "hive";

export type Cell = {
  id: string;
  x: number;
  y: number;
  type: CellType;
  stage: number;
};

export type GameState = {
  cells: Cell[];
  bees: Bee[];
  honey: number;
};
