import { screen, waitFor } from "../test/utils/test-utils";
import { describe, expect, it } from "vitest";
import { App } from "./App";
import { render } from "@testing-library/react";

describe(App.name, () => {
    it("Simple top level render test", async () => {
        render(<App />);

        await waitFor(() => {
            expect(screen.getByText("PokeGame")).toBeInTheDocument();
        });
    });
});
