import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "../../../test/utils/test-utils";
import { GameSaveSelectionPage } from "./GameSaveSelectionPage";
import { GameSave } from "../models/GameSave";
import * as GameSaveContext from "../contexts/GameSaveContext";
import * as AppSettingsContext from "../contexts/AppSettingsContext";
import userEvent from "@testing-library/user-event";

// Mock hooks and contexts
vi.mock("../hooks/useGetAllGameSavesQuery");
vi.mock("../contexts/GameSaveContext");
vi.mock("../contexts/AppSettingsContext");

// Mock child components
vi.mock("../components/GameSaveCard", () => ({
    GameSaveCard: ({ gameSave, onSelect }: any) => (
        <div data-testid={`game-save-card-${gameSave.id}`}>
            <button onClick={() => onSelect(gameSave)}>
                {gameSave.characterName}
            </button>
        </div>
    ),
}));

vi.mock("../components/NewGameForm", () => ({
    NewGameForm: ({ onGameCreated, onCancel }: any) => (
        <div data-testid="new-game-form">
            <button onClick={() => onGameCreated(mockNewGameSave)}>
                Create Game
            </button>
            <button onClick={onCancel}>Cancel</button>
        </div>
    ),
}));

const mockGameSave1: GameSave = {
    id: "save-1",
    userId: "user-1",
    characterName: "Ash",
    dateCreated: "2024-01-01T00:00:00Z",
    lastPlayed: "2024-12-01T00:00:00Z",
    gameSaveData: {
        id: "data-1",
        gameSaveId: "save-1",
        gameData: {},
    },
};

const mockGameSave2: GameSave = {
    id: "save-2",
    userId: "user-1",
    characterName: "Misty",
    dateCreated: "2024-01-02T00:00:00Z",
    lastPlayed: "2024-12-02T00:00:00Z",
    gameSaveData: {
        id: "data-2",
        gameSaveId: "save-2",
        gameData: {},
    },
};

const mockNewGameSave: GameSave = {
    id: "save-new",
    userId: "user-1",
    characterName: "Brock",
    dateCreated: "2024-12-11T00:00:00Z",
    lastPlayed: "2024-12-11T00:00:00Z",
    gameSaveData: {
        id: "data-new",
        gameSaveId: "save-new",
        gameData: {},
    },
};

describe("GameSaveSelectionPage", () => {
    const mockSetCurrentGameSave = vi.fn();
    const mockRefetch = vi.fn();
    const mockOnGameSaveSelected = vi.fn();

    beforeEach(async () => {
        vi.clearAllMocks();

        vi.mocked(GameSaveContext.useGameSaveContext).mockReturnValue({
            currentGameSave: null,
            setCurrentGameSave: mockSetCurrentGameSave,
        });

        vi.mocked(AppSettingsContext.useGetAppSettingsContext).mockReturnValue({
            serviceName: "PokeGame Test",
            releaseVersion: "1.0.0",
            coreApiUrl: "http://localhost:5000",
            signalRUrl: "http://localhost:5000/hub",
        });

        const { useGetAllGameSavesQuery } = await import(
            "../hooks/useGetAllGameSavesQuery"
        );
        vi.mocked(useGetAllGameSavesQuery).mockReturnValue({
            data: [mockGameSave1, mockGameSave2],
            isLoading: false,
            error: null,
            refetch: mockRefetch,
        } as any);
    });

    describe("loading state", () => {
        it("should display loading component when loading", async () => {
            const { useGetAllGameSavesQuery } = await import(
                "../hooks/useGetAllGameSavesQuery"
            );
            vi.mocked(useGetAllGameSavesQuery).mockReturnValue({
                data: undefined,
                isLoading: true,
                error: null,
                refetch: mockRefetch,
            } as any);

            render(<GameSaveSelectionPage />);

            expect(
                screen.getByText("Loading game saves...")
            ).toBeInTheDocument();
        });
    });

    describe("error state", () => {
        it("should display error message when query fails", async () => {
            const { useGetAllGameSavesQuery } = await import(
                "../hooks/useGetAllGameSavesQuery"
            );
            const mockError = new Error("Failed to load game saves");
            vi.mocked(useGetAllGameSavesQuery).mockReturnValue({
                data: undefined,
                isLoading: false,
                error: mockError,
                refetch: mockRefetch,
            } as any);

            render(<GameSaveSelectionPage />);

            expect(
                screen.getByText(/Error loading game saves/)
            ).toBeInTheDocument();
            expect(
                screen.getByText(/Failed to load game saves/)
            ).toBeInTheDocument();
        });

        it("should call refetch when Try Again button is clicked", async () => {
            const user = userEvent.setup();
            const { useGetAllGameSavesQuery } = await import(
                "../hooks/useGetAllGameSavesQuery"
            );
            vi.mocked(useGetAllGameSavesQuery).mockReturnValue({
                data: undefined,
                isLoading: false,
                error: new Error("Network error"),
                refetch: mockRefetch,
            } as any);

            render(<GameSaveSelectionPage />);

            const retryButton = screen.getByTestId("retry-button");
            await user.click(retryButton);

            expect(mockRefetch).toHaveBeenCalledTimes(1);
        });
    });

    describe("rendering with game saves", () => {
        it("should render page header", () => {
            render(<GameSaveSelectionPage />);

            expect(screen.getByText("Select Game Save")).toBeInTheDocument();
            expect(
                screen.getByText(
                    /Choose an existing game save or create a new one/
                )
            ).toBeInTheDocument();
        });

        it("should render create new game button", () => {
            render(<GameSaveSelectionPage />);

            expect(
                screen.getByTestId("create-new-game-button")
            ).toBeInTheDocument();
            expect(screen.getByText("+ Create New Game")).toBeInTheDocument();
        });

        it("should display existing game saves count", () => {
            render(<GameSaveSelectionPage />);

            expect(
                screen.getByText("Existing Game Saves (2)")
            ).toBeInTheDocument();
        });

        it("should render all game save cards", () => {
            render(<GameSaveSelectionPage />);

            expect(
                screen.getByTestId("game-save-card-save-1")
            ).toBeInTheDocument();
            expect(
                screen.getByTestId("game-save-card-save-2")
            ).toBeInTheDocument();
            expect(screen.getByText("Ash")).toBeInTheDocument();
            expect(screen.getByText("Misty")).toBeInTheDocument();
        });

        it("should render game saves in a grid", () => {
            render(<GameSaveSelectionPage />);

            const grid = screen.getByTestId("game-saves-grid");
            expect(grid).toBeInTheDocument();
        });
    });

    describe("no game saves", () => {
        it("should display no game saves message when list is empty", async () => {
            const { useGetAllGameSavesQuery } = await import(
                "../hooks/useGetAllGameSavesQuery"
            );
            vi.mocked(useGetAllGameSavesQuery).mockReturnValue({
                data: [],
                isLoading: false,
                error: null,
                refetch: mockRefetch,
            } as any);

            render(<GameSaveSelectionPage />);

            expect(
                screen.getByTestId("no-game-saves-message")
            ).toBeInTheDocument();
            expect(screen.getByText("No game saves found")).toBeInTheDocument();
            expect(
                screen.getByText(
                    /Create your first game save to start your Pokémon adventure/
                )
            ).toBeInTheDocument();
        });
    });

    describe("game save selection", () => {
        it("should call setCurrentGameSave when a game save is selected", async () => {
            const user = userEvent.setup();
            render(<GameSaveSelectionPage />);

            const ashButton = screen.getByText("Ash");
            await user.click(ashButton);

            expect(mockSetCurrentGameSave).toHaveBeenCalledWith(mockGameSave1);
        });

        it("should call onGameSaveSelected callback when provided", async () => {
            const user = userEvent.setup();
            render(
                <GameSaveSelectionPage
                    onGameSaveSelected={mockOnGameSaveSelected}
                />
            );

            const mistyButton = screen.getByText("Misty");
            await user.click(mistyButton);

            expect(mockOnGameSaveSelected).toHaveBeenCalledWith(mockGameSave2);
        });

        it("should work without onGameSaveSelected callback", async () => {
            const user = userEvent.setup();
            render(<GameSaveSelectionPage />);

            const ashButton = screen.getByText("Ash");
            await user.click(ashButton);

            expect(mockSetCurrentGameSave).toHaveBeenCalled();
        });
    });

    describe("new game form", () => {
        it("should show new game form when create button is clicked", async () => {
            const user = userEvent.setup();
            render(<GameSaveSelectionPage />);

            expect(
                screen.queryByTestId("new-game-form")
            ).not.toBeInTheDocument();

            const createButton = screen.getByTestId("create-new-game-button");
            await user.click(createButton);

            expect(screen.getByTestId("new-game-form")).toBeInTheDocument();
        });

        it("should hide create button when form is shown", async () => {
            const user = userEvent.setup();
            render(<GameSaveSelectionPage />);

            const createButton = screen.getByTestId("create-new-game-button");
            await user.click(createButton);

            expect(
                screen.queryByTestId("create-new-game-button")
            ).not.toBeInTheDocument();
        });

        it("should hide form when cancel is clicked", async () => {
            const user = userEvent.setup();
            render(<GameSaveSelectionPage />);

            const createButton = screen.getByTestId("create-new-game-button");
            await user.click(createButton);

            const cancelButton = screen.getByText("Cancel");
            await user.click(cancelButton);

            expect(
                screen.queryByTestId("new-game-form")
            ).not.toBeInTheDocument();
            expect(
                screen.getByTestId("create-new-game-button")
            ).toBeInTheDocument();
        });

        it("should handle new game creation", async () => {
            const user = userEvent.setup();
            render(
                <GameSaveSelectionPage
                    onGameSaveSelected={mockOnGameSaveSelected}
                />
            );

            const createButton = screen.getByTestId("create-new-game-button");
            await user.click(createButton);

            const createGameButton = screen.getByText("Create Game");
            await user.click(createGameButton);

            await waitFor(() => {
                expect(mockRefetch).toHaveBeenCalled();
            });
        });

        it("should auto-select newly created game", async () => {
            const user = userEvent.setup();
            render(<GameSaveSelectionPage />);

            const createButton = screen.getByTestId("create-new-game-button");
            await user.click(createButton);

            const createGameButton = screen.getByText("Create Game");
            await user.click(createGameButton);

            await waitFor(() => {
                expect(mockSetCurrentGameSave).toHaveBeenCalledWith(
                    mockNewGameSave
                );
            });
        });

        it("should hide form after successful creation", async () => {
            const user = userEvent.setup();
            render(<GameSaveSelectionPage />);

            const createButton = screen.getByTestId("create-new-game-button");
            await user.click(createButton);

            const createGameButton = screen.getByText("Create Game");
            await user.click(createGameButton);

            await waitFor(() => {
                expect(
                    screen.queryByTestId("new-game-form")
                ).not.toBeInTheDocument();
            });
        });
    });

    describe("integration", () => {
        it("should handle complete flow: show form, create game, select it", async () => {
            const user = userEvent.setup();
            render(
                <GameSaveSelectionPage
                    onGameSaveSelected={mockOnGameSaveSelected}
                />
            );

            // Initially shows game saves
            expect(screen.getByText("Ash")).toBeInTheDocument();

            // Click create new game
            await user.click(screen.getByTestId("create-new-game-button"));
            expect(screen.getByTestId("new-game-form")).toBeInTheDocument();

            // Create game
            await user.click(screen.getByText("Create Game"));

            // Verify refetch and auto-select
            await waitFor(() => {
                expect(mockRefetch).toHaveBeenCalled();
                expect(mockSetCurrentGameSave).toHaveBeenCalledWith(
                    mockNewGameSave
                );
                expect(mockOnGameSaveSelected).toHaveBeenCalledWith(
                    mockNewGameSave
                );
            });
        });

        it("should not show no saves message when form is open", async () => {
            const user = userEvent.setup();
            const { useGetAllGameSavesQuery } = await import(
                "../hooks/useGetAllGameSavesQuery"
            );
            vi.mocked(useGetAllGameSavesQuery).mockReturnValue({
                data: [],
                isLoading: false,
                error: null,
                refetch: mockRefetch,
            } as any);

            render(<GameSaveSelectionPage />);

            const createButton = screen.getByTestId("create-new-game-button");
            await user.click(createButton);

            expect(
                screen.queryByTestId("no-game-saves-message")
            ).not.toBeInTheDocument();
        });
    });
});
