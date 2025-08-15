export type BaseWebOutcome = {
    exceptionMessage?: string | null;
    extraData?: Record<string, any> | null;
    isSuccess: boolean;
};

export type WebOutcome<T> = {
    data?: T | null;
} & BaseWebOutcome;
