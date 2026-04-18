The game bootstraps in [project installer](Assets/Project/Scripts/DI/ProjectContainerInstaller.cs). There creates [game driver](Assets/Project/Scripts/Game/GameDriver.cs).
</br>
That driver declares game states and starts state machine that run those states.
</br>
States itself are just description of when and to what state transition must be.
</br>
All logic behind states covered in processors and feature runners. They are implement logic belongs to declared state.  