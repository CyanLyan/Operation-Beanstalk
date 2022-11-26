# Operation-Beanstalk
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
