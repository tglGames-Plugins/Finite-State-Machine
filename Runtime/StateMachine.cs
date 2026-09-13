using System;
using System.Threading.Tasks;
using TGL.FSM.Exceptions;
using UnityEngine;

namespace TGL.FSM
{
    public class StateMachine<TStateType> where TStateType : Enum
    {
        public IState<TStateType> CurrentState { get; private set; }
        public TStateType CurrentStateType => CurrentState == null ? default : CurrentState.GetStateType;
        public IState<TStateType> PrevState { get; private set; }
        public TStateType PrevStateType =>  PrevState == null ? default : PrevState.GetStateType;
        public bool IsStateChanging { get; private set; }

        public async Awaitable Initialize(IState<TStateType> initState, Action<bool> onStateMachineInitialized = null)
        {
            bool initSuccess = false;
            try
            {
                if (CurrentState != null)
                {
                    Debug.LogError($"During Initialize, current state {CurrentState.GetType().Name} is not null");
                }
                else
                {
                    IsStateChanging = true;
                    await initState.PreEnter();
                    PrevState = CurrentState;
                    CurrentState = initState;
                    await CurrentState.Enter();
                    initSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex); // Shows full stack trace in Console
            }
            finally
            {
                IsStateChanging = false;
                onStateMachineInitialized?.Invoke(initSuccess);
            }
        }
        
        public async Awaitable ChangeState(IState<TStateType> newState, Action<bool> onStateChangeSuccess = null)
        {
            bool stateChanged = false;
            try
            {
                if (newState.GetStateType.Equals(CurrentStateType))
                {
                    Debug.LogError($"Already in state [{CurrentState.GetType().Name}:{CurrentStateType}] : cannot change to state [{newState.GetType().Name}:{newState.GetStateType}]");
                    return;
                }
                
                IsStateChanging = true;
                await CurrentState.Exit();
                await newState.PreEnter();
                PrevState = CurrentState;
                CurrentState = newState;
                await PrevState.PostExit();
                await CurrentState.Enter();
                stateChanged = true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex); // Shows full stack trace in Console
            }
            finally
            {
                IsStateChanging = false;
                onStateChangeSuccess?.Invoke(stateChanged);
            }
        }

        public void ChangeStateType(TStateType stateType)
        {
            throw new FsmException($"State Object should identify the state and call {nameof(ChangeState)} function");
        }
        
        internal void LogicUpdate(float deltaTime)
        {
            if (IsStateChanging) { return; }
            CurrentState.LogicUpdate(deltaTime);
        }
    }
}