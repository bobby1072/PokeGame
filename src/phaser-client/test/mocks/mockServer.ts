import { setupServer } from "msw/node";
import { mockHandlers } from "./mockHandlers";

export const mockServer = setupServer(...mockHandlers);
