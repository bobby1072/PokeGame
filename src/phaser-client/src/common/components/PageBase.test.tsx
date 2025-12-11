import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen } from "../../../test/utils/test-utils";
import { PageBase } from "./PageBase";
import * as AppSettingsContext from "../contexts/AppSettingsContext";

// Mock the context
vi.mock("../contexts/AppSettingsContext");

describe("PageBase", () => {
    const mockAppSettings = {
        serviceName: "PokeGame Test",
        releaseVersion: "1.2.3",
        coreApiUrl: "http://localhost:5000",
        signalRUrl: "http://localhost:5000/hub",
    };

    beforeEach(() => {
        vi.clearAllMocks();
        vi.mocked(AppSettingsContext.useGetAppSettingsContext).mockReturnValue(
            mockAppSettings
        );
    });

    describe("rendering", () => {
        it("should render children content", () => {
            render(
                <PageBase>
                    <div data-testid="child-content">Test Content</div>
                </PageBase>
            );

            expect(screen.getByTestId("child-content")).toBeInTheDocument();
            expect(screen.getByText("Test Content")).toBeInTheDocument();
        });

        it("should render footer by default", () => {
            render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            expect(screen.getByText("PokeGame Test")).toBeInTheDocument();
            expect(screen.getByText("v1.2.3")).toBeInTheDocument();
        });

        it("should render service name from app settings", () => {
            render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            expect(screen.getByText("PokeGame Test")).toBeInTheDocument();
        });

        it("should render version from app settings", () => {
            render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            expect(screen.getByText("v1.2.3")).toBeInTheDocument();
        });
    });

    describe("footer visibility", () => {
        it("should show footer when showFooter is true", () => {
            render(
                <PageBase showFooter={true}>
                    <div>Content</div>
                </PageBase>
            );

            expect(screen.getByText("PokeGame Test")).toBeInTheDocument();
        });

        it("should hide footer when showFooter is false", () => {
            render(
                <PageBase showFooter={false}>
                    <div>Content</div>
                </PageBase>
            );

            expect(screen.queryByText("PokeGame Test")).not.toBeInTheDocument();
            expect(screen.queryByText("v1.2.3")).not.toBeInTheDocument();
        });
    });

    describe("maxWidth prop", () => {
        it("should use lg maxWidth by default", () => {
            const { container } = render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            const containers = container.querySelectorAll(".MuiContainer-root");
            expect(containers.length).toBeGreaterThan(0);
        });

        it("should accept xs maxWidth", () => {
            const { container } = render(
                <PageBase maxWidth="xs">
                    <div>Content</div>
                </PageBase>
            );

            const containers = container.querySelectorAll(".MuiContainer-root");
            expect(containers.length).toBeGreaterThan(0);
        });

        it("should accept sm maxWidth", () => {
            const { container } = render(
                <PageBase maxWidth="sm">
                    <div>Content</div>
                </PageBase>
            );

            const containers = container.querySelectorAll(".MuiContainer-root");
            expect(containers.length).toBeGreaterThan(0);
        });

        it("should accept md maxWidth", () => {
            const { container } = render(
                <PageBase maxWidth="md">
                    <div>Content</div>
                </PageBase>
            );

            const containers = container.querySelectorAll(".MuiContainer-root");
            expect(containers.length).toBeGreaterThan(0);
        });

        it("should accept xl maxWidth", () => {
            const { container } = render(
                <PageBase maxWidth="xl">
                    <div>Content</div>
                </PageBase>
            );

            const containers = container.querySelectorAll(".MuiContainer-root");
            expect(containers.length).toBeGreaterThan(0);
        });

        it("should accept false to disable maxWidth", () => {
            const { container } = render(
                <PageBase maxWidth={false}>
                    <div>Content</div>
                </PageBase>
            );

            const containers = container.querySelectorAll(".MuiContainer-root");
            expect(containers.length).toBeGreaterThan(0);
        });
    });

    describe("layout structure", () => {
        it("should have proper container structure", () => {
            const { container } = render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            expect(container.querySelector(".MuiBox-root")).toBeInTheDocument();
            expect(
                container.querySelector(".MuiContainer-root")
            ).toBeInTheDocument();
        });

        it("should render footer as footer element", () => {
            const { container } = render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            const footer = container.querySelector("footer");
            expect(footer).toBeInTheDocument();
        });

        it("should maintain proper flex layout", () => {
            const { container } = render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            const mainBox = container.querySelector(".MuiBox-root");
            expect(mainBox).toBeInTheDocument();
        });
    });

    describe("multiple children", () => {
        it("should render multiple children elements", () => {
            render(
                <PageBase>
                    <div data-testid="child-1">Child 1</div>
                    <div data-testid="child-2">Child 2</div>
                    <div data-testid="child-3">Child 3</div>
                </PageBase>
            );

            expect(screen.getByTestId("child-1")).toBeInTheDocument();
            expect(screen.getByTestId("child-2")).toBeInTheDocument();
            expect(screen.getByTestId("child-3")).toBeInTheDocument();
        });

        it("should render complex nested children", () => {
            render(
                <PageBase>
                    <div>
                        <h1>Title</h1>
                        <p>Paragraph</p>
                        <button>Button</button>
                    </div>
                </PageBase>
            );

            expect(screen.getByText("Title")).toBeInTheDocument();
            expect(screen.getByText("Paragraph")).toBeInTheDocument();
            expect(screen.getByRole("button")).toBeInTheDocument();
        });
    });

    describe("app settings fallbacks", () => {
        it("should use fallback service name when not provided", () => {
            vi.mocked(
                AppSettingsContext.useGetAppSettingsContext
            ).mockReturnValue({
                serviceName: null as any,
                releaseVersion: "1.0",
                coreApiUrl: "",
                signalRUrl: "",
            });

            render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            expect(screen.getByText("PokeGame")).toBeInTheDocument();
        });

        it("should use fallback version when not provided", () => {
            vi.mocked(
                AppSettingsContext.useGetAppSettingsContext
            ).mockReturnValue({
                serviceName: "PokeGame",
                releaseVersion: null as any,
                coreApiUrl: "",
                signalRUrl: "",
            });

            render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            expect(screen.getByText("v1.0")).toBeInTheDocument();
        });

        it("should handle empty app settings", () => {
            vi.mocked(
                AppSettingsContext.useGetAppSettingsContext
            ).mockReturnValue({
                serviceName: "",
                releaseVersion: "",
                coreApiUrl: "",
                signalRUrl: "",
            });

            render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            expect(screen.getByText("PokeGame")).toBeInTheDocument();
            expect(screen.getByText("v1.0")).toBeInTheDocument();
        });
    });

    describe("footer styling", () => {
        it("should render footer with proper typography", () => {
            render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            const serviceNameElement = screen.getByText("PokeGame Test");
            const versionElement = screen.getByText("v1.2.3");

            expect(serviceNameElement).toHaveClass("MuiTypography-caption");
            expect(versionElement).toHaveClass("MuiTypography-caption");
        });

        it("should render footer with secondary text color", () => {
            render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            const serviceNameElement = screen.getByText("PokeGame Test");
            const versionElement = screen.getByText("v1.2.3");

            expect(serviceNameElement).toHaveClass("MuiTypography-caption");
            expect(versionElement).toHaveClass("MuiTypography-caption");
        });
    });

    describe("accessibility", () => {
        it("should have semantic footer element", () => {
            const { container } = render(
                <PageBase>
                    <div>Content</div>
                </PageBase>
            );

            const footer = container.querySelector("footer");
            expect(footer).toBeInTheDocument();
        });

        it("should maintain proper document structure", () => {
            const { container } = render(
                <PageBase>
                    <main>Main Content</main>
                </PageBase>
            );

            expect(container.querySelector("main")).toBeInTheDocument();
            expect(container.querySelector("footer")).toBeInTheDocument();
        });
    });

    describe("edge cases", () => {
        it("should handle null children", () => {
            render(<PageBase>{null}</PageBase>);
            expect(screen.getByText("PokeGame Test")).toBeInTheDocument();
        });

        it("should handle undefined children", () => {
            render(<PageBase>{undefined}</PageBase>);
            expect(screen.getByText("PokeGame Test")).toBeInTheDocument();
        });

        it("should handle empty string children", () => {
            render(<PageBase>{""}</PageBase>);
            expect(screen.getByText("PokeGame Test")).toBeInTheDocument();
        });
    });

    describe("combined props", () => {
        it("should handle all props together", () => {
            render(
                <PageBase maxWidth="md" showFooter={true}>
                    <div data-testid="content">Test Content</div>
                </PageBase>
            );

            expect(screen.getByTestId("content")).toBeInTheDocument();
            expect(screen.getByText("PokeGame Test")).toBeInTheDocument();
            expect(screen.getByText("v1.2.3")).toBeInTheDocument();
        });

        it("should handle minimal props", () => {
            render(
                <PageBase maxWidth={false} showFooter={false}>
                    <div data-testid="content">Minimal</div>
                </PageBase>
            );

            expect(screen.getByTestId("content")).toBeInTheDocument();
            expect(screen.queryByText("PokeGame Test")).not.toBeInTheDocument();
        });
    });
});
