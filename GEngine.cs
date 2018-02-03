using System;
using System.Drawing;
using System.Threading;

namespace The_Evolution_Simulator
{
    public class GEngine
    {
        //============
        // References
        public MainWindow main;
        public Game game;

        //============
        // Variables
        public Graphics g;
        public Thread renderThread;
        public Animal animalSelected;
        public SolidBrush plantColor = new SolidBrush(Color.DarkGreen);

        // UI colors
        Font font = new Font(new FontFamily("Arial"), 10);
        SolidBrush fontColor = new SolidBrush(Color.LightGray);
        SolidBrush fontColorDark = new SolidBrush(Color.Black);
        SolidBrush fontColorPlant = new SolidBrush(Color.Green);
        SolidBrush fontColorAnimal = new SolidBrush(Color.Red);
        SolidBrush UIColor = new SolidBrush(Color.Gray);
        SolidBrush UIBorderColor = new SolidBrush(Color.DarkGray);
        SolidBrush HighlightColor = new SolidBrush(Color.FromArgb(50, Color.White));
        Pen UILineColor = new Pen(new SolidBrush(Color.Black), 2);

        Pen UIPlantLineColor = new Pen(new SolidBrush(Color.Green), 2);
        Pen UIAnimalLineColor = new Pen(new SolidBrush(Color.Red), 2);

        public bool graphDisplayed;

        //============
        // Functions

        // Sets up graphics
        public void Init()
        {
            // Starts rendering frames
            renderThread = new Thread(new ThreadStart(Render));
            renderThread.Start();
        }

        // Draws frame to graphics
        public void Render()
        {
            Bitmap frame = new Bitmap(Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT);
            Graphics frameGraphics = Graphics.FromImage(frame);

            // Repeats forever
            while (true)
            {
                // Draw background
                frameGraphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT);

                // Draw ground
                frameGraphics.FillRectangle(new SolidBrush(Color.Green), game.cameraX, game.cameraY, Game.MAP_WIDTH, Game.MAP_HEIGHT);

                // Draw plants
                for (int i = 0; i < game.plants.Count; i++)
                {
                    Plant p = game.plants[i];
                    frameGraphics.FillRectangle(
                        plantColor, // Colour
                        (p.posX - (p.size / 50)) + game.cameraX, (p.posY - (p.size / 50)) + game.cameraY, // Actual Pos
                        p.size / 25, p.size / 25); // Size
                }

                // Draw animals
                for (int i = 0; i < game.animals.Count; i++)
                {
                    Animal a = game.animals[i];
                    frameGraphics.FillRectangle(
                        new SolidBrush(Color.FromArgb(a.colorR, a.colorG, a.colorB)), // Colour
                        (a.posX - a.size/2) + game.cameraX, (a.posY - a.size/2) + game.cameraY, // Actual Pos
                        a.size, a.size); // Size
                }

                // UI
                DrawUI(frameGraphics);

                // Draw entire frame
                g.DrawImage(frame, 0, 0);
            }
        }

        // Draws relevent UI to frame
        public void DrawUI(Graphics f)
        {
            // Highlight selected animal ----------------------
            if (animalSelected != null)
            {
                // Draws highlight
                f.FillEllipse(
                    HighlightColor,
                    (animalSelected.posX - 20) + game.cameraX,
                    (animalSelected.posY - 20) + game.cameraY,
                    40, 40);

                // Redraw animal ontop of highlight
                f.FillRectangle(
                   new SolidBrush(Color.FromArgb(animalSelected.colorR, animalSelected.colorG, animalSelected.colorB)), // Colour
                    (animalSelected.posX - animalSelected.size / 2) + game.cameraX, (animalSelected.posY - animalSelected.size / 2) + game.cameraY, // Actual Pos
                    animalSelected.size, animalSelected.size); // Size
            }

            // World info --------------------------------------
            DrawBorderedPanel(f, 5, 5, 120, 100, 2);
            f.DrawString(
                "Step: " + game.step,
                font, fontColor, 10, 10);
            f.DrawString(
                "Animals: " + game.animals.Count,
                font, fontColor, 10, 30);
            f.DrawString(
                "Plants: " + game.plants.Count,
                font, fontColor, 10, 45);

            if (Game.SEED != 0)
                f.DrawString(
                    "Seed: " + Game.SEED,
                    font, fontColor, 10, 70);
            else
                f.DrawString(
                    "Seed: Random",
                    font, fontColor, 10, 70);

            f.DrawString(
                "X" + (-game.cameraX) + " Y" + (-game.cameraY),
                font, fontColor, 10, 85);

            // Selected animal info
            if (animalSelected != null)
            {
                DrawBorderedPanel(f, 5, 120, 200, 250, 2);
                f.DrawString(
                    "Species: " + animalSelected.name,
                    font, fontColor, 10, 130);
                f.DrawString(
                    "Age: " + animalSelected.age,
                    font, fontColor, 10, 150);
                f.DrawString(
                    "Health: " + animalSelected.health + " (" + animalSelected.startHealth + ")",
                    font, fontColor, 10, 170);
                f.DrawString(
                    "Speed: " + animalSelected.speed,
                    font, fontColor, 10, 200);
                f.DrawString(
                    "Size: " + animalSelected.size,
                    font, fontColor, 10, 220);
                f.DrawString(
                    "Color: R" + animalSelected.colorR + " G" + animalSelected.colorG + " B" + animalSelected.colorB,
                    font, fontColor, 10, 240);
                f.DrawString(
                    "Reproduction Cycle: " + animalSelected.eggValue + "/" + animalSelected.eggValueRequired,
                    font, fontColor, 10, 260);
                f.DrawString(
                    "Food Eaten: " + animalSelected.foodEaten,
                    font, fontColor, 10, 290);
                f.DrawString(
                    "Children: " + animalSelected.childrenMade,
                    font, fontColor, 10, 310);
                f.DrawString(
                    "Generation: " + animalSelected.generation,
                    font, fontColor, 10, 330);
            }

            // Graph ------------------------------------------
            if (graphDisplayed)
            {
                // Graph UI
                DrawBorderedPanel(f, 100, Game.WINDOW_HEIGHT - 150, 600, 160, 2);
                f.FillRectangle(UIBorderColor, 110, Game.WINDOW_HEIGHT - 140, 580, 130);

                // Actual graph
                if (game.plants.Count != 0)
                    for (int i = 1; i < game.numPlants.Count; i++)
                        f.DrawLine(
                            UIPlantLineColor,
                            ((550 / game.numPlants.Count) * (i - 1)) + 130,
                            (Game.WINDOW_HEIGHT - 20) - (game.numPlants[i - 1] / (game.peakNumPlants / 100f)),
                            ((550 / game.numPlants.Count) * i) + 130,
                            (Game.WINDOW_HEIGHT - 20) - (game.numPlants[i] / (game.peakNumPlants / 100f)));

                if (game.animals.Count != 0)
                    for (int i = 1; i < game.numAnimals.Count; i++)
                        f.DrawLine(
                            UIAnimalLineColor,
                            ((550 / game.numAnimals.Count) * (i - 1)) + 130,
                            (Game.WINDOW_HEIGHT - 20) - (game.numAnimals[i - 1] / (game.peakNumAnimals / 100f)),
                            ((550 / game.numAnimals.Count) * i) + 130,
                            (Game.WINDOW_HEIGHT - 20) - (game.numAnimals[i] / (game.peakNumAnimals / 100f)));
                
                // Side numbers
                f.DrawString(
                    "" + game.numPlants[Game.GRAPH_MEMORY],
                    font, fontColorPlant, 635, (Game.WINDOW_HEIGHT - 27) - (game.numPlants[Game.GRAPH_MEMORY] / (game.peakNumPlants / 100f)));

                f.DrawString(
                    "" + game.numAnimals[Game.GRAPH_MEMORY],
                    font, fontColorAnimal, 635, (Game.WINDOW_HEIGHT - 27) - (game.numAnimals[Game.GRAPH_MEMORY] / (game.peakNumAnimals / 100f)));

                // Black lines
                f.DrawLine(
                    UILineColor,
                    130,
                    (Game.WINDOW_HEIGHT - 20),
                    630,
                    (Game.WINDOW_HEIGHT - 20));

                f.DrawLine(
                    UILineColor,
                    630,
                    (Game.WINDOW_HEIGHT - 20),
                    630,
                    (Game.WINDOW_HEIGHT - 130));
            }
            else
            {
                // Draws the arrow UI

                DrawBorderedPanel(f, Game.WINDOW_WIDTH/2 - 20, Game.WINDOW_HEIGHT - 10, 40, 20, 2);
                f.DrawLine(
                    UILineColor,
                    Game.WINDOW_WIDTH / 2 - 14,
                    Game.WINDOW_HEIGHT - 3,
                    Game.WINDOW_WIDTH / 2 - 1,
                    Game.WINDOW_HEIGHT - 8);
                f.DrawLine(
                    UILineColor,
                    Game.WINDOW_WIDTH / 2 + 12,
                    Game.WINDOW_HEIGHT - 3,
                    Game.WINDOW_WIDTH / 2 - 1,
                    Game.WINDOW_HEIGHT - 8);
            }

            // Info text ----------------------------------------
            for (int i = 0; i < game.infoText.Count; i++)
            {
                try
                {
                    f.DrawString(
                        game.infoText[i].text,
                        font, fontColor, Game.WINDOW_WIDTH - 250, 20 + (i * 20));
                    game.infoText[i].lifetime--;
                    if (game.infoText[i].lifetime <= 0)
                        game.infoText.RemoveAt(i);
                }
                catch (Exception e) { }
            }
        }

        public void DrawBorderedPanel(Graphics f, int x, int y, int width, int height, int thickness)
        {
            f.FillRectangle(UIBorderColor, x-thickness, y-thickness, width+thickness, height+thickness);
            f.FillRectangle(UIColor, x, y, width, height);
        }
    }
}
