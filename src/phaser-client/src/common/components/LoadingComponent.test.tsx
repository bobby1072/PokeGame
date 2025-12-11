import { describe, it, expect } from "vitest";
import { render, screen } from "../../../test/utils/test-utils";
import { LoadingComponent } from "./LoadingComponent";

describe("LoadingComponent", () => {
    describe("variant prop", () => {
        it("should render with page-section variant by default", () => {
            render(<LoadingComponent />);
            expect(screen.getByTestId("loading-spinner")).toBeInTheDocument();
            expect(screen.getByTestId("loading-container")).toBeInTheDocument();
        });

        it("should render with fullscreen variant", () => {
            render(<LoadingComponent variant="fullscreen" />);
            expect(screen.getByTestId("loading-spinner")).toBeInTheDocument();
        });

        it("should render with inline variant", () => {
            render(<LoadingComponent variant="inline" />);
            expect(screen.getByTestId("loading-spinner")).toBeInTheDocument();
        });
    });

    describe("spinner size", () => {
        it("should use default size for fullscreen variant (40)", () => {
            render(<LoadingComponent variant="fullscreen" />);
            const spinner = screen.getByTestId("loading-spinner");
            expect(spinner).toHaveStyle({ width: "40px", height: "40px" });
        });

        it("should use default size for page-section variant (60)", () => {
            render(<LoadingComponent variant="page-section" />);
            const spinner = screen.getByTestId("loading-spinner");
            expect(spinner).toHaveStyle({ width: "60px", height: "60px" });
        });

        it("should use default size for inline variant (24)", () => {
            render(<LoadingComponent variant="inline" />);
            const spinner = screen.getByTestId("loading-spinner");
            expect(spinner).toHaveStyle({ width: "24px", height: "24px" });
        });

        it("should override default size with custom size prop", () => {
            render(<LoadingComponent variant="fullscreen" size={100} />);
            const spinner = screen.getByTestId("loading-spinner");
            expect(spinner).toHaveStyle({ width: "100px", height: "100px" });
        });
    });

    describe("message prop", () => {
        it("should not render message by default", () => {
            render(<LoadingComponent />);
            expect(screen.queryByText("Loading...")).not.toBeInTheDocument();
        });

        it("should render message when provided for page-section variant", () => {
            render(
                <LoadingComponent
                    variant="page-section"
                    message="Loading game data..."
                />
            );
            expect(
                screen.getByText("Loading game data...")
            ).toBeInTheDocument();
        });

        it("should render message when provided for fullscreen variant", () => {
            render(
                <LoadingComponent
                    variant="fullscreen"
                    message="Loading game..."
                />
            );
            expect(screen.getByText("Loading game...")).toBeInTheDocument();
        });

        it("should not render message for inline variant", () => {
            render(
                <LoadingComponent
                    variant="inline"
                    message="This should not appear"
                />
            );
            expect(
                screen.queryByText("This should not appear")
            ).not.toBeInTheDocument();
        });
    });

    describe("custom sx prop", () => {
        it("should apply custom styles", () => {
            const { container } = render(
                <LoadingComponent sx={{ backgroundColor: "red" }} />
            );
            const box = container.querySelector(".MuiBox-root");
            expect(box).toBeInTheDocument();
        });

        it("should merge custom styles with variant styles", () => {
            const { container } = render(
                <LoadingComponent
                    variant="fullscreen"
                    sx={{ padding: "20px" }}
                />
            );
            const box = container.querySelector(".MuiBox-root");
            expect(box).toBeInTheDocument();
        });

        it("should handle array of sx props", () => {
            const { container } = render(
                <LoadingComponent
                    sx={[{ backgroundColor: "blue" }, { color: "white" }]}
                />
            );
            const box = container.querySelector(".MuiBox-root");
            expect(box).toBeInTheDocument();
        });
    });

    describe("accessibility", () => {
        it("should render CircularProgress with proper role", () => {
            const { container } = render(<LoadingComponent />);
            const spinner = container.querySelector(
                ".MuiCircularProgress-root"
            );
            expect(spinner).toBeInTheDocument();
        });

        it("should render message with proper typography", () => {
            render(
                <LoadingComponent variant="fullscreen" message="Loading..." />
            );
            const message = screen.getByText("Loading...");
            expect(message).toHaveClass("MuiTypography-root");
        });
    });

    describe("styling", () => {
        it("should have proper container structure", () => {
            render(<LoadingComponent />);
            expect(screen.getByTestId("loading-container")).toBeInTheDocument();
        });

        it("should center content for fullscreen variant", () => {
            render(<LoadingComponent variant="fullscreen" />);
            expect(screen.getByTestId("loading-container")).toBeInTheDocument();
        });

        it("should display message with proper styling", () => {
            render(
                <LoadingComponent variant="page-section" message="Loading..." />
            );
            const message = screen.getByTestId("loading-message");
            expect(message).toBeInTheDocument();
            expect(message).toHaveTextContent("Loading...");
        });
    });

    describe("edge cases", () => {
        it("should handle empty message string", () => {
            render(<LoadingComponent variant="page-section" message="" />);
            expect(screen.queryByRole("heading")).not.toBeInTheDocument();
        });

        it("should handle size of 0", () => {
            render(<LoadingComponent size={0} />);
            const spinner = screen.getByTestId("loading-spinner");
            expect(spinner).toHaveStyle({ width: "0px", height: "0px" });
        });

        it("should handle very large size", () => {
            render(<LoadingComponent size={500} />);
            const spinner = screen.getByTestId("loading-spinner");
            expect(spinner).toHaveStyle({ width: "500px", height: "500px" });
        });

        it("should handle long message text", () => {
            const longMessage = "A".repeat(200);
            render(
                <LoadingComponent variant="fullscreen" message={longMessage} />
            );
            expect(screen.getByText(longMessage)).toBeInTheDocument();
        });
    });

    describe("combined props", () => {
        it("should handle all props together", () => {
            const { container } = render(
                <LoadingComponent
                    variant="fullscreen"
                    message="Custom loading message"
                    size={80}
                    sx={{ backgroundColor: "blue" }}
                />
            );
            expect(
                screen.getByText("Custom loading message")
            ).toBeInTheDocument();
            const spinner = screen.getByTestId("loading-spinner");
            expect(spinner).toHaveStyle({ width: "80px", height: "80px" });
        });
    });
});
