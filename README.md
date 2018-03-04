# The Evolution Simulator

The Evolution Simulator is a basic simulation of Darwin's Theory of evolution, where species compete for food and other resources, in a game known widely as 'natural selection' or 'survival of the fittest'. The faster/stronger/bigger/etc animal gets more food, so is generally able to live longer, and ends up having more children. So that animals faster/stronger/bigger/etc genes are passed down, while the slower/weaker/smaller/etc genes die off. Slowly over time, the average speed/strength/size/etc of the entire species increases, as do the animals in general. It's this effect which is replicated in this simulation.

[Download the most recent build here](https://drive.google.com/file/d/1gM37NQOmjs4zPqChTcnSp0h8q-Xn_Nfv/view?usp=sharing)

[Blog post on initial development](https://tedjohnsondevblog.blogspot.com/2018/02/the-evolution-simulator.html)

## Using The Simulator

There is currently no interaction with the program once it has started, it's just for observation. All variables are set as constants before the simulation starts, which can be edited in the code.

The camera can be panned using the mouse. Simply click and drag to look around the map. There is currently no zoom functionality available.

Clicking on an animal brings up its stats. This is helpful in understanding which species is evolving and in what way. The arrow at the bottom brings up the population graph, which is useful for an overall view on how the population of animals and plants has changed over time.

## Getting Started

If you want to mess around with the code on your own local machine, you will need to set up the development env.

### Prerequisites

You will need a few tools to develop and test the program:
* [Visual Studio IDE](https://www.visualstudio.com) (2017 preferably)
* Visual C#
* Windows Forms

You should be able to acquire all these once you have downloaded and installed Visual Studio.

### Set Up

This setup process goes through every step required to use the code located in this project. If you would rather just download the finished project, or if you're having any problems setting the project up, contact me with `tedjohnsonjs@gmail.com`.

1. Setup a new Windows Forms project with the default settings.
2. Dock a panel element to the Form1 window, and name it `canvas`. Make sure it completely fills the window.
3. Adjust the Form1 windows properties so it suits the application. This may require you change the size of the window and prevent resizing and maximizing.
4. Create the event `canvas_paint`.
5. Add a timer to the project called `UpdateTimer`, and create the `UpdateTimer_Tick` event for it.
6. Create a class for every script in this project, named the same as the file.
7. Fill in each class with the relevant script.
8. Compile and test. The window should open and the simulation should begin.

### Developing

You can edit the parameters in the Game.cs file, which contains all the constants which define the world. The plant and animal classes contain all the information a object of that type should have. The lists `animals` and `plants` contained in the Game.cs file contain all the animals and plants during the simulation. The GEngine class is responsible for rendering the simulation to the canvas. It runs off a different thread, which can be hazards when removing objects.

If you have any problems or questions, check [the initial development blog](https://tedjohnsondevblog.blogspot.com/2018/02/the-evolution-simulator.html) post or contact me with `tedjohnsonjs@gmail.com`.
