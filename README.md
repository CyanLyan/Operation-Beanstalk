# Operation-Beanstalk

This is a basic clone of Jenga done in Unity v202.3.17f. The original intention was to create an expanded game with multiple towers and a real-estate/monopoly metagame aspect, but for the moment we're just trying to nail down the basics. Back in 2022 we had a basic gameplay loop working, where blocks could be clicked/dragged, placed on top, before moving more blocks. 

## TODO LIST ##
-Fix block mover to move to MidwayBlockMovePoint THEN the dropview selected, using LERP

-Fix camera shake for block nudge, which keeps returning the camera position to the default after every effect

-Add a UI element to visually represent drag tool, which will replace LineRenderer in DragBlock

-Fix HandleBlockTouchingNothing() inside Block.cs to only trigger when the tower is truly toppling over

--Fix the calling hierarchy for this method. Right now it goes TowerCollisionBox->Block->GameController.Tower.TowerIsCollapsing(), which is ridiculous!

--Fix TowerIsCollapsing by ensuring it only collapses ONCE

-Fix which tower drop zone position is selected so that blocks always fall perpendiculur to the rotation of the blocks on the level below

-create a glowing, transluscent plane in the DropZone which shows where players can place blocks on top, which only appears when we're dropping a block

-Have multiple players with turns - half done but needs UI for player1 or player 2

-Create AI opponent which can make decisions and nudge/drag like player (so that we can play against an opponent!)

## Update (18/10/25) ##: After implementing a new Input System, fixing assembly issues, and recovering lost files, the current build is a little broken. While blocks can be clicked on, they cannot be dragged nor placed. We're currently working on restoring functionality and also just improving general code quality. 
This current build is a bit of a mess as most of the code was admittedly made when I was much newer to coding large-scale projects, and I got a bit over-ambitious with trying new ideas and in the process I created a bunch of asset bloat. Back when I wrote a large part of this, I didn't really understand the difference between writing code that felt smart, and...good code. Coming back to this project now is a bit of a rude awakening to my old code habits, because a lot of this project could be _massively_ simplified. The gamefeel is good, the visuals work, but debugging errors and moving through the stacktrace has been a nightmare. I intend to continue working on this project as it's been a good learning experience to refactor.
