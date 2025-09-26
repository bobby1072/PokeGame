CREATE TABLE "public".game_save(
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES public."user"(id) ON DELETE CASCADE ON UPDATE CASCADE,
    character_name TEXT NOT NULL,
    date_created TIMESTAMP WITHOUT TIME ZONE DEFAULT now(),
    last_played TIMESTAMP WITHOUT TIME ZONE DEFAULT now()
);