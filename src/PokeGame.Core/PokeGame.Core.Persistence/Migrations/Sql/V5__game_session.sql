CREATE TABLE "public".game_session(
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    game_save_id UUID NOT NULL REFERENCES public.game_save(id) ON DELETE CASCADE ON UPDATE CASCADE UNIQUE,
    user_id UUID NOT NULL REFERENCES public."user"(id) ON DELETE CASCADE ON UPDATE CASCADE,
    connection_id TEXT NOT NULL UNIQUE,
    started_at TIMESTAMP WITHOUT TIME ZONE DEFAULT now()
);