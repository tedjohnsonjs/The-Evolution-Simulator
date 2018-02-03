using System;
using System.Windows.Forms;

namespace The_Evolution_Simulator
{
    public partial class MainWindow : Form
    {
        //==============
        // References
        GEngine gEngine;
        Game game;

        //==============
        // Functions

        // Sets up window
        public MainWindow()
        {
            InitializeComponent();
        }

        // Starts everything here
        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            // So the program doesnt start more than once
            if (gEngine == null && game == null)
            {
                // Starts graphics
                gEngine = new GEngine() { g = canvas.CreateGraphics(), main = this };

                // Starts game
                game = new Game() { gEngine = gEngine, main = this };

                // Gives game to gEngine
                gEngine.game = game;

                // Starts everything
                gEngine.Init();
                game.Init();

                // Starts updating game
                UpdateTimer.Start();
            }
        }

        // Updates the game frequently
        private void timer1_Tick(object sender, EventArgs e)
        {
            game.Update();
        }

        // As the window closes
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            game.Stop();
        }

        // For mouse control ------------------------------------

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            // Dont accept too early clicks
            if (gEngine != null && game != null)
                game.MouseClick(e.X, e.Y);
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            // Dont accept too early clicks
            if (gEngine != null && game != null)
                game.mouseDown = false;
        }
    }
}
