import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "../../../test/utils/test-utils";
import { LoginPage } from "./LoginPage";
import { PokeGameUser } from "../models/PokeGameUser";
import * as AppSettingsContext from "../contexts/AppSettingsContext";
import userEvent from "@testing-library/user-event";

// Mock hooks
vi.mock("../hooks/useGetUserMutation");
vi.mock("../hooks/useSaveUserMutation");
vi.mock("../contexts/AppSettingsContext");

const mockUser: PokeGameUser = {
    id: "user-123",
    name: "Test User",
    email: "test@example.com",
};

describe("LoginPage", () => {
    const mockSetUser = vi.fn();
    const mockGetUserMutate = vi.fn();
    const mockSaveUserMutate = vi.fn();
    const mockGetUserReset = vi.fn();
    const mockSaveUserReset = vi.fn();

    beforeEach(async () => {
        vi.clearAllMocks();

        vi.mocked(AppSettingsContext.useGetAppSettingsContext).mockReturnValue({
            serviceName: "PokeGame Test",
            releaseVersion: "1.0.0",
            coreApiUrl: "http://localhost:5000",
            signalRUrl: "http://localhost:5000/hub",
        });

        const { useGetUserMutation } = await import(
            "../hooks/useGetUserMutation"
        );
        vi.mocked(useGetUserMutation).mockReturnValue({
            data: undefined,
            error: null,
            isPending: false,
            mutate: mockGetUserMutate,
            reset: mockGetUserReset,
        } as any);

        const { useSaveUserMutation } = await import(
            "../hooks/useSaveUserMutation"
        );
        vi.mocked(useSaveUserMutation).mockReturnValue({
            data: undefined,
            error: null,
            isPending: false,
            mutate: mockSaveUserMutate,
            reset: mockSaveUserReset,
        } as any);
    });

    describe("rendering", () => {
        it("should render page title", () => {
            render(<LoginPage setUser={mockSetUser} />);

            expect(screen.getByText("PokeGame")).toBeInTheDocument();
        });

        it("should render login and register tabs", () => {
            render(<LoginPage setUser={mockSetUser} />);

            expect(
                screen.getByRole("tab", { name: "Login" })
            ).toBeInTheDocument();
            expect(
                screen.getByRole("tab", { name: "Register" })
            ).toBeInTheDocument();
        });

        it("should show login form by default", () => {
            render(<LoginPage setUser={mockSetUser} />);

            expect(screen.getByTestId("login-form")).toBeInTheDocument();
            expect(
                screen.queryByTestId("register-form")
            ).not.toBeInTheDocument();
        });
    });

    describe("tab switching", () => {
        it("should switch to register tab when clicked", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            const registerTab = screen.getByRole("tab", { name: "Register" });
            await user.click(registerTab);

            expect(screen.getByTestId("register-form")).toBeInTheDocument();
            expect(screen.queryByTestId("login-form")).not.toBeInTheDocument();
        });

        it("should switch back to login tab", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            // Switch to register
            await user.click(screen.getByRole("tab", { name: "Register" }));
            expect(screen.getByTestId("register-form")).toBeVisible();

            // Switch back to login
            await user.click(screen.getByRole("tab", { name: "Login" }));
            expect(screen.getByTestId("login-form")).toBeInTheDocument();
        });

        it("should reset forms when switching tabs", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            // Type in login form
            const loginEmailInput = screen.getByLabelText(/email/i, {
                selector: 'input[type="email"]',
            });
            await user.type(loginEmailInput, "test@example.com");
            expect(loginEmailInput).toHaveValue("test@example.com");

            // Switch tabs
            await user.click(screen.getByRole("tab", { name: "Register" }));
            await user.click(screen.getByRole("tab", { name: "Login" }));

            // Login form should be reset
            expect(mockGetUserReset).toHaveBeenCalled();
            expect(mockSaveUserReset).toHaveBeenCalled();
        });
    });

    describe("login form", () => {
        it("should render email field", () => {
            render(<LoginPage setUser={mockSetUser} />);

            expect(
                screen.getByRole("textbox", { name: /email/i })
            ).toBeInTheDocument();
        });

        it("should render submit button", () => {
            render(<LoginPage setUser={mockSetUser} />);

            const submitButton = screen.getByTestId("login-submit-button");
            expect(submitButton).toBeInTheDocument();
            expect(submitButton).toHaveTextContent("Login");
        });

        it("should disable submit button when form is pristine", () => {
            render(<LoginPage setUser={mockSetUser} />);

            const submitButton = screen.getByTestId("login-submit-button");
            expect(submitButton).toBeDisabled();
        });

        it("should enable submit button when email is entered", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            const emailInput = screen.getByRole("textbox", { name: /email/i });
            await user.type(emailInput, "test@example.com");

            const submitButton = screen.getByTestId("login-submit-button");
            expect(submitButton).not.toBeDisabled();
        });

        it("should prevent submission with invalid email", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            const emailInput = screen.getByRole("textbox", { name: /email/i });
            await user.type(emailInput, "invalid-email");

            const submitButton = screen.getByTestId("login-submit-button");
            await user.click(submitButton);

            // Mutation should not be called if validation fails
            expect(mockGetUserMutate).not.toHaveBeenCalled();
        });

        it("should call getUserMutation on submit", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            const emailInput = screen.getByRole("textbox", { name: /email/i });
            await user.type(emailInput, "test@example.com");

            const submitButton = screen.getByTestId("login-submit-button");
            await user.click(submitButton);

            expect(mockGetUserMutate).toHaveBeenCalledWith({
                email: "test@example.com",
            });
        });

        it("should show success message on successful login", async () => {
            const { useGetUserMutation } = await import(
                "../hooks/useGetUserMutation"
            );
            vi.mocked(useGetUserMutation).mockReturnValue({
                data: mockUser,
                error: null,
                isPending: false,
                mutate: mockGetUserMutate,
                reset: mockGetUserReset,
            } as any);

            render(<LoginPage setUser={mockSetUser} />);

            await waitFor(() => {
                expect(
                    screen.getByText("Login successful!")
                ).toBeInTheDocument();
            });
            expect(mockSetUser).toHaveBeenCalledWith(mockUser);
        });

        it("should show loading state while logging in", async () => {
            const { useGetUserMutation } = await import(
                "../hooks/useGetUserMutation"
            );
            vi.mocked(useGetUserMutation).mockReturnValue({
                data: undefined,
                error: null,
                isPending: true,
                mutate: mockGetUserMutate,
                reset: mockGetUserReset,
            } as any);

            render(<LoginPage setUser={mockSetUser} />);

            const submitButton = screen.getByTestId("login-submit-button");
            expect(submitButton).toBeDisabled();
        });

        it("should display error when login fails", async () => {
            const mockError = new Error("User not found");
            const { useGetUserMutation } = await import(
                "../hooks/useGetUserMutation"
            );
            vi.mocked(useGetUserMutation).mockReturnValue({
                data: undefined,
                error: mockError,
                isPending: false,
                mutate: mockGetUserMutate,
                reset: mockGetUserReset,
            } as any);

            render(<LoginPage setUser={mockSetUser} />);

            expect(screen.getByText("User not found")).toBeInTheDocument();
        });
    });

    describe("register form", () => {
        it("should render email and name fields", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            await user.click(screen.getByRole("tab", { name: "Register" }));

            expect(
                screen.getAllByRole("textbox", { name: /email/i })[0]
            ).toBeInTheDocument();
            expect(
                screen.getByRole("textbox", { name: /name/i })
            ).toBeInTheDocument();
        });

        it("should render submit button", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            await user.click(screen.getByRole("tab", { name: "Register" }));

            const submitButton = screen.getByTestId("register-submit-button");
            expect(submitButton).toBeInTheDocument();
            expect(submitButton).toHaveTextContent("Register");
        });

        it("should disable submit button when form is pristine", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            await user.click(screen.getByRole("tab", { name: "Register" }));

            const submitButton = screen.getByTestId("register-submit-button");
            expect(submitButton).toBeDisabled();
        });

        it("should enable submit button when all fields are filled", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            await user.click(screen.getByRole("tab", { name: "Register" }));

            const inputs = screen.getAllByRole("textbox");
            const emailInput = inputs.find(
                (input) => input.getAttribute("type") === "email"
            );
            const nameInput = screen.getByRole("textbox", { name: /name/i });

            await user.type(emailInput!, "test@example.com");
            await user.type(nameInput, "Test User");

            const submitButton = screen.getByTestId("register-submit-button");
            expect(submitButton).not.toBeDisabled();
        });

        it("should prevent submission with short name", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            await user.click(screen.getByRole("tab", { name: "Register" }));

            const inputs = screen.getAllByRole("textbox");
            const emailInput = inputs.find(
                (input) => input.getAttribute("type") === "email"
            );
            const nameInput = screen.getByRole("textbox", { name: /name/i });

            await user.type(emailInput!, "test@example.com");
            await user.type(nameInput, "A");

            const submitButton = screen.getByTestId("register-submit-button");
            await user.click(submitButton);

            // Mutation should not be called if validation fails
            expect(mockSaveUserMutate).not.toHaveBeenCalled();
        });

        it("should call saveUserMutation on submit", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            await user.click(screen.getByRole("tab", { name: "Register" }));

            const inputs = screen.getAllByRole("textbox");
            const emailInput = inputs.find(
                (input) => input.getAttribute("type") === "email"
            );
            const nameInput = screen.getByRole("textbox", { name: /name/i });

            await user.type(emailInput!, "test@example.com");
            await user.type(nameInput, "Test User");

            const submitButton = screen.getByTestId("register-submit-button");
            await user.click(submitButton);

            expect(mockSaveUserMutate).toHaveBeenCalledWith({
                userInput: {
                    email: "test@example.com",
                    name: "Test User",
                },
            });
        });

        it("should call setUser when registration succeeds", async () => {
            // Mock successful registration
            const { useSaveUserMutation } = await import(
                "../hooks/useSaveUserMutation"
            );
            vi.mocked(useSaveUserMutation).mockReturnValue({
                data: mockUser,
                error: null,
                isPending: false,
                mutate: mockSaveUserMutate,
                reset: mockSaveUserReset,
            } as any);

            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            await user.click(screen.getByRole("tab", { name: "Register" }));

            // setUser should be called with the mock user from the mutation
            expect(mockSetUser).toHaveBeenCalledWith(mockUser);
        });

        it("should show loading state while registering", async () => {
            const { useSaveUserMutation } = await import(
                "../hooks/useSaveUserMutation"
            );
            vi.mocked(useSaveUserMutation).mockReturnValue({
                data: undefined,
                error: null,
                isPending: true,
                mutate: mockSaveUserMutate,
                reset: mockSaveUserReset,
            } as any);

            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            await user.click(screen.getByRole("tab", { name: "Register" }));

            const submitButton = screen.getByTestId("register-submit-button");
            expect(submitButton).toBeDisabled();
        });

        it("should display error when registration fails", async () => {
            const mockError = new Error("Email already exists");
            const { useSaveUserMutation } = await import(
                "../hooks/useSaveUserMutation"
            );
            vi.mocked(useSaveUserMutation).mockReturnValue({
                data: undefined,
                error: mockError,
                isPending: false,
                mutate: mockSaveUserMutate,
                reset: mockSaveUserReset,
            } as any);

            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            await user.click(screen.getByRole("tab", { name: "Register" }));

            expect(
                screen.getByText("Email already exists")
            ).toBeInTheDocument();
        });
    });

    describe("integration", () => {
        it("should handle complete login flow", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            // Enter email
            const emailInput = screen.getByRole("textbox", { name: /email/i });
            await user.type(emailInput, "test@example.com");

            // Submit form
            const submitButton = screen.getByTestId("login-submit-button");
            await user.click(submitButton);

            // Verify mutation was called
            expect(mockGetUserMutate).toHaveBeenCalledWith({
                email: "test@example.com",
            });
        });

        it("should handle complete registration flow", async () => {
            const user = userEvent.setup();
            render(<LoginPage setUser={mockSetUser} />);

            // Switch to register tab
            await user.click(screen.getByRole("tab", { name: "Register" }));

            // Fill form
            const inputs = screen.getAllByRole("textbox");
            const emailInput = inputs.find(
                (input) => input.getAttribute("type") === "email"
            );
            const nameInput = screen.getByRole("textbox", { name: /name/i });

            await user.type(emailInput!, "newuser@example.com");
            await user.type(nameInput, "New User");

            // Submit form
            const submitButton = screen.getByTestId("register-submit-button");
            await user.click(submitButton);

            // Verify mutation was called
            expect(mockSaveUserMutate).toHaveBeenCalledWith({
                userInput: {
                    email: "newuser@example.com",
                    name: "New User",
                },
            });
        });
    });
});
