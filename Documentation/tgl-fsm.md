# TGL FSM  
## Overview  
A simple FSM library, it is made so we can use FSM without needing to re-write the logic.

## Package contents  
NA

## Installation instructions  
You can check steps to install at 'https://docs.unity3d.com/Manual/upm-ui-install.html' as they are the same

## Requirements  
The code is written with c# 8 in mind, but if you re-do the necessary format, the code can be run in Unity 2020 or later
  
## Limitations
If you decide to override 'PreEnter()' or 'PostExit()' in 'GenericMonoBehaviorFSMState' derived class, the GameObject will not auto enable or disable.  
  
## Workflows  
See 'Tutorials' or 'How to Use' section.  
  
## Advanced topics  
NA  
  
## Reference  
NA  
  
## Samples  
Samples can be found in "Samples" folder. There is also a scene with the necessary information.  
  
## Tutorials  
  
### How to Use  
To use this plugin, follow these steps:  
- create an enum which will be used to identify each state (.cs)  
- create a state, this can be your base state (.cs)  
- create state code, these should be inherited from the base state.  
- create a state Object   
    - this will be the object which will change and determine which state to have  
    - this will also generate the StateMachine which is needed to change the state and control the states  
- Add the states to the desired objects  
    - define the enum in the states  
- Add the state Object to the desired object  
    - Attach all states and define and attach the initial state to the object  