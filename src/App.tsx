import { GameCanvas } from "./ui/GameCanvas";

export default function App() {
  return (
    <div style={{ display: "flex", gap: "1rem", padding: "1rem" }}>
      <GameCanvas />
      <div style={{ color: "white" }}>UI goes here</div>
    </div>
  );
}
