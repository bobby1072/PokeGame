import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen } from "../../../test/utils/test-utils";
import { GameSelectionWrapper } from "./GameSelectionWrapper";
import { GameSave } from "../models/GameSave";
import * as GameSaveContext from "../contexts/GameSaveContext";
import * as PokeGameUserContext from "../contexts/PokeGameUserContext";
import * as PokeGameCoreHttpClientContext from "../contexts/PokeGameCoreHttpClientContext";

// Mock the contexts
vi.mock("../contexts/GameSaveContext");
vi.mock("../contexts/PokeGameUserContext");
vi.mock("../contexts/PokeGameCoreHttpClientContext");
vi.mock("../pages/GameSaveSelectionPage", () => ({
    GameSaveSelectionPage: ({ onGameSaveSelected }: any) => (
        <div data-testid="game-save-selection-page">
            <button onClick={() => onGameSaveSelected(mockGameSave)}>
                Select Game
            </button>
        </div>
    ),
}));

const mockGameSave: GameSave = {
    id: "test-save-123",
    userId: "user-456",
    characterName: "Test Character",
    dateCreated: "2024-01-01T00:00:00Z",
    lastPlayed: "2024-12-10T00:00:00Z",
    gameSaveData: {
        id: "data-789",
        gameSaveId: "test-save-123",
        gameData: {},
    },
};

const mockHttpClient = {
    setUserId: vi.fn(),
};

const mockUser = {
    id: "user-456",
    username: "testuser",
    email: "test@example.com",
};

describe("GameSelectionWrapper", () => {
    let mockSetCurrentGameSave: ReturnType<typeof vi.fn>;

    beforeEach(() => {
        vi.clearAllMocks();
        mockSetCurrentGameSave = vi.fn();

        vi.mocked(
            PokeGameUserContext.useGetPokeGameUserContext
        ).mockReturnValue(mockUser);
        vi.mocked(
            PokeGameCoreHttpClientContext.useGetPokeGameHttpClientContext
        ).mockReturnValue(mockHttpClient as any);
    });

    describe("when no game save is selected", () => {
        beforeEach(() => {
            vi.mocked(GameSaveContext.useGameSaveContext).mockReturnValue({
                currentGameSave: null,
                setCurrentGameSave: mockSetCurrentGameSave,
            });
        });

        it("should render GameSaveSelectionPage", () => {
            render(
                <GameSelectionWrapper>
                    <div data-testid="game-content">Game Content</div>
                </GameSelectionWrapper>
            );

            expect(
                screen.getByTestId("game-save-selection-page")
            ).toBeInTheDocument();
            expect(
                screen.queryByTestId("game-content")
            ).not.toBeInTheDocument();
        });

        it("should set user ID on http client when user exists", () => {
            render(
                <GameSelectionWrapper>
                    <div>Game Content</div>
                </GameSelectionWrapper>
            );

            expect(mockHttpClient.setUserId).toHaveBeenCalledWith("user-456");
        });

        it("should not set user ID when user is null", () => {
            vi.mocked(
                PokeGameUserContext.useGetPokeGameUserContext
            ).mockReturnValue(null);

            render(
                <GameSelectionWrapper>
                    <div>Game Content</div>
                </GameSelectionWrapper>
            );

            expect(mockHttpClient.setUserId).not.toHaveBeenCalled();
        });

        it("should call setCurrentGameSave when a game save is selected", () => {
            render(
                <GameSelectionWrapper>
                    <div>Game Content</div>
                </GameSelectionWrapper>
            );

            const selectButton = screen.getByText("Select Game");
            selectButton.click();

            expect(mockSetCurrentGameSave).toHaveBeenCalledWith(mockGameSave);
        });
    });

    describe("when a game save is selected", () => {
        beforeEach(() => {
            vi.mocked(GameSaveContext.useGameSaveContext).mockReturnValue({
                currentGameSave: mockGameSave,
                setCurrentGameSave: mockSetCurrentGameSave,
            });
        });

        it("should render children instead of selection page", () => {
            render(
                <GameSelectionWrapper>
                    <div data-testid="game-content">Game Content</div>
                </GameSelectionWrapper>
            );

            expect(screen.getByTestId("game-content")).toBeInTheDocument();
            expect(
                screen.queryByTestId("game-save-selection-page")
            ).not.toBeInTheDocument();
        });

        it("should still set user ID on http client", () => {
            render(
                <GameSelectionWrapper>
                    <div>Game Content</div>
                </GameSelectionWrapper>
            );

            expect(mockHttpClient.setUserId).toHaveBeenCalledWith("user-456");
        });

        it("should render multiple children", () => {
            render(
                <GameSelectionWrapper>
                    <div data-testid="child-1">Child 1</div>
                    <div data-testid="child-2">Child 2</div>
                </GameSelectionWrapper>
            );

            expect(screen.getByTestId("child-1")).toBeInTheDocument();
            expect(screen.getByTestId("child-2")).toBeInTheDocument();
        });
    });

    describe("user ID changes", () => {
        it("should update http client when user ID changes", () => {
            const { rerender } = render(
                <GameSelectionWrapper>
                    <div>Game Content</div>
                </GameSelectionWrapper>
            );

            expect(mockHttpClient.setUserId).toHaveBeenCalledWith("user-456");

            // Simulate user change
            const newUser = { ...mockUser, id: "user-789" };
            vi.mocked(
                PokeGameUserContext.useGetPokeGameUserContext
            ).mockReturnValue(newUser);

            vi.mocked(GameSaveContext.useGameSaveContext).mockReturnValue({
                currentGameSave: null,
                setCurrentGameSave: mockSetCurrentGameSave,
            });

            rerender(
                <GameSelectionWrapper>
                    <div>Game Content</div>
                </GameSelectionWrapper>
            );

            expect(mockHttpClient.setUserId).toHaveBeenCalledWith("user-789");
        });
    });

    describe("edge cases", () => {
        it("should handle undefined user gracefully", () => {
            vi.mocked(
                PokeGameUserContext.useGetPokeGameUserContext
            ).mockReturnValue(undefined as any);
            vi.mocked(GameSaveContext.useGameSaveContext).mockReturnValue({
                currentGameSave: null,
                setCurrentGameSave: mockSetCurrentGameSave,
            });

            render(
                <GameSelectionWrapper>
                    <div>Game Content</div>
                </GameSelectionWrapper>
            );

            expect(mockHttpClient.setUserId).not.toHaveBeenCalled();
        });

        it("should handle user without id property", () => {
            vi.mocked(
                PokeGameUserContext.useGetPokeGameUserContext
            ).mockReturnValue({ username: "test" } as any);
            vi.mocked(GameSaveContext.useGameSaveContext).mockReturnValue({
                currentGameSave: null,
                setCurrentGameSave: mockSetCurrentGameSave,
            });

            render(
                <GameSelectionWrapper>
                    <div>Game Content</div>
                </GameSelectionWrapper>
            );

            expect(mockHttpClient.setUserId).not.toHaveBeenCalled();
        });
    });
});
