using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonsOfDoom
{
    class Game //Spellogiken
    {
        Player player; //Klassvariabel - behövs ingen property eftersom den är private
        Room[,] world; //En array
        Random random = new Random();
        static int oldStrength = 0;
        const int defaultStrength = 15;

        public void Play() //Spelflödet
        {
            CreatePlayer();
            CreateWorld();

            do
            {
                Console.Clear();
                DisplayStats();
                DisplayWorld();
                AskForMovement();
            } while (player.Health > 0);

            GameOver();
        }

        private void DisplayStats()
        {
            Console.WriteLine($"Health: {player.Health}");
        }

        private void AskForMovement()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            int newX = player.X;
            int newY = player.Y;
            bool isValidMove = true; //Om spelaren håller sig inom rummets gränser

            switch (keyInfo.Key)
            {
                case ConsoleKey.RightArrow: newX++; break;
                case ConsoleKey.LeftArrow: newX--; break;
                case ConsoleKey.UpArrow: newY--; break;
                case ConsoleKey.DownArrow: newY++; break;
                default: isValidMove = false; break;
            }

            if (isValidMove &&
                newX >= 0 && newX < world.GetLength(0) &&
                newY >= 0 && newY < world.GetLength(1))
            {
                player.X = newX;
                player.Y = newY;

                Room r = world[player.X, player.Y];
                BackpackCollector(r);

                if (r.Monster != null)
                {
                    Console.WriteLine($"You have met a {r.Monster.CharacterName}.");
                    Console.WriteLine($"Select an item");
                    BackPackInventory();
                    string itemChoice = Console.ReadLine().ToLower();
                    bool weaponIsValid = false;

                    switch (itemChoice)
                    {
                        case "k":
                            //Hur välja knife? 

                            foreach (var item in player.BackPack)
                            {
                                if (item.Name == "Knife")
                                {
                                    Console.WriteLine($"{item.Name} is equiped");
                                    
                                    player.Strength = player.Strength + item.ExtraStrength;
                                    player.BackPack.Remove(item);

                                    weaponIsValid = true;
                                    break;
                                }
                                
                            }
                            if (weaponIsValid == false)
                            {
                                Console.WriteLine("You can not find you knife. /n Lets use our fists!");
                            }
                            
                            break;
                        case "s":
                            break;
                        case "b":
                            break;
                        default:
                            Console.WriteLine("You are a real warrior using your hands!");
                            break;

                    }


                    Console.ReadLine();
                    while (player.Health > 0 && r.Monster.Health > 0)
                    {
                        player.Fight(r.Monster);
                        Console.WriteLine($"You hit {r.Monster.CharacterName} with {player.Strength} BP");
                        Console.WriteLine($"{r.Monster.CharacterName} health: {r.Monster.Health} HP.");
                        Console.ReadLine();


                        if (r.Monster.Health > 0)
                        {
                            r.Monster.Fight(player);
                            Console.WriteLine($"{r.Monster.CharacterName} hit you with {r.Monster.Strength} BP.");
                            Console.WriteLine($"{player.CharacterName} health: {player.Health} HP.");
                            Console.ReadLine();
                        }
                    }

                    if (r.Monster.Health <= 0)
                    {
                        Console.WriteLine($"You have slayed {r.Monster.CharacterName}, you are Victorius!!!");
                        Console.ReadLine();
                        player.Strength = random.Next(5,defaultStrength);
                        r.Monster = null; // Monstret har dött!
                    }
                }

            }
        }



        private void BackpackCollector(Room r)
        {
            if (r.Item != null)
            {
                Console.WriteLine("");
                Console.WriteLine("==============================");
                Console.WriteLine($"You have found a {r.Item.Name}.");
                Console.WriteLine($"Press [y] to add the {r.Item.Name} into your backpack ");
                Console.WriteLine($"or [n] to leave it.");

                string choice = (Console.ReadLine()).ToLower();
                if (choice == "y")
                {
                    player.BackPack.Add(r.Item);
                    BackPackInventory();
                    Console.WriteLine("Press [enter] to continue your adventure!");
                    Console.ReadLine();

                    r.Item = null;
                }
            }
        }

        private void BackPackInventory()
        {
            Console.WriteLine("BACKPACK INVENTORY:");
            foreach (var item in player.BackPack)
            {
                Console.WriteLine($"- {item.Name}");
            }
        }

        private void DisplayWorld()
        {
            for (int y = 0; y < world.GetLength(1); y++)
            {
                for (int x = 0; x < world.GetLength(0); x++)
                {
                    Room room = world[x, y]; //Pekar på ett rum som inte innehållar några items eller monster, if-satserna skriver ut dem
                    if (player.X == x && player.Y == y)
                        Console.Write("P");
                    else if (room.Monster != null)
                        Console.Write("M");
                    else if (room.Item != null)
                        Console.Write(room.Item.Name.First());
                    else
                        Console.Write(".");
                }
                Console.WriteLine();
            }
        }

        private void GameOver()
        {
            Console.Clear();
            Console.WriteLine("==========================================================================");
            Console.WriteLine("==========================================================================");
            Console.WriteLine("=====   ====  ===   ===   ========   =========         ===        ========");
            Console.WriteLine("=====   ==  =====   ===   ========   =========   =========   ====   ======");
            Console.WriteLine("=====     =======   ===   ========   =========   =========   =====   =====");
            Console.WriteLine("=====   =========   ===   ========   =========       =====   =====   =====");
            Console.WriteLine("=====     =======   ===   ========   =========   =========   =====   =====");
            Console.WriteLine("=====   ==  =====   ===   ========   =========   =========   ====   ======");
            Console.WriteLine("=====   ====  ===   ===        ===         ===         ===        ========");
            Console.WriteLine("==========================================================================");
            Console.WriteLine("==========================================================================");
            Console.ReadKey();
            Play();
        }

        private void CreateWorld()
        {
            world = new Room[20, 5]; //En array med plats för 20 * 5 rum
            for (int y = 0; y < world.GetLength(1); y++)
            {
                for (int x = 0; x < world.GetLength(0); x++)
                {
                    world[x, y] = new Room(); //Anropar konstruktorn i klassen Room

                    if (player.X != x || player.Y != y)
                    {
                        int i = random.Next(0, 100);
                        if (i < 10)
                        {
                            if (i < 7) { world[x, y].Monster = new Skeleton(30, random.Next(1, 15), "Skeletor"); }
                            else { world[x, y].Monster = new TheHippo(60, random.Next(5, 31), "Hippoletto"); }
                        }

                        else if (i >= 10 && i < 20)
                            world[x, y].Item = Weapon.GenerateRandom(random);

                    }
                }
            }
        }

        private void CreatePlayer()
        {
            player = new Player(30, random.Next(5, defaultStrength), 0, 0, "Håkan");
        }


    }
}
