using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace The_Evolution_Simulator
{
    public class Game
    {
        //=============
        // Constants
        public const int SEED = 0;
        public const int WINDOW_WIDTH = 800;
        public const int WINDOW_HEIGHT = 600;
        public const int MAP_WIDTH = 1500;
        public const int MAP_HEIGHT = 1500;
        public const int EXTRA_BOUNDARY = 10;
        public const int MOUSE_SIZE = 6;

        public const int GRAPH_UPDATE_RATE = 10;
        public const int GRAPH_MEMORY = 500; // Max 500

        public const int START_NUM_ANIMALS = 16;
        public const int START_NUM_PLANTS = 1500;

        public const int MAX_NUM_PLANTS = 2500;
        public const int CHANCE_NEW_PLANT = 2;

        //=============
        // Variables
        public MainWindow main;
        public GEngine gEngine;

        public List<Animal> animals = new List<Animal>();
        public List<Plant> plants = new List<Plant>();

        public List<int> numAnimals = new List<int>();
        public List<int> numPlants = new List<int>();

        public List<InfoText> infoText = new List<InfoText>();

        public Random r = new Random();
        public int step = 0;
        public int peakNumPlants = START_NUM_PLANTS;
        public int peakNumAnimals = START_NUM_ANIMALS * 4;

        public int cameraX, cameraY;
        public bool mouseDown = false;
        public int mouseX, mouseY = 0;
        public int oldMouseX, oldMouseY = 0;

        // Syllables for creating a name
        public string[] startSyllables =
        {
            "Ti", "Ri", "Be", "Per", "To", "Pro", "Ac",
            "Ad", "Ar", "Ers", "Ment", "Or", "At", "Man",
            "Bu", "Gro", "Ha", "Da", "Ka"
        };
        public string[] midSyllables =
        {
            "a", "ma", "ta", "gra", "la", "sha", "da", "na",
            "e", "eh", "eo", "ea", "em", "en", "er",
            "i", "ie", "ia", "ir", "il", "in",
            "o", "lo", "po", "so", "ro", "no", "mo", "co",
            "u", "ul", "un", "ur", "ui", "us",
            "'", "-", " "
        };
        public string[] endSyllables =
        {
            "ers", "ions", "ows", "ans","ens",
            "ments","uns", "ins", "mans", "als"
        };

        //=============
        // Functions

        // Starts game
        public void Init()
        {
            // Uses the preset seed if not 0
            if (SEED != 0)
                r = new Random(SEED);

            // Spawns in initial animals
            for (int i = 0; i < START_NUM_ANIMALS; i++)
                animals.Add(NewAnimal());

            // Spawns in initial plants
            for (int i = 0; i < START_NUM_PLANTS; i++)
                plants.Add(new Plant(
                    r.Next(MAP_WIDTH - EXTRA_BOUNDARY * 2) + EXTRA_BOUNDARY,
                    r.Next(MAP_HEIGHT - EXTRA_BOUNDARY * 2) + EXTRA_BOUNDARY,
                    r.Next(50, 151)));

            // Sets up numAnimals and numPlants lists
            for (int i = 0; i <= GRAPH_MEMORY; i++) {
                numAnimals.Add(0);
                numPlants.Add(0);
            }

            infoText.Add(new InfoText("The world has generated!"));
        }

        // Generates a random animal and returns it
        public Animal NewAnimal()
        {
            Animal newAnimal = new Animal();

            newAnimal.game = this;
            newAnimal.name = RandomName();
            newAnimal.posX = r.Next(MAP_WIDTH);
            newAnimal.posY = r.Next(MAP_HEIGHT);
            newAnimal.colorR = r.Next(255);
            newAnimal.colorG = r.Next(100);
            newAnimal.colorB = r.Next(255);

            newAnimal.startHealth = r.Next(200, 1000);
            newAnimal.speed = (r.Next(5, 15)) / 10f;
            newAnimal.size = r.Next(35, 55) / 10f;
            newAnimal.eggValueRequired = r.Next(5, 9);
            newAnimal.generation = 1;

            newAnimal.r = r;

            newAnimal.Init();

            return newAnimal;
        }

        // Generates a random name
        public string RandomName()
        {
            string name = startSyllables[r.Next(startSyllables.Length)];
            int size = r.Next(0, 3);

            for (int i = 0; i < size; i++)
                name += midSyllables[r.Next(midSyllables.Length)];

            name += endSyllables[r.Next(endSyllables.Length)];

            return name;
        }

        // Updates game
        public void Update()
        {
            step++;

            // Updates all the animals
            for (int i = 0; i < animals.Count; i++)
                animals[i].Update();

            // Spawn plants depending on random chance
            if (r.Next(CHANCE_NEW_PLANT) == 0 && plants.Count < MAX_NUM_PLANTS)
                //if (r.Next(MAX_NUM_PLANTS) > plants.Count) // More spawn if less available (reachs an equilibrium)
                    plants.Add(new Plant(
                        r.Next(MAP_WIDTH - EXTRA_BOUNDARY * 2) + EXTRA_BOUNDARY,
                        r.Next(MAP_HEIGHT - EXTRA_BOUNDARY * 2) + EXTRA_BOUNDARY,
                        r.Next(50, 151)));

            // Spawn animals if too low
            if (animals.Count < START_NUM_ANIMALS / 2)
            {
                animals.Add(NewAnimal());
                infoText.Add(new InfoText("The " + animals[2].name + " have appeared."));
            }

            // Add points to graph every 10 steps
            if (step % GRAPH_UPDATE_RATE == 0)
            {
                numAnimals.Add(animals.Count);
                numPlants.Add(plants.Count);
                numAnimals.RemoveAt(0);
                numPlants.RemoveAt(0);

                if (plants.Count > peakNumPlants)
                    peakNumPlants = plants.Count;

                if (animals.Count > peakNumAnimals)
                    peakNumAnimals = animals.Count;
            }

            // Removes excess messages
            while (infoText.Count > 5)
                infoText.RemoveAt(0);

            // Deals with drag movement
            mouseX = System.Windows.Forms.Cursor.Position.X;
            mouseY = System.Windows.Forms.Cursor.Position.Y;

            if (mouseDown)
                MoveCamera(mouseX - oldMouseX, mouseY - oldMouseY);

            oldMouseX = mouseX;
            oldMouseY = mouseY;
        }

        // Handles mouse input
        public void MouseClick(int x, int y)
        {
            mouseDown = true;
            bool noChange = true;

            // Display / Hide graph
            if (gEngine.graphDisplayed)
            {
                if (x > 100 && x < WINDOW_WIDTH - 100 && y > WINDOW_HEIGHT - 150)
                {
                    noChange = false;
                    gEngine.graphDisplayed = false;
                }
            }
            else
            {
                if (x > (WINDOW_WIDTH / 2) - 20 && x < (WINDOW_WIDTH / 2) + 20 && y > WINDOW_HEIGHT - 15)
                {
                    noChange = false;
                    gEngine.graphDisplayed = true;
                }
            }

            // If the graph wasnt interacted with
            if (noChange)
            {
                // Select animals
                bool selectionComplete = false;
                for (int i = 0; i < animals.Count; i++)
                {
                    if (Math.Abs(x - (animals[i].posX + cameraX)) < MOUSE_SIZE && Math.Abs(y - (animals[i].posY + cameraY)) < MOUSE_SIZE)
                    {
                        gEngine.animalSelected = animals[i];
                        selectionComplete = true;
                    }
                }

                // If no selection made
                if (!selectionComplete)
                    Task.Delay(100).ContinueWith(t => DeselectAnimal());
            }
        }

        public void DeselectAnimal()
        {
            if (!mouseDown)
                gEngine.animalSelected = null;
        }

        // Moves the camera pos
        public void MoveCamera(int x, int y)
        {
            cameraX += x;
            cameraY += y;
        }

        // End game safely
        public void Stop()
        {
            gEngine.renderThread.Abort();
        }
    }

    public class InfoText
    {
        public string text;
        public int lifetime = 100;

        public InfoText(string _text)
        {
            text = _text;
        }
    }
}
