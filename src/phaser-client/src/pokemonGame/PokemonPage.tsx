import { PokemonPhaserGame } from "./PokemonPhaserGame";

export default function PokemonPage() {
    return (
        <div style={{ width: "100%", height: "100vh", display: "flex", flexDirection: "column", alignItems: "center", justifyContent: "center" }}>
            <div style={{ marginBottom: 8 }}>
                <a className="button" href="/">Back to default game</a>
            </div>
            <PokemonPhaserGame />
        </div>
    );
}
