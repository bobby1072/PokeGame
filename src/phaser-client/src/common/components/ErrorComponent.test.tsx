import { describe, it, expect } from "vitest";
import { render, screen } from "../../../test/utils/test-utils";
import { ErrorComponent } from "./ErrorComponent";
import { AxiosError } from "axios";
import { FieldErrors } from "react-hook-form";

describe("ErrorComponent", () => {
    describe("rendering with different error types", () => {
        it("should not render anything when error is null", () => {
            const { container } = render(<ErrorComponent error={null} />);
            expect(container.firstChild).toBeNull();
        });

        it("should not render anything when error is undefined", () => {
            const { container } = render(<ErrorComponent />);
            expect(container.firstChild).toBeNull();
        });

        it("should render string error", () => {
            render(<ErrorComponent error="Test error message" />);
            expect(screen.getByText("Test error message")).toBeInTheDocument();
        });

        it("should render Error object", () => {
            const error = new Error("Error object message");
            render(<ErrorComponent error={error} />);
            expect(
                screen.getByText("Error object message")
            ).toBeInTheDocument();
        });

        it("should render AxiosError with response data", () => {
            const axiosError = new AxiosError("API error");
            Object.defineProperty(axiosError, "response", {
                value: {
                    data: {
                        exceptionMessage: "API error message",
                    },
                },
                writable: true,
                configurable: true,
            });

            render(<ErrorComponent error={axiosError} />);
            expect(screen.getByText("API error message")).toBeInTheDocument();
        });

        it("should render fallback message for AxiosError without response data", () => {
            const axiosError = new AxiosError("API error");
            Object.defineProperty(axiosError, "response", {
                value: {
                    data: {},
                },
                writable: true,
                configurable: true,
            });

            render(<ErrorComponent error={axiosError} />);
            expect(
                screen.getByText("An unexpected error occurred")
            ).toBeInTheDocument();
        });

        it("should render FieldErrors with multiple messages", () => {
            const fieldErrors: FieldErrors = {
                username: {
                    type: "required",
                    message: "Username is required",
                },
                email: {
                    type: "pattern",
                    message: "Invalid email format",
                },
            };

            render(<ErrorComponent error={fieldErrors} />);
            expect(
                screen.getByText("Username is required")
            ).toBeInTheDocument();
            expect(
                screen.getByText("Invalid email format")
            ).toBeInTheDocument();
        });

        it("should not render anything for empty FieldErrors", () => {
            const fieldErrors: FieldErrors = {};
            const { container } = render(
                <ErrorComponent error={fieldErrors} />
            );
            expect(container.firstChild).toBeNull();
        });

        it("should filter out field errors without messages", () => {
            const fieldErrors: FieldErrors = {
                valid: {
                    type: "required",
                    message: "This is valid",
                },
                invalid: null,
            };

            render(<ErrorComponent error={fieldErrors} />);
            expect(screen.getByText("This is valid")).toBeInTheDocument();
            expect(screen.queryAllByRole("alert")).toHaveLength(1);
        });
    });

    describe("variant prop", () => {
        it("should render with page-section variant by default", () => {
            const { container } = render(<ErrorComponent error="Test error" />);
            const box = container.querySelector(".MuiBox-root");
            expect(box).toBeInTheDocument();
        });

        it("should render with fullscreen variant", () => {
            const { container } = render(
                <ErrorComponent error="Test error" variant="fullscreen" />
            );
            const box = container.querySelector(".MuiBox-root");
            expect(box).toBeInTheDocument();
        });

        it("should render with inline variant", () => {
            const { container } = render(
                <ErrorComponent error="Test error" variant="inline" />
            );
            const box = container.querySelector(".MuiBox-root");
            expect(box).toBeInTheDocument();
        });
    });

    describe("custom sx prop", () => {
        it("should apply custom sx styles", () => {
            const customSx = { backgroundColor: "red" };
            render(<ErrorComponent error="Test error" sx={customSx} />);
            expect(screen.getByText("Test error")).toBeInTheDocument();
        });

        it("should apply array of sx styles", () => {
            const customSx = [{ backgroundColor: "red" }, { color: "white" }];
            render(<ErrorComponent error="Test error" sx={customSx} />);
            expect(screen.getByText("Test error")).toBeInTheDocument();
        });
    });

    describe("alert severity", () => {
        it("should always render alerts with error severity", () => {
            render(<ErrorComponent error="Test error" />);
            const alert = screen.getByRole("alert");
            expect(alert).toHaveClass("MuiAlert-standardError");
        });
    });

    describe("multiple error messages", () => {
        it("should render multiple alerts for FieldErrors", () => {
            const fieldErrors: FieldErrors = {
                field1: { message: "Error 1" },
                field2: { message: "Error 2" },
                field3: { message: "Error 3" },
            };

            render(<ErrorComponent error={fieldErrors} />);
            expect(screen.getAllByRole("alert")).toHaveLength(3);
        });

        it("should render single alert for string error", () => {
            render(<ErrorComponent error="Single error" />);
            expect(screen.getAllByRole("alert")).toHaveLength(1);
        });
    });
});
