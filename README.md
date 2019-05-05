# My name is MatchingGame. It is puzzle game with a 10x10 board. 
	- The objective of this game is to swap a tile with an adjacent tile to form a horizontal or vertical chain of three or more tiles (like in match-3 games)

# Tools to developed this game:
	- Unity version Unity 2018.2.19f1 (64-bit)
	- Visual studio 2017
	- Photoshop CS: I used it to create Icon, Background, Nodes,...
	- Icons from Client's requirement and Google.
	- Sounds of https://www.bfxr.net/
# Run project:	
	- Start project with Init Scene.
# Game play:
	- Init Scene:
		+ Init Map data and Hint from JSON
		+ Init Manager Components
		+ UI Start Game
		+ Display Best Score
		+ Display Random Hint
	- Main Scene:
		+ Generate a random board filled with different types of tiles, and:
			There is no chain in the board initially.
			There is at least one possible move.
		+ MainUI: Score, Node, Highlight Node, Fly score to Score UI.
		+ Save and load Best Score
# Control: have two way to swap nodes
	- Touch 2 nodes
	- Drag from node to other node