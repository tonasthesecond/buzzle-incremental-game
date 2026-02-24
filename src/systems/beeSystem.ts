import { useGameStore } from "../store/gameStore";
import { GRID_SIZE, CELL_SIZE } from "../game/config";

export function tickBees() {
  const { bees } = useGameStore.getState();

  const updated = bees.map((bee) => {
    if (bee.targetGridX !== undefined) return bee;

    const targetGridX = Math.floor(Math.random() * GRID_SIZE);
    const targetGridY = Math.floor(Math.random() * GRID_SIZE);
    return { ...bee, targetGridX, targetGridY };
  });

  useGameStore.setState({ bees: updated });
}

export function updateBeeVisuals() {
  const { bees } = useGameStore.getState();

  const updated = bees.map((bee) => {
    if (bee.targetGridX === undefined) return bee;

    const targetVisualX = bee.targetGridX * CELL_SIZE;
    const targetVisualY = bee.targetGridY! * CELL_SIZE;

    const dx = targetVisualX - bee.visualX;
    const dy = targetVisualY - bee.visualY;
    const dist = Math.sqrt(dx * dx + dy * dy);

    if (dist < bee.speed) {
      return {
        ...bee,
        visualX: targetVisualX,
        visualY: targetVisualY,
        targetGridX: undefined,
        targetGridY: undefined,
      };
    }

    return {
      ...bee,
      visualX: bee.visualX + (dx / dist) * bee.speed,
      visualY: bee.visualY + (dy / dist) * bee.speed,
    };
  });

  useGameStore.setState({ bees: updated });
}
