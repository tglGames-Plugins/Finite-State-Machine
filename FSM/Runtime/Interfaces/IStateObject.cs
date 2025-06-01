using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGL.FSM
{
    public interface IStateObject<TStateType> 
        where TStateType : Enum
    {
        StateMachine<TStateType> MyStateMachine { get; }
        IState<TStateType> InitializationState { get; }
        List<IState<TStateType>> AllPossibleStates { get; }
        bool IsInitialized { get; } 

        Task Initialize(IState<TStateType> initState, 
            List<IState<TStateType>> allStates,
            Action<bool> initializedSuccessfully = null);

        Task ChangeState(TStateType targetStateType, Action<bool> onStateChangeSuccess = null);
        void LogicUpdate(float deltaTime);
    }
}