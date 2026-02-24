import { create } from "zustand";
import type { GameState, Cell } from "../game/GameState";

const GRID_SIZE = 10;
const CELL_SIZE = 48;

function initCells(): Cell[] {
  const cells: Cell[] = [];
  for (let y = 0; y < GRID_SIZE; y++) {
    for (let x = 0; x < GRID_SIZE; x++) {
      cells.push({ id: `${x}-${y}`, x, y, type: "empty", stage: 0 });
    }
  }
  return cells;
}

export const useGameStore = create<GameState>(() => ({
  cells: initCells(),
  bees: [
    {
      id: "bee-1",
      job: "idle",
      gridX: 2,
      gridY: 2,
      visualX: 2 * CELL_SIZE,
      visualY: 2 * CELL_SIZE,
      speed: 4,
    },
    {
      id: "bee-2",
      job: "idle",
      gridX: 3,
      gridY: 3,
      visualX: 3 * CELL_SIZE,
      visualY: 3 * CELL_SIZE,
      speed: 4,
    },
  ],
  honey: 0,
}));
