using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGL.FSM.Exceptions;
using UnityEngine;

namespace TGL.FSM
{
    public interface IStateObject<TStateType> where TStateType : Enum
    {
        StateMachine<TStateType> MyStateMachine { get; }
        IState<TStateType> InitializationState { get; }
        List<IState<TStateType>> AllPossibleStates { get; }
        bool IsInitialized { get; } 

        Awaitable Initialize(IState<TStateType> initState, List<IState<TStateType>> allStates, Action<bool> initializedSuccessfully = null);

        Awaitable ChangeState(TStateType targetStateType, Action<bool> onStateChangeSuccess = null);
        void LogicUpdate(float deltaTime);

        public FSMResult InitTaskResult { get; }
        
        /// <summary>
        /// Public access point for child classes or external scripts to wait for setup completion safely.
        /// Supports multiple concurrent awaiters without violating Unity Awaitable rules.
        /// </summary>
        public async Awaitable<FSMResult> EnsureInitializedAwaitable()
        {
            try
            {
                if (InitTaskResult != null)
                {
                    return InitTaskResult;
                }
                
                // We poll or yield until InitTaskResult is non-null.
                while (InitTaskResult == null)
                {
                    await Awaitable.NextFrameAsync();
                }
                return InitTaskResult;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new FSMResult(false, false, new FsmException("Setup has not been initiated."));
            }
        }
        
        public async Task<FSMResult> EnsureInitializedAsync() // has to be a task
        {
            try
            {
                if (InitTaskResult != null)
                {
                    return InitTaskResult;
                }
                
                // We poll or yield until InitTaskResult is non-null.
                while (InitTaskResult == null)
                {
                    await Task.Yield();
                }
                return InitTaskResult;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new FSMResult(false, false, new FsmException("Setup has not been initiated."));
            }
        }
    }
}