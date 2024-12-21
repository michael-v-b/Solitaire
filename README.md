#Solitaire
A custom implementation of the popular card game Solitaire created as a personal Project created over the Summer of 2022.

## Rules:
As you start the game you are presented with a set of 7 decks each containing an incrementing number of cards. 
Each turn you have 1 of 3 options: 
- move a card under a card in a different pile that is 1 rank above it and an opposite color
- move a card to a stack of the same suit, where Ace is the first entry
- select a card from the deck
The goal of the game is to organize the entire deck into the 4 stacks in the top left.

##Features
-Clicking shortcuts which automatically move the card to the first available option
-Hints which indicate a move that is available to the player
-Reset, not ever game is winnable so there's no shame in having to restart a game
-Undo and Redo allowing for thorough exploration of different movement options
-Procedural animations, both for general play and for winning a match

##How Was it Made?
I made this program using C# in the Unity Game Engine. I chose this language and program due to the abundance of resources for it, along with some previous experience using it.

##What I learned?
This was a very fun project which taught me a lot about development and the importance of maintaining organized and easily readible code. There were many bugs which had ocurred resulting from spaghetti code that needed to be refactored later during the project. 
For example: while implementing the card automatically travelling to an available space after being clicked, when the player clicked on the deck it would automatically go to the next available space. This, of course was not intended behavior and resulted from my labelling the deck as a card when it should not have been.
Additonally this project taught me a significant amount about Front-End design such as the importance of animations and graphics to the game experience.
