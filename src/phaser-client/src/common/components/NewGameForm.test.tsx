import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "../../../test/utils/test-utils";
import { NewGameForm } from "./NewGameForm";
import userEvent from "@testing-library/user-event";
import { GameSave } from "../models/GameSave";

// Mock the mutation hook
vi.mock("../hooks/useSaveNewGameMutation");

const mockGameSave: GameSave = {
    id: "new-save-123",
    userId: "user-456",
    characterName: "New Character",
    dateCreated: "2024-12-11T00:00:00Z",
    lastPlayed: "2024-12-11T00:00:00Z",
    gameSaveData: {
        id: "data-789",
        gameSaveId: "new-save-123",
        gameData: {},
    },
};

describe("NewGameForm", () => {
    const mockOnGameCreated = vi.fn();
    const mockOnCancel = vi.fn();
    const mockMutate = vi.fn();

    beforeEach(async () => {
        vi.clearAllMocks();

        // Default mock implementation
        const { useSaveNewGameMutation } = vi.mocked(
            await import("../hooks/useSaveNewGameMutation")
        );
        useSaveNewGameMutation.mockReturnValue({
            data: undefined,
            error: null,
            isPending: false,
            mutate: mockMutate,
        } as any);
    });

    describe("rendering", () => {
        it("should render form title", () => {
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );
            expect(screen.getByText("Create New Game")).toBeInTheDocument();
        });

        it("should render character name input", () => {
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );
            expect(
                screen.getByPlaceholderText("Enter your character name")
            ).toBeInTheDocument();
        });

        it("should render cancel button", () => {
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );
            expect(
                screen.getByRole("button", { name: "Cancel" })
            ).toBeInTheDocument();
        });

        it("should render create button", () => {
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );
            expect(
                screen.getByRole("button", { name: "Create Game" })
            ).toBeInTheDocument();
        });

        it("should show character count helper text", () => {
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );
            expect(screen.getByText("0/50 characters")).toBeInTheDocument();
        });
    });

    describe("form interactions", () => {
        it("should update character name on input", async () => {
            const user = userEvent.setup();
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            const input = screen.getByPlaceholderText(
                "Enter your character name"
            );
            await user.type(input, "Ash");

            expect(input).toHaveValue("Ash");
            expect(screen.getByText("3/50 characters")).toBeInTheDocument();
        });

        it("should call onCancel when cancel button is clicked", async () => {
            const user = userEvent.setup();
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            await user.click(screen.getByRole("button", { name: "Cancel" }));
            expect(mockOnCancel).toHaveBeenCalledTimes(1);
        });

        it("should disable submit button when character name is empty", () => {
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            const submitButton = screen.getByRole("button", {
                name: "Create Game",
            });
            expect(submitButton).toBeDisabled();
        });

        it("should enable submit button when character name is provided", async () => {
            const user = userEvent.setup();
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            const input = screen.getByPlaceholderText(
                "Enter your character name"
            );
            await user.type(input, "Ash");

            const submitButton = screen.getByRole("button", {
                name: "Create Game",
            });
            expect(submitButton).not.toBeDisabled();
        });

        it("should disable submit button for whitespace-only names", async () => {
            const user = userEvent.setup();
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            const input = screen.getByPlaceholderText(
                "Enter your character name"
            );
            await user.type(input, "   ");

            const submitButton = screen.getByRole("button", {
                name: "Create Game",
            });
            expect(submitButton).toBeDisabled();
        });
    });

    describe("form submission", () => {
        it("should call mutate with trimmed character name on submit", async () => {
            const user = userEvent.setup();
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            const input = screen.getByPlaceholderText(
                "Enter your character name"
            );
            await user.type(input, "  Ash Ketchum  ");

            const submitButton = screen.getByRole("button", {
                name: "Create Game",
            });
            await user.click(submitButton);

            expect(mockMutate).toHaveBeenCalledWith({
                input: { newCharacterName: "Ash Ketchum" },
            });
        });

        it("should not submit empty form", async () => {
            const user = userEvent.setup();
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            const form = screen
                .getByPlaceholderText("Enter your character name")
                .closest("form");
            if (form) {
                // Try to submit by pressing enter
                const input = screen.getByPlaceholderText(
                    "Enter your character name"
                );
                await user.type(input, "{Enter}");
            }

            expect(mockMutate).not.toHaveBeenCalled();
        });

        it("should call onGameCreated when mutation succeeds", async () => {
            const { useSaveNewGameMutation } = await import(
                "../hooks/useSaveNewGameMutation"
            );
            vi.mocked(useSaveNewGameMutation).mockReturnValue({
                data: mockGameSave,
                error: null,
                isPending: false,
                mutate: mockMutate,
            } as any);

            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            await waitFor(() => {
                expect(mockOnGameCreated).toHaveBeenCalledWith(mockGameSave);
            });
        });
    });

    describe("character limit", () => {
        it("should enforce 50 character maximum", async () => {
            const user = userEvent.setup();
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            const input = screen.getByRole("textbox") as HTMLInputElement;
            expect(input.maxLength).toBe(50);
        });

        it("should update character count as user types", async () => {
            const user = userEvent.setup();
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            const input = screen.getByPlaceholderText(
                "Enter your character name"
            );
            await user.type(input, "Ash Ketchum");

            expect(screen.getByText("11/50 characters")).toBeInTheDocument();
        });

        it("should handle maximum character input", async () => {
            const user = userEvent.setup();
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            const maxLengthName = "A".repeat(50);
            const input = screen.getByPlaceholderText(
                "Enter your character name"
            );
            await user.type(input, maxLengthName);

            expect(screen.getByText("50/50 characters")).toBeInTheDocument();
        });
    });

    describe("loading state", () => {
        it("should disable inputs during submission", async () => {
            const { useSaveNewGameMutation } = vi.mocked(
                await import("../hooks/useSaveNewGameMutation")
            );
            useSaveNewGameMutation.mockReturnValue({
                data: undefined,
                error: null,
                isPending: true,
                mutate: mockMutate,
            } as any);

            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            expect(
                screen.getByPlaceholderText("Enter your character name")
            ).toBeDisabled();
            expect(
                screen.getByRole("button", { name: "Cancel" })
            ).toBeDisabled();
        });

        it("should show loading text on submit button", async () => {
            const { useSaveNewGameMutation } = vi.mocked(
                await import("../hooks/useSaveNewGameMutation")
            );
            useSaveNewGameMutation.mockReturnValue({
                data: undefined,
                error: null,
                isPending: true,
                mutate: mockMutate,
            } as any);

            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            expect(
                screen.getByRole("button", { name: /Creating/i })
            ).toBeInTheDocument();
        });

        it("should show loading spinner during submission", async () => {
            const { useSaveNewGameMutation } = vi.mocked(
                await import("../hooks/useSaveNewGameMutation")
            );
            useSaveNewGameMutation.mockReturnValue({
                data: undefined,
                error: null,
                isPending: true,
                mutate: mockMutate,
            } as any);

            const { container } = render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            const spinner = container.querySelector(
                ".MuiCircularProgress-root"
            );
            expect(spinner).toBeInTheDocument();
        });
    });

    describe("error handling", () => {
        it("should display error when mutation fails", async () => {
            const { useSaveNewGameMutation } = vi.mocked(
                await import("../hooks/useSaveNewGameMutation")
            );
            const mockError = new Error("Failed to create game");
            useSaveNewGameMutation.mockReturnValue({
                data: undefined,
                error: mockError,
                isPending: false,
                mutate: mockMutate,
            } as any);

            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            expect(
                screen.getByText("Failed to create game")
            ).toBeInTheDocument();
        });

        it("should render ErrorComponent with error", async () => {
            const { useSaveNewGameMutation } = vi.mocked(
                await import("../hooks/useSaveNewGameMutation")
            );
            useSaveNewGameMutation.mockReturnValue({
                data: undefined,
                error: new Error("Test error"),
                isPending: false,
                mutate: mockMutate,
            } as any);

            const { container } = render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            expect(
                container.querySelector(".MuiAlert-root")
            ).toBeInTheDocument();
        });
    });

    describe("form styling", () => {
        it("should render as a Paper component", () => {
            const { container } = render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );
            expect(
                container.querySelector(".MuiPaper-root")
            ).toBeInTheDocument();
        });

        it("should have primary border color", () => {
            const { container } = render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );
            const paper = container.querySelector(".MuiPaper-root");
            expect(paper).toBeInTheDocument();
        });
    });

    describe("accessibility", () => {
        it("should have proper label association", () => {
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            const input = screen.getByRole("textbox");
            expect(input).toHaveAttribute("id", "characterName");
        });

        it("should mark character name as required", () => {
            render(
                <NewGameForm
                    onGameCreated={mockOnGameCreated}
                    onCancel={mockOnCancel}
                />
            );

            const input = screen.getByRole("textbox");
            expect(input).toBeRequired();
        });
    });
});
