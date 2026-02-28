import { GameCanvas } from "./ui/GameCanvas";
import { addBee } from "./systems/beeSystem";

export default function App() {
  return (
    <div style={{ display: "flex", gap: "1rem", padding: "1rem" }}>
      <GameCanvas/>
      <div style={{ color: "white" }}>UI goes here</div>
      <div style={{ display: "flex", flexDirection: "column", gap: "1rem" }}>
        <button
          onClick={addBee}
          style={{
            backgroundColor: "#f5c542",
            borderRadius: "8px",
            border: "none",
          }}
        >
          Add Bee
        </button>
      </div>
    </div>
  );
}
