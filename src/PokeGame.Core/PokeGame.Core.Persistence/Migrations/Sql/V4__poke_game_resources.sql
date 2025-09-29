CREATE TABLE "public".owned_pokemon(
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    game_save_id UUID NOT NULL REFERENCES public.game_save(id) ON DELETE CASCADE ON UPDATE CASCADE,
    resource_name TEXT NOT NULL,
    caught_at TIMESTAMP WITHOUT TIME ZONE DEFAULT now(),
    in_deck BOOLEAN DEFAULT FALSE,
    pokemon_level INT NOT NULL CHECK (pokemon_level <= 100),
    current_experience INT DEFAULT 0,
    current_hp INT NOT NULL,
    move_one_resource_name TEXT NOT NULL,
    move_two_resource_name TEXT,
    move_three_resource_name TEXT,
    move_four_resource_name TEXT
);


CREATE TABLE "public".item_stack(
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    game_save_id UUID NOT NULL REFERENCES public.game_save(id) ON DELETE CASCADE ON UPDATE CASCADE,
    resource_name TEXT NOT NULL,
    quantity INT NOT NULL CHECK (quantity <= 99)
);
