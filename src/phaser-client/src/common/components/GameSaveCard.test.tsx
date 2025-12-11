import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { render, screen } from "../../../test/utils/test-utils";
import { GameSaveCard } from "./GameSaveCard";
import { GameSave } from "../models/GameSave";
import userEvent from "@testing-library/user-event";

describe("GameSaveCard", () => {
    const mockGameSave: GameSave = {
        id: "test-id-123",
        userId: "user-456",
        characterName: "Ash Ketchum",
        dateCreated: "2024-01-15T10:30:00Z",
        lastPlayed: "2024-12-10T15:45:00Z",
        gameSaveData: {
            id: "data-id-789",
            gameSaveId: "test-id-123",
            gameData: {},
        },
    };

    const mockOnSelect = vi.fn();

    beforeEach(() => {
        vi.clearAllMocks();
        // Mock current date to ensure consistent relative time tests
        vi.useFakeTimers();
        vi.setSystemTime(new Date("2024-12-11T15:45:00Z"));
    });

    afterEach(() => {
        vi.useRealTimers();
    });

    describe("rendering", () => {
        it("should render character name", () => {
            render(
                <GameSaveCard gameSave={mockGameSave} onSelect={mockOnSelect} />
            );
            expect(screen.getByText("Ash Ketchum")).toBeInTheDocument();
        });

        it("should render created date", () => {
            render(
                <GameSaveCard gameSave={mockGameSave} onSelect={mockOnSelect} />
            );
            expect(screen.getByText("Created:")).toBeInTheDocument();
            // Date format depends on locale, check for date and time components
            expect(
                screen.getByText(/15.*01.*2024.*10:30|1.*15.*2024.*10:30/)
            ).toBeInTheDocument();
        });

        it("should render last played chip", () => {
            render(
                <GameSaveCard gameSave={mockGameSave} onSelect={mockOnSelect} />
            );
            expect(screen.getByText("Last Played:")).toBeInTheDocument();
            expect(screen.getByText("1 days ago")).toBeInTheDocument();
        });

        it("should truncate long character names", () => {
            const longNameSave = {
                ...mockGameSave,
                characterName: "A".repeat(100),
            };
            render(
                <GameSaveCard gameSave={longNameSave} onSelect={mockOnSelect} />
            );
            const nameElement = screen.getByText("A".repeat(100));
            expect(nameElement).toBeInTheDocument();
        });
    });

    describe("date formatting", () => {
        it("should format relative time for minutes", () => {
            const recentSave = {
                ...mockGameSave,
                lastPlayed: new Date(Date.now() - 30 * 60 * 1000).toISOString(),
            };
            render(
                <GameSaveCard gameSave={recentSave} onSelect={mockOnSelect} />
            );
            expect(screen.getByText("30 minutes ago")).toBeInTheDocument();
        });

        it("should format relative time for hours", () => {
            const hoursSave = {
                ...mockGameSave,
                lastPlayed: new Date(
                    Date.now() - 5 * 60 * 60 * 1000
                ).toISOString(),
            };
            render(
                <GameSaveCard gameSave={hoursSave} onSelect={mockOnSelect} />
            );
            expect(screen.getByText("5 hours ago")).toBeInTheDocument();
        });

        it("should format relative time for days", () => {
            const daysSave = {
                ...mockGameSave,
                lastPlayed: new Date(
                    Date.now() - 3 * 24 * 60 * 60 * 1000
                ).toISOString(),
            };
            render(
                <GameSaveCard gameSave={daysSave} onSelect={mockOnSelect} />
            );
            expect(screen.getByText("3 days ago")).toBeInTheDocument();
        });

        it("should handle less than 1 minute as 0 minutes ago", () => {
            const justNowSave = {
                ...mockGameSave,
                lastPlayed: new Date(Date.now() - 30 * 1000).toISOString(),
            };
            render(
                <GameSaveCard gameSave={justNowSave} onSelect={mockOnSelect} />
            );
            expect(screen.getByText("0 minutes ago")).toBeInTheDocument();
        });
    });

    describe("interactions", () => {
        it("should call onSelect when card is clicked", async () => {
            vi.useRealTimers();
            const user = userEvent.setup();
            render(
                <GameSaveCard gameSave={mockGameSave} onSelect={mockOnSelect} />
            );

            const card = screen.getByText("Ash Ketchum").closest("button");
            expect(card).toBeInTheDocument();

            await user.click(card!);
            expect(mockOnSelect).toHaveBeenCalledTimes(1);
            expect(mockOnSelect).toHaveBeenCalledWith(mockGameSave);
            vi.useFakeTimers();
        });

        it("should handle multiple clicks", async () => {
            vi.useRealTimers();
            const user = userEvent.setup();
            render(
                <GameSaveCard gameSave={mockGameSave} onSelect={mockOnSelect} />
            );

            const card = screen.getByText("Ash Ketchum").closest("button");
            await user.click(card!);
            await user.click(card!);

            expect(mockOnSelect).toHaveBeenCalledTimes(2);
            vi.useFakeTimers();
        });

        it("should be keyboard accessible", async () => {
            vi.useRealTimers();
            const user = userEvent.setup();
            render(
                <GameSaveCard gameSave={mockGameSave} onSelect={mockOnSelect} />
            );

            const card = screen.getByText("Ash Ketchum").closest("button");
            card?.focus();
            await user.keyboard("{Enter}");

            expect(mockOnSelect).toHaveBeenCalledTimes(1);
            vi.useFakeTimers();
        });
    });

    describe("card styling", () => {
        it("should render as a Card component", () => {
            const { container } = render(
                <GameSaveCard gameSave={mockGameSave} onSelect={mockOnSelect} />
            );
            expect(
                container.querySelector(".MuiCard-root")
            ).toBeInTheDocument();
        });

        it("should have CardActionArea for clickable area", () => {
            const { container } = render(
                <GameSaveCard gameSave={mockGameSave} onSelect={mockOnSelect} />
            );
            expect(
                container.querySelector(".MuiCardActionArea-root")
            ).toBeInTheDocument();
        });

        it("should render primary chip for last played", () => {
            const { container } = render(
                <GameSaveCard gameSave={mockGameSave} onSelect={mockOnSelect} />
            );
            const chip = container.querySelector(".MuiChip-root");
            expect(chip).toBeInTheDocument();
            expect(chip).toHaveClass("MuiChip-colorPrimary");
        });
    });

    describe("edge cases", () => {
        it("should handle game save without optional id", () => {
            const saveWithoutId = { ...mockGameSave, id: undefined };
            render(
                <GameSaveCard
                    gameSave={saveWithoutId}
                    onSelect={mockOnSelect}
                />
            );
            expect(screen.getByText("Ash Ketchum")).toBeInTheDocument();
        });

        it("should handle empty character name", () => {
            const emptyNameSave = { ...mockGameSave, characterName: "" };
            render(
                <GameSaveCard
                    gameSave={emptyNameSave}
                    onSelect={mockOnSelect}
                />
            );
            // Should still render the card even with empty name
            expect(screen.getByText("Created:")).toBeInTheDocument();
        });
    });
});
