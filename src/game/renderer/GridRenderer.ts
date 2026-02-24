import type { Cell } from "../GameState";
import type { Bee } from "../../types/Bee";

const CELL_SIZE = 48;

export class GridRenderer {
  private ctx: CanvasRenderingContext2D;
  private canvas!: HTMLCanvasElement;

  constructor(canvas: HTMLCanvasElement) {
    this.canvas = canvas;
    this.ctx = canvas.getContext("2d")!;
  }

  render(cells: Cell[], bees: Bee[]) {
    this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
    this.drawCells(cells);
    this.drawBees(bees);
  }

  private drawCells(cells: Cell[]) {
    for (const cell of cells) {
      this.ctx.fillStyle =
        cell.type === "hive"
          ? "#c8a45a"
          : cell.type === "flower"
            ? `hsl(120, 60%, ${20 + cell.stage * 15}%)`
            : "#2a2a2a";
      this.ctx.fillRect(
        cell.x * CELL_SIZE,
        cell.y * CELL_SIZE,
        CELL_SIZE - 1,
        CELL_SIZE - 1,
      );
    }
  }

  private drawBees(bees: Bee[]) {
    for (const bee of bees) {
      this.ctx.fillStyle = "#f5c542";
      this.ctx.beginPath();
      this.ctx.arc(
        bee.visualX + CELL_SIZE / 2,
        bee.visualY + CELL_SIZE / 2,
        CELL_SIZE * 0.1,
        0,
        Math.PI * 2,
      );
      this.ctx.fill();
    }
  }
}
