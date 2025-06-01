using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGL.FSM.MonoBehaviourFSM;
using TGL.FSM.Threads;
using UnityEngine;

namespace TGL.FSM
{
    public class ConcreteStateObject<TStateType, TBaseState> : IStateObject<TStateType> 
        where TStateType : Enum
        where TBaseState : IState<TStateType>
    {
        #region MyVariables
        private StateMachine<TStateType> myStateMachine;
        private TBaseState initializationState;
        private List<TBaseState> allPossibleStates;
        private bool isInitialized;
        #endregion
        
        #region Interface Properties
        
        bool IStateObject<TStateType>.IsInitialized => isInitialized;
        // Interface Properties
        StateMachine<TStateType> IStateObject<TStateType>.MyStateMachine => myStateMachine;
        IState<TStateType> IStateObject<TStateType>.InitializationState => initializationState;
        List<IState<TStateType>> IStateObject<TStateType>.AllPossibleStates => allPossibleStates.Cast<IState<TStateType>>().ToList();
        
        #endregion

        #region StateObjectMethods
        
        private async Task Setup(TBaseState initState, 
            List<TBaseState> allStates, 
            Action<bool> initializedSuccessfully = null) 
        {
            isInitialized = false;
            try
            {
                initializationState = initState;
                myStateMachine = new StateMachine<TStateType>();
                allPossibleStates = allStates;
                
                if (allPossibleStates is not { Count: not 0 })
                {
                    Debug.LogError($"The passed states list is null or empty. for type {typeof(TStateType)}");
                }
                else
                {
                    if (allPossibleStates.Any(x => x == null))
                    {
                        Debug.LogError($"There are null in {nameof(allPossibleStates)}, cannot Initialize");
                    }
                    else
                    {
                        allPossibleStates.ForEach(x => x.Initialize(myStateMachine, this));
                        await myStateMachine.Initialize(initializationState, (initSuccess) =>
                        {
                            Debug.Log(initSuccess ? $"successfully initialized stateMachine for type {typeof(TStateType)}" : $"Unable to Initialize stateMachine for type {typeof(TStateType)}");
                            isInitialized = initSuccess;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                throw;
            }
            finally
            {
                initializedSuccessfully?.Invoke(isInitialized);
            }
        }
        #endregion StateObjectMethods

        #region InterfaceOverrides

        public virtual async Task Initialize(IState<TStateType> initState, 
            List<IState<TStateType>> allStates,
            Action<bool> initializedSuccessfully = null)
        {
            await Setup((TBaseState)initState, allStates.Cast<TBaseState>().ToList(), initializedSuccessfully);
        }
        
        public virtual void LogicUpdate(float deltaTime)
        {
            if (!isInitialized)
            {
                Debug.LogError($"The state object is not initialized");
                return;
            }
            
            myStateMachine.LogicUpdate(deltaTime);
        }
        
        public virtual async Task ChangeState(TStateType targetStateType, Action<bool> onStateChangeSuccess = null)
        {
            bool stateChanged = false;
            
            if (!isInitialized)
            {
                Debug.LogError($"The state object is not initialized, cannot change state to {targetStateType}");
                onStateChangeSuccess?.Invoke(stateChanged);
                return;
            }
            
            try
            {
                TBaseState targetState = allPossibleStates.Find(x=> (Equals(x.GetStateType, targetStateType)));
                if (targetState is null)
                {
                    Debug.LogError($"The available states list({allPossibleStates?.Count} items) does not have '{targetStateType}' state, did you Initialize the object before changing states?");
                }
                else
                {
                    await myStateMachine.ChangeState(targetState, (changedSuccessfully) =>
                    {
                        stateChanged = changedSuccessfully;
                        if (changedSuccessfully)
                        {
                            Debug.Log($"Successfully changed state from {myStateMachine.PrevStateType} to {myStateMachine.CurrentStateType}");
                        }
                        else
                        {
                            Debug.LogError($"Could not change state to {targetStateType}");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                throw;
            }
            finally
            {
                onStateChangeSuccess?.Invoke(stateChanged);
            }
        }

        #endregion InterfaceOverrides
    }
}