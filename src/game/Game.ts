import { GridRenderer } from "./renderer/GridRenderer";
import { useGameStore } from "../store/gameStore";
import { tickBees, updateBeeVisuals } from "../systems/beeSystem";

const TICK_RATE = 250;

export class Game {
  private renderer!: GridRenderer;
  private lastTick = 0;

  init(canvas: HTMLCanvasElement) {
    this.renderer = new GridRenderer(canvas);
    requestAnimationFrame(this.loop.bind(this));
  }

  private loop(timestamp: number) {
    if (timestamp - this.lastTick >= TICK_RATE) {
      this.tick();
      this.lastTick = timestamp;
    }

    updateBeeVisuals();

    const { cells, bees } = useGameStore.getState();
    this.renderer.render(cells, bees);
    requestAnimationFrame(this.loop.bind(this));
  }

  private tick() {
    tickBees();
  }
}

export const game = new Game();
