CREATE TABLE "public".game_session(
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    game_save_id UUID NOT NULL REFERENCES public.game_save(id) ON DELETE CASCADE ON UPDATE CASCADE,
    user_id UUID NOT NULL REFERENCES public."user"(id) ON DELETE CASCADE ON UPDATE CASCADE,
    connection_id TEXT NOT NULL,
    started_at TIMESTAMP WITHOUT TIME ZONE DEFAULT now()
);