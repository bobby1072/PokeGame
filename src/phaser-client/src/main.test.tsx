import { render, screen, waitFor } from "../test/utils/test-utils";
import { describe, expect, it } from 'vitest';
import { App } from './App';

describe("Main", () => {
    it("Simple top level render test", async () => {
        render(<App />);

        await waitFor(() => {
            expect(screen.getByTitle("PokeGame")).toBeInTheDocument();
        });
    });
});