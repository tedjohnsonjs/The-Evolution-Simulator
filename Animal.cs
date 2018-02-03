using System;
using System.Threading.Tasks;

namespace The_Evolution_Simulator
{
    public class Animal
    {
        //=============
        // References
        public Game game;

        //=============
        // Variables
        public string name;
        public float posX, posY;
        public int colorR, colorG, colorB;

        // Evolving Traits
        public int startHealth;
        public float speed;
        public float size;
        public int eggValueRequired;

        // Utilities
        public int age = 0;
        public int health;
        public int eggValue = 0;
        public float velX, velY;
        public Random r;
        public int foodEaten = 0;
        public int childrenMade = 0;
        public int generation;

        //=============
        // Functions

        public void Init()
        {
            ChangeDir(r.Next(360), 1);
            health = startHealth;
            eggValue = 0;
        }
        
        // Update animal
        public void Update()
        {
            age++;

            // Slowly die unless eating
            health -= 1 + (age / 1000);

            // Death
            if (health <= 0)
            {
                game.infoText.Add(new InfoText("A " + name + " has died."));
                Die();
            }
            
            // Boundary
            if (posX <= Game.EXTRA_BOUNDARY)
                ChangeDir(r.Next(10, 170), 1);
            if (posX >= Game.MAP_WIDTH - Game.EXTRA_BOUNDARY)
                ChangeDir(r.Next(190, 350), 1);
            if (posY <= Game.EXTRA_BOUNDARY)
                ChangeDir(r.Next(100, 260), 1);
            if (posY >= Game.MAP_HEIGHT - Game.EXTRA_BOUNDARY)
                if (r.Next(2) == 0)
                    ChangeDir(r.Next(0, 80), 1);
                else
                    ChangeDir(r.Next(280, 360), 1);

            // Eating
            for (int i = game.plants.Count - 1; i >= 0; i--)
            {
                Plant p = game.plants[i];

                if (Math.Abs(posX - p.posX) < size && Math.Abs(posY - p.posY) < size)
                {
                    eggValue++;
                    health += (int)p.size * 2;
                    foodEaten++;
                    game.plants.Remove(p);
                    ChangeDir(0, 0);
                    Task.Delay(1000).ContinueWith(t => ChangeDir(r.Next(360), 1));
                }
            }

            // Fighting
            for (int i = game.animals.Count - 1; i >= 0; i--)
            {
                Animal a = game.animals[i];

                // Isnt the same species?
                if (a.name != name)
                {
                    // Colliding?
                    if (Math.Abs(posX - a.posX) < (size + a.size) / 2 && Math.Abs(posY - a.posY) < (size + a.size) / 2)
                    {
                        // Hurt each other
                        int preHealth = (health / 4) * (int)size;
                        int aPreHealth = (a.health / 4) * (int)a.size;
                        health -= aPreHealth;
                        a.health -= preHealth;

                        // Whoever dies, dies
                        if (health <= 0)
                        {
                            // Dies
                            game.infoText.Add(new InfoText("A " + a.name + " killed a " + name + "."));
                            Die();

                            // Recover some health and move on
                            a.health += aPreHealth / 2;
                            a.ChangeDir(0, 0);
                            Task.Delay(250).ContinueWith(t => a.ChangeDir(r.Next(360), 1));
                        }
                        else if (a.health <= 0)
                        {
                            // Dies
                            game.infoText.Add(new InfoText("A " + name + " killed a " + a.name + "."));
                            a.Die();

                            // Recover some health and move on
                            health += preHealth / 2;
                            ChangeDir(0, 0);
                            Task.Delay(250).ContinueWith(t => ChangeDir(r.Next(360), 1));
                        }
                    }
                }
            }

            // Reproduction
            if (eggValue >= eggValueRequired && health > startHealth / 2)
                Reproduce();

            // Actual movement
            posX += velX;
            posY += velY;
        }

        // Reproduction
        public void Reproduce()
        {
            game.infoText.Add(new InfoText("A " + name + " has reproduced."));

            // Generates a new animal based on this one + evolution
            Animal newAnimal = new Animal();
            newAnimal.game = game;
            newAnimal.name = name;
            newAnimal.posX = posX;
            newAnimal.posY = posY;
            newAnimal.colorR = colorR + r.Next(-8, 9);
            newAnimal.colorG = colorG + r.Next(-8, 9);
            newAnimal.colorB = colorB + r.Next(-8, 9);
            newAnimal.startHealth = startHealth + r.Next(-30, 31);
            newAnimal.speed = speed + (r.Next(-30, 31) / 100f);
            newAnimal.size = size + (r.Next(-30, 31) / 100f);
            newAnimal.eggValueRequired = eggValueRequired + r.Next(-1, 2);
            newAnimal.generation = generation + 1;
            newAnimal.r = r;
            newAnimal.Init();

            // Prevents over coloring
            if (newAnimal.colorR > 255)
                newAnimal.colorR = 255;
            if (newAnimal.colorR < 0)
                newAnimal.colorR = 0;
            if (newAnimal.colorG > 255)
                newAnimal.colorG = 255;
            if (newAnimal.colorG < 0)
                newAnimal.colorG = 0;
            if (newAnimal.colorB > 255)
                newAnimal.colorB = 255;
            if (newAnimal.colorB < 0)
                newAnimal.colorB = 0;

            // Safety measure!
            if (newAnimal.eggValueRequired < 2)
                newAnimal.eggValueRequired = 2;

            // Adds to list
            game.animals.Add(newAnimal);

            // Resets reproductive cycle
            eggValue = 0;
            eggValueRequired += 1;
            health -= startHealth / 2;
            childrenMade++;
        }

        // Aim velocity towards angle (0 - North, 90 - East, 180 - South, 270 - West)
        public void ChangeDir(float angle, float speedMultiplier)
        {
            velX = (float)(Math.Sin(angle / (180 / Math.PI)) * speed * speedMultiplier);
            velY = (float)(Math.Cos((angle + 180) / (180 / Math.PI)) * speed * speedMultiplier);
        }

        // Death
        public void Die()
        {
            if (game.gEngine.animalSelected == this)
                game.gEngine.animalSelected = null;

            game.animals.Remove(this);
        }
    }
}
