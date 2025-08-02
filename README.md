# Finite State Machine
A simple Finite state machine. we can use it to define the states and change states

## Limitations
- If you decide to override 'PreEnter()' or 'PostExit()' in 'GenericMonoBehaviorFSMState' derived class, the GameObject will not auto enable or disable.  
- A simple FSM plugin for Unity. Does not support WebGl platform

### How to Use
To use this plugin, follow these steps:
- create an enum which will be used to identify each state (StateEnum.cs)  
- create a state, this can be your base state (StateBase.cs)
- create state code(concrete states), these should be inherited from the base state. (StateOne.cs, StateTwo.cs, StateThree.cs, etc.)  
- create a state Object  (UIStateObject.cs)
    - this will be the object which will change and determine which state to have
    - this will also generate the StateMachine which is needed to change the state and control the states  
- Add the states to the desired gameobjects if you are using GenericMonoBehaviorFSMState.cs
    - define the enum in the concrete states, in case of ConcreteState.cs, you will have to call the constructor.
- Add the state Object to the desired object which is derived from GenericMonoBehaviourFSMObject.cs or ConcreteStateObject.cs
    - Attach all states and define and attach the initial state to the object

## Samples
Samples can be found in "Samples" folder. There is also a scene with the necessary information.

## How to add this package?
- Open unity package manaegr
- On top right, there is a button to add a package
- add a git package (from git URL)
- fill the Https link for the package, in this case, 'https://github.com/tglGames-Plugins/Finite-State-Machine.git'
- Add
The package will be added under 'TGL FSM' in packages, use as needed.
