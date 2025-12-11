import { ReactElement } from "react";
import { RenderOptions, render } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

const queryClient = new QueryClient();
interface IWrapper {
  children: any;
}
const customRender = (ui: ReactElement, options?: RenderOptions) => {
  let wrapper: React.FC<IWrapper>;
  if (options?.wrapper) {
    const Wrapper = options.wrapper;
    wrapper = ({ children }) => (
      <Wrapper>
        <QueryClientProvider client={queryClient}>
          {children}
        </QueryClientProvider>
      </Wrapper>
    );
  } else {
    wrapper = ({ children }) => (
      <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    );
  }
  return render(ui, { ...options, wrapper });
};
export * from "@testing-library/react";
export { customRender as render };