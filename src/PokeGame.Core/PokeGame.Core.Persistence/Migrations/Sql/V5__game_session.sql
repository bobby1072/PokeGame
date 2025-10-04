CREATE TABLE "public".game_session(
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    game_save_id UUID NOT NULL REFERENCES public.game_save(id) ON DELETE CASCADE ON UPDATE CASCADE,
    user_id UUID NOT NULL REFERENCES public."user"(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_game_session_user_match CHECK (
        user_id = (SELECT user_id FROM public.game_save WHERE id = game_save_id)
    )
);