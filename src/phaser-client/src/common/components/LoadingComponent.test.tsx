import { describe, it, expect } from "vitest";
import { render, screen } from "../../../test/utils/test-utils";
import { LoadingComponent } from "./LoadingComponent";

describe("LoadingComponent", () => {
    describe("variant prop", () => {
        it("should render with page-section variant by default", () => {
            const { container } = render(<LoadingComponent />);
            const spinner = container.querySelector(
                ".MuiCircularProgress-root"
            );
            expect(spinner).toBeInTheDocument();
        });

        it("should render with fullscreen variant", () => {
            const { container } = render(
                <LoadingComponent variant="fullscreen" />
            );
            const spinner = container.querySelector(
                ".MuiCircularProgress-root"
            );
            expect(spinner).toBeInTheDocument();
        });

        it("should render with inline variant", () => {
            const { container } = render(<LoadingComponent variant="inline" />);
            const spinner = container.querySelector(
                ".MuiCircularProgress-root"
            );
            expect(spinner).toBeInTheDocument();
        });
    });

    describe("spinner size", () => {
        it("should use default size for fullscreen variant (40)", () => {
            const { container } = render(
                <LoadingComponent variant="fullscreen" />
            );
            const spinner = container.querySelector(
                ".MuiCircularProgress-root"
            );
            expect(spinner).toHaveStyle({ width: "40px", height: "40px" });
        });

        it("should use default size for page-section variant (60)", () => {
            const { container } = render(
                <LoadingComponent variant="page-section" />
            );
            const spinner = container.querySelector(
                ".MuiCircularProgress-root"
            );
            expect(spinner).toHaveStyle({ width: "60px", height: "60px" });
        });

        it("should use default size for inline variant (24)", () => {
            const { container } = render(<LoadingComponent variant="inline" />);
            const spinner = container.querySelector(
                ".MuiCircularProgress-root"
            );
            expect(spinner).toHaveStyle({ width: "24px", height: "24px" });
        });

        it("should override default size with custom size prop", () => {
            const { container } = render(
                <LoadingComponent variant="fullscreen" size={100} />
            );
            const spinner = container.querySelector(
                ".MuiCircularProgress-root"
            );
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
            const { container } = render(<LoadingComponent />);
            const box = container.querySelector(".MuiBox-root");
            expect(box).toBeInTheDocument();
        });

        it("should center content for fullscreen variant", () => {
            const { container } = render(
                <LoadingComponent variant="fullscreen" />
            );
            const box = container.querySelector(".MuiBox-root");
            expect(box).toBeInTheDocument();
        });

        it("should use text.secondary color for message", () => {
            render(
                <LoadingComponent variant="page-section" message="Loading..." />
            );
            const message = screen.getByText("Loading...");
            expect(message).toHaveClass("MuiTypography-root");
        });
    });

    describe("edge cases", () => {
        it("should handle empty message string", () => {
            render(<LoadingComponent variant="page-section" message="" />);
            expect(screen.queryByRole("heading")).not.toBeInTheDocument();
        });

        it("should handle size of 0", () => {
            const { container } = render(<LoadingComponent size={0} />);
            const spinner = container.querySelector(
                ".MuiCircularProgress-root"
            );
            expect(spinner).toHaveStyle({ width: "0px", height: "0px" });
        });

        it("should handle very large size", () => {
            const { container } = render(<LoadingComponent size={500} />);
            const spinner = container.querySelector(
                ".MuiCircularProgress-root"
            );
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
            const spinner = container.querySelector(
                ".MuiCircularProgress-root"
            );
            expect(spinner).toHaveStyle({ width: "80px", height: "80px" });
        });
    });
});
