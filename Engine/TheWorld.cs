using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class TheWorld
    {
        public static readonly List<Item> Items = new List<Item>();
        public static readonly List<Monster> Monsters = new List<Monster>();
        public static readonly List<Quest> Quests = new List<Quest>();
        public static readonly List<Location> Locations = new List<Location>();
        public static readonly List<Character> Characters = new List<Character>();

        public const int ITEM_ID_SWORD = 1;
        public const int ITEM_ID_RAT_TAIL = 2;
        public const int ITEM_ID_RAT_SKIN = 3;
        public const int ITEM_ID_SNAKE_TOOTH= 4;
        public const int ITEM_ID_SNAKE_SKIN = 5;
        public const int ITEM_ID_KNIFE = 6;
        public const int ITEM_ID_HEALING_POTION = 7;
        public const int ITEM_ID_SPIDER_LEG = 8;
        public const int ITEM_ID_SPIDER_EYE = 9;
        public const int ITEM_ID_METHAMPHETAMINE = 10;

        public const int MONSTER_ID_RAT = 1;
        public const int MONSTER_ID_SNAKE = 2;
        public const int MONSTER_ID_SPIDER = 3;

        public const int QUEST_ID_CLEAR_ALCHEMIST_GARDEN = 1;
        public const int QUEST_ID_CLEAR_FARMERS_FIELD = 2;
        public const int QUEST_ID_CLEAR_SPIDER_FIELD = 3;

        public const int LOCATION_ID_HOME = 1;
        public const int LOCATION_ID_TOWN_SQUARE = 2;
        public const int LOCATION_ID_GUARD_POST = 3;
        public const int LOCATION_ID_ALCHEMIST_HUT = 4;
        public const int LOCATION_ID_ALCHEMISTS_GARDEN = 5;
        public const int LOCATION_ID_FARMHOUSE = 6;
        public const int LOCATION_ID_FARM_FIELD = 7;
        public const int LOCATION_ID_BRIDGE = 8;
        public const int LOCATION_ID_SPIDER_FIELD = 9;

        public const int CHARACTER_ID_GUIDER = 1;
        public const int CHARACTER_ID_ALCHEMIST = 2;
        public const int CHARACTER_ID_FARMER = 3;
        public const int CHARACTER_ID_GUARDER = 4;

        static TheWorld()
        {
            FillItems();
            FillMonsters();
            FillQuests();
            FillCharacters();
            FillLocations();
        }

        private static void FillItems()
        {
            Items.Add(new Weapon(ITEM_ID_SWORD, "Sword", 15, 30));
            Items.Add(new Weapon(ITEM_ID_KNIFE, "Knife", 10, 20));
            Items.Add(new HealingPotion(ITEM_ID_HEALING_POTION, "Healing potion", 40));
            Items.Add(new Item(ITEM_ID_RAT_TAIL, "Rat tail"));
            Items.Add(new Item(ITEM_ID_RAT_SKIN, "Rat skin"));
            Items.Add(new Item(ITEM_ID_SNAKE_TOOTH, "Snake tooth"));
            Items.Add(new Item(ITEM_ID_SNAKE_SKIN, "Snake skin"));
            Items.Add(new Item(ITEM_ID_SPIDER_EYE, "Spider eye"));
            Items.Add(new Item(ITEM_ID_SPIDER_LEG, "Spider leg"));
            Items.Add(new HealingPotion(ITEM_ID_METHAMPHETAMINE, "Methamphetamine", 999));
        }

        private static void FillMonsters() 
        {
            Monster rat = new Monster(40, 40, MONSTER_ID_RAT, "Rat", 5, 1, 3);
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_RAT_TAIL), 50, false));
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_RAT_SKIN), 75, true));

            Monster snake = new Monster(60, 60, MONSTER_ID_SNAKE, "Snake", 10, 2, 5);
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKE_TOOTH), 50, false));
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKE_SKIN), 75, true));

            Monster spider = new Monster(75, 75, MONSTER_ID_SPIDER, "Spider", 15, 3, 10);
            spider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_EYE), 50, false));
            spider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_LEG), 75, true));

            Monsters.Add(rat);
            Monsters.Add(snake);
            Monsters.Add(spider);
        }  

        private static void FillQuests()
        {
            Quest clearAlchemistGarden =
                new Quest(
                    QUEST_ID_CLEAR_ALCHEMIST_GARDEN,
                    "Clear the alchemist's garden",
                    "Kill rats in the alchemist's garden and bring back 3 snake tooths. You will receive a healing potion and 10 gold.",
                    5, 10, ItemByID(ITEM_ID_HEALING_POTION));
            clearAlchemistGarden.QuestCompletionItems.Add(ItemByID(ITEM_ID_SNAKE_TOOTH), 3);

            Quest clearFarmresField =
                new Quest(
                    QUEST_ID_CLEAR_FARMERS_FIELD,
                    "Clear the farmer's field",
                    "Kill snakes in the farmer's field and bring back 3 rat tails. You will receive an sword and 20 gold pieces.",
                    10, 20, ItemByID(ITEM_ID_SWORD));
            clearFarmresField.QuestCompletionItems.Add(ItemByID(ITEM_ID_RAT_TAIL), 3);

            Quest clearSpiderField =
                new Quest(
                    QUEST_ID_CLEAR_SPIDER_FIELD,
                    "Clear the spider's field",
                    "Kill spiders in the spiders's field and bring back 3 spider eye. You will receive an healing potion and 30 gold pieces.",
                    20, 40, ItemByID(ITEM_ID_METHAMPHETAMINE));
            clearSpiderField.QuestCompletionItems.Add(ItemByID(ITEM_ID_SPIDER_EYE), 3);

            Quests.Add(clearAlchemistGarden);
            Quests.Add(clearFarmresField);
            Quests.Add(clearSpiderField);
        }

        private static void FillLocations()
        {
            // Create each location
            Location home = new Location(LOCATION_ID_HOME, "Home", "Your house. You really need place to rest.",
                new string[]
                {
"|-------------------------------[           ]--------------------------|",
"|                                                                      |",
"|   ######                                      ###### |",
"|   #          #                                     #         #  |",
"|   #          #                                     #         #  |",
"|   ######                                     ######  |",
"|                                  ccc                 _____   |",
"|   ____                      ( .-. )                 |poooo|  |",
"|   |       |                     /(__)\\               |ooooo|  |",
"|   |       |                       |    |                 |----------|  |",
"|                                                         |          |  |",
"|                                                         |----------|  |",
"|----------------------------------------------------------------------|"
                }                
                );

            Location townSquare = new Location(LOCATION_ID_TOWN_SQUARE, "Town square", "You see a fountain, and a character Guider.",
                new string[]
                {
"|-----------------------------[           ]--------------------------|",
"|            .,.,.                                     ⏛⏛       |",
"|        .  /* |*\\   ,,                              (o_o)       |",
"|     ___|   |   |___                           /(000)\\     |",
"|     \\____|____/                            _|    |_      |",
"‾                                                                   ‾",
"                                 ccc                                 ",
"_                               ( .-. )                              _",
"|           O O              /(__)\\              OOO       |",
"|        O O  OO            |    |            OO OO       |",
"|     O   O O  OO                          OOO OO     |",
"|     O  O O  OOO O                OO OOO OOO |",
"|------------------------------[            ]--------------------------|"
                }
                );
            townSquare.CharacterLivingHere = CharacterByID(CHARACTER_ID_GUIDER);
            townSquare.LocationOverviewDialogue = new string[]
            {
"|-----------------------------[           ]--------------------------|",
"|            .,.,.                          ccc     ⏛⏛       |",
"|        .  /* |*\\   ,,                   ( .-. )    (o_o)       |",
"|     ___|   |   |___                /(__)\\  /(000)\\     |",
"|     \\____|____/                  |    |     _|    |_      |",
"‾                                                                   ‾",
"                                                                       ",
"_                                                                    _",
"|           O O                                       OOO     |",
"|        O O  OO                               OO OO     |",
"|     O   O O  OO                          OOO OO     |",
"|     O  O O  OOO O                OO OOO OOO |",
"|------------------------------[            ]--------------------------|"
            };

            Location alchemistHut = new Location(LOCATION_ID_ALCHEMIST_HUT, "Alchemist's hut", "There are many strange plants on the shelves. And a character Alchemist.",
                new string[]
                {
"|---------------------------[           ]------------------------------|",
"|      /‾‾\\                                             ⚓⚓      |",
"|    /        \\                                         (ಠ_ಠ)    |",
"|  /            \\                                      /(🕇 🕇)\\   |",
"| |‾‾‾‾‾‾‾‾|                                        |‾‾|      |",
"| |‾‾‾|     |‾‾|                                                   |",
"| |___|     |    |            ccc                 |‾‾‾‾‾‾‾| |",
"| |______|__|           ( .-. )                 |   🌱       | |",
"|                             /(__)\\                |  🌱  🍀  | |",
"|                               |    |                  |  🌱  🌱  | |",
"|‾‾‾\\                                               | 🌱    🌱 | |",
"|♨ ♨ \\__                                         |_______| |",
"|--------------|-------------[             ]----------------------------|"
                }
                );
            alchemistHut.CharacterLivingHere = CharacterByID(CHARACTER_ID_ALCHEMIST);
            alchemistHut.LocationOverviewDialogue = new string[]
            {
"|---------------------------[           ]------------------------------|",
"|      /‾‾\\                               ccc        ⚓⚓      |",
"|    /        \\                             ( .-. )     (ಠ_ಠ)    |",
"|  /            \\                          /(__)\\  /(🕇 🕇)\\   |",
"| |‾‾‾‾‾‾‾‾|                           |    |       |‾‾|       |",
"| |‾‾‾|     |‾‾|                                                   |",
"| |___|     |    |                                   |‾‾‾‾‾‾‾| |",
"| |______|__|                                   |   🌱       | |",
"|                                                      |  🌱  🍀   | |",
"|                                                      |  🌱  🌱   | |",
"|‾‾‾\\                                              | 🌱    🌱  | |",
"|♨ ♨ \\__                                        |_______| |",
"|--------------|-------------[             ]----------------------------|"
            };

            Location alchemistsGarden = new Location(LOCATION_ID_ALCHEMISTS_GARDEN, "Alchemist's garden", "Many plants are growing here.",
                new string[]
                {
"|----------------------------------------------------------------------|",
"|      🌷                    🌷                                 🌿|",
"|               🌿               🌿         🐍         🌷       |",
"|       🐍                                    🌷                    |",
"|          🌿        🐍    🌷     🐍                🌿         |",
"|                                          🐍                         |",
"|        🌷                       ccc               🐍           |",
"|              🌷         🌿    ( .-. )              🌷            |",
"|       🐍                       /(__)\\                            |",
"|      🌿                          |    |          🐍                |",
"|                🌿 🐍                      🌿   🌷              |",
"|                           🌿                               🌿     |",
"|---------------------------[              ]----------------------------|"
                }
                );
            alchemistsGarden.MonsterLivingHere = MonsterByID(MONSTER_ID_SNAKE);
            alchemistsGarden.LocationOverviewFight = new string[]
            {
"|----------------------------------------------------------------------|",
"|      🌷                    🌷                                 🌿|",
"|               🌿               🌿         🐍         🌷       |",
"|       🐍                                    🌷                    |",
"|          🌿        🐍    🌷     🐍                🌿         |",
"|                                          🐍                         |",
"|        🌷                       ccc               🐍           |",
"|              🌷    🌿 🐍  | ( .-. )              🌷            |",
"|       🐍                      |/(__)\\                            |",
"|      🌿                          |    |          🐍                |",
"|                🌿 🐍                      🌿   🌷              |",
"|                           🌿                               🌿     |",
"|---------------------------[              ]----------------------------|"
            };

            Location farmhouse = new Location(LOCATION_ID_FARMHOUSE, "Farmhouse", "There is a small farmhouse, with a farmer in front.",
                new string[]
                {
"|----------------------------------------------------------------------|",
"|    /‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾\\           /‾‾\\    |",
"|    |‾‾‾‾|‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾|          ‾‾|‾‾   |",
"|    |        |                                  |          |‾‾‾‾|   |",
"|    |____|_________________|          |____|   |",
"‾                                                                     ‾",
"                              ccc                                    ",
"_                            ( .-. )                                  _",
"|      👞         🐑      /(__)\\   🐄    🐑    🐖    🐖   |",
"|      😐                     |    |     🐖     🐑         🐖    |",
"|    /(⚿)\\        🐑                        🐖      🐖        |",
"|       |  |                    🐑         🐖        🐑     🐖   |",
"|----------------------------------------------------------------------|"
                }
                );
            farmhouse.CharacterLivingHere = CharacterByID(CHARACTER_ID_FARMER);
            farmhouse.LocationOverviewDialogue = new string[]
            {
"|----------------------------------------------------------------------|",
"|    /‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾\\           /‾‾\\    |",
"|    |‾‾‾‾|‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾|          ‾‾|‾‾   |",
"|    |        |                                  |          |‾‾‾‾|   |",
"|    |____|_________________|          |____|   |",
"‾                                                                    ‾",
"                                                                    ",
"_                                                                    _",
"|      👞       ccc                 🐄    🐑    🐖    🐖   |",
"|      😐       ( .-. )                 🐖     🐑         🐖    |",
"|    /(⚿)\\   /(__)\\            🐑       🐖      🐖        |",
"|       |  |        |    |                   🐖        🐑     🐖   |",
"|----------------------------------------------------------------------|"
            };

            Location farmersField = new Location(LOCATION_ID_FARM_FIELD, "Farmer's field", "You see rows of vegetables growing here.",
                new string[]
                {
"|--------------------------------------------------------------------|",
"|           🌾                   🐀           🌽                |",
"|                     ⸙       🐀            🌾      🐀        |",
"|              🐀                      🐀      🌾       🌽    |",
"|         🌾        🌾           ⸙                              |",
"|            🌽               🐀                                  ‾",
"|                    ⸙           ccc             🐀                    ",
"|          🐀          🐀      ( .-. )                             _",
"|                                /(__)\\            ⸙             |",
"|       🌽     🌾               |    |          🐀                |",
"|              🐀         🌾                      🌾   🌽       |",
"|                            🐀       🌽       🐀                 |",
"|----------------------------------------------------------------------|"
                }
                );
            farmersField.MonsterLivingHere = MonsterByID(MONSTER_ID_RAT);
            farmersField.LocationOverviewFight = new string[]
            {
"|--------------------------------------------------------------------|",
"|           🌾                   🐀           🌽                |",
"|                     ⸙       🐀            🌾      🐀        |",
"|              🐀                      🐀      🌾       🌽    |",
"|         🌾        🌾           ⸙                              |",
"|            🌽               🐀                                  ‾",
"|                    ⸙           ccc             🐀                    ",
"|          🐀          🐀   |  ( .-. )                             _",
"|                               |/(__)\\            ⸙             |",
"|       🌽     🌾               |    |          🐀                |",
"|              🐀         🌾                      🌾   🌽       |",
"|                            🐀       🌽       🐀                 |",
"|----------------------------------------------------------------------|"
            };

            Location guardPost = new Location(LOCATION_ID_GUARD_POST, "Guard post", "There is a large, tough-looking guarder here.",
                new string[]
                {
"|----------------------------------------------------------------------|",
"‾      ccc                         🖶              /‾‾‾‾\\   |",
"        ( .-. )                    |   (-_-)            |‾‾‾‾‾|   |",
"_     /(__)\\                   |/(✗ ✗)\\        |     |‾‾|   |",
"|        |    |                      |  🁣 🁣          |     |    |   |",
"|                                                       ‾‾‾‾‾     |",
"|‾‾‾‾‾‾‾‾‾‾‾‾‾|‾‾‾‾‾‾‾|‾‾‾‾‾‾‾‾‾‾‾‾‾‾|      ",
"|_____________|_______|______________|",
"|          🕱                        🕱                    🕱        |",
"|     🕱                   🕱                     🕱                 ‾",
"|             🕱                 🕱                   🕱                ",
"|      🕱                  🕱        🕱       🕱            🕱      _",
"|---------------------------------------------------------------------|"
                }
                );
            guardPost.CharacterLivingHere = CharacterByID(CHARACTER_ID_GUARDER);
            guardPost.LocationOverviewDialogue = new string[]
            {
"|----------------------------------------------------------------------|",
"‾                     ccc          🖶              /‾‾‾‾\\   |",
"                       ( .-. )     |   (-_-)            |‾‾‾‾‾|   |",
"_                    /(__)\\    |/(✗ ✗)\\        |     |‾‾|   |",
"|                       |    |       |  🁣 🁣          |     |    |   |",
"|                                                       ‾‾‾‾‾     |",
"|‾‾‾‾‾‾‾‾‾‾‾‾‾|‾‾‾‾‾‾‾|‾‾‾‾‾‾‾‾‾‾‾‾‾‾|      ",
"|_____________|_______|______________|",
"|          🕱                        🕱                    🕱        |",
"|     🕱                   🕱                     🕱                 ‾",
"|             🕱                 🕱                   🕱                ",
"|      🕱                  🕱        🕱       🕱            🕱      _",
"|---------------------------------------------------------------------|"
            };

            Location bridge = new Location(LOCATION_ID_BRIDGE, "Bridge", "A stone bridge crosses a wide river.",
                new string[]
                {
"|------------|--------------------------------\\------------------------|",
"|             \\      ⸯ             ⸯ   ⸯⸯ      \\             🐸    |",
"|    🌳       \\           ⸯ   ⸯ          ⸯ     |        🐸        |",
"|                 \\       ⸯ           ⸯ     ⸯ   |     🐸     🐸   |",
"|       🌳        \\  ⸯ        ccc         /     🐸             |",
"|            ____|_____( .-. )____|_____     🐸     |",
"|          /                  /(__)\\                  \\     🐸   |",
"|        /                      |    |                       \\         |",
"|      /        ___________________         \\      |",
"‾‾‾        /      |                 ⸯ        |      \\        ‾‾‾",
"            /        |        ⸯ                 |  🌳  \\          ",
"_____/   🌳   |               ⸯ          |          \\______",
"|---------------------|--------------------------|-----------------------|"
                }
                );

            Location spiderField = new Location(LOCATION_ID_SPIDER_FIELD, "Forest", "You see spiders webs covering covering the trees in this forest.",
                new string[]
                {
"|----------------------------------------------------------------------|",
"|             🕸                           🕸           🕷        |",
"|   🕸                  🕸        🕷              🕸            |",
"|                 🕷                   🕸             🕷          |",
"|       🕸         🕸                         🕸                  |",
"|                                🕸                                  |",
"|   🕸              🕷      ccc                 🕷  🕸       |",
"|               🕸           ( .-. )         🕸                     |",
"|          🕷               /(__)\\             🕸     🕸       |",
"‾             🕸             |    |           🕷                   |",
"       🕸              🕷                      🕸                 |",
"_                    🕸                    🕷🕸                  |",
"|----------------------------------------------------------------------|"
                }
                );
            spiderField.MonsterLivingHere = MonsterByID(MONSTER_ID_SPIDER);
            spiderField.LocationOverviewFight = new string[]
            {
"|----------------------------------------------------------------------|",
"|             🕸                           🕸           🕷        |",
"|   🕸                  🕸        🕷              🕸            |",
"|                 🕷                   🕸             🕷          |",
"|       🕸         🕸                         🕸                  |",
"|                                🕸                                  |",
"|   🕸              🕷      ccc                 🕷  🕸       |",
"|               🕸           ( .-. )  | 🕷    🕸                  |",
"|          🕷               /(__)\\|            🕸     🕸       |",
"‾             🕸             |    |           🕷                   |",
"       🕸              🕷                      🕸                 |",
"_                    🕸                    🕷🕸                  |",
"|----------------------------------------------------------------------|"
            };

            // Link the locations together
            home.LocationToNorth = townSquare;

            townSquare.LocationToNorth = alchemistHut;
            townSquare.LocationToSouth = home;
            townSquare.LocationToEast = guardPost;
            townSquare.LocationToWest = farmhouse;

            farmhouse.LocationToEast = townSquare;
            farmhouse.LocationToWest = farmersField;

            farmersField.LocationToEast = farmhouse;

            alchemistHut.LocationToSouth = townSquare;
            alchemistHut.LocationToNorth = alchemistsGarden;

            alchemistsGarden.LocationToSouth = alchemistHut;

            guardPost.LocationToEast = bridge;
            guardPost.LocationToWest = townSquare;

            bridge.LocationToWest = guardPost;
            bridge.LocationToEast = spiderField;

            spiderField.LocationToWest = bridge;

            // Add the locations to the static list
            Locations.Add(home);
            Locations.Add(townSquare);
            Locations.Add(guardPost);
            Locations.Add(alchemistHut);
            Locations.Add(alchemistsGarden);
            Locations.Add(farmhouse);
            Locations.Add(farmersField);
            Locations.Add(bridge);
            Locations.Add(spiderField);
        }

        private static void FillCharacters()
        {
            Character Guider = new Character(CHARACTER_ID_GUIDER, "Speedwagon", "This is guider. He tell you about this world.");

            Character Alchemist = new Character(CHARACTER_ID_ALCHEMIST, "Edward", "This is alchemist. He Silent, but can give a quest.");
            Alchemist.QuestAvailable = QuestByID(QUEST_ID_CLEAR_ALCHEMIST_GARDEN);

            Character Farmer = new Character(CHARACTER_ID_FARMER, "Sasha", "This is farmer. He tell you about farm and give a quest.");
            Farmer.QuestAvailable = QuestByID(QUEST_ID_CLEAR_FARMERS_FIELD);

            Character Guarder = new Character(CHARACTER_ID_GUARDER, "Adam", "This is guarder. He tell you about spider field and give a quest.");
            Guarder.QuestAvailable = QuestByID(QUEST_ID_CLEAR_SPIDER_FIELD);

            Characters.Add(Guider);
            Characters.Add(Alchemist);
            Characters.Add(Farmer);
            Characters.Add(Guarder);
        }

        public static Item ItemByID(int id)
        {
            foreach (Item item in Items)
            {
                if(item.ID==id)
                {
                    return item;
                }
            }
            return null;
        }

        public static Monster MonsterByID(int id)
        {
            foreach (Monster monster in Monsters)
            {
                if(monster.ID == id)
                {
                    return monster;
                }
            }
            return null;
        }

        public static Quest QuestByID(int id)
        {
            foreach(Quest quest in Quests)
            {
                if(quest.ID == id)
                {
                    return quest;
                }
            }
            return null;
        }

        public static Location LocationByID(int id)
        {
            foreach (Location location in Locations)
            {
                if(location.ID == id)
                {
                    return location;
                }
            }
            return null;
        }

        public static Character CharacterByID(int id)
        {
            foreach(Character character in Characters)
            {
                if(character.ID == id)
                {
                    return character;
                }
            }
            return null;
        }
    }
}
