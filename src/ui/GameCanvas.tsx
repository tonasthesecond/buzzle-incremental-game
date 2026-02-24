import { useEffect, useRef } from "react";
import { game } from "../game/Game";

export function GameCanvas() {
  const ref = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    if (ref.current) game.init(ref.current);
  }, []);

  return (
    <canvas ref={ref} width={480} height={480} style={{ display: "block" }} />
  );
}
