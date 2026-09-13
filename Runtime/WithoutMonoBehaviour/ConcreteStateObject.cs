using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGL.FSM.Exceptions;
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
        
        
        private FSMResult _initTaskResult = null;
        private AwaitableCompletionSource<FSMResult> _initAwaitableCompletionSource;
        #endregion
        
        #region Interface Properties
        
        bool IStateObject<TStateType>.IsInitialized => isInitialized;
        // Interface Properties
        StateMachine<TStateType> IStateObject<TStateType>.MyStateMachine => myStateMachine;
        IState<TStateType> IStateObject<TStateType>.InitializationState => initializationState;
        List<IState<TStateType>> IStateObject<TStateType>.AllPossibleStates => allPossibleStates.Cast<IState<TStateType>>().ToList();
        
        FSMResult IStateObject<TStateType>.InitTaskResult => _initTaskResult;
        #endregion

        #region StateObjectMethods
        
        private async Awaitable<FSMResult> Setup(TBaseState initState, List<TBaseState> allStates, Action<bool> initializedSuccessfully = null) 
        {
            _initAwaitableCompletionSource = new AwaitableCompletionSource<FSMResult>();
            _initTaskResult = null;

            isInitialized = false;
            FSMResult result = null;
            string errorMsg = null;
            try
            {
                initializationState = initState;
                myStateMachine = new StateMachine<TStateType>();
                allPossibleStates = allStates;
                
                if (allPossibleStates is not { Count: not 0 })
                {
                    errorMsg = $"The passed states list is null or empty. for type {typeof(TStateType)}";
                    Debug.LogError(errorMsg);
                    result = new FSMResult(false, false, new FsmException(errorMsg));
                }
                else if (allPossibleStates.Any(x => x == null))
                {
                    errorMsg = $"There are null items in {nameof(allPossibleStates)}, cannot Initialize";
                    Debug.LogError(errorMsg);
                    result = new FSMResult(false, false, new FsmException(errorMsg));
                }
                else
                {
                    allPossibleStates.ForEach(x => x.Initialize(myStateMachine, this));
                    
                    errorMsg = $"Unable to Initialize stateMachine for type {typeof(TStateType)}";
                    await myStateMachine.Initialize(initializationState, (initSuccess) =>
                    {
                        isInitialized = initSuccess;
                    });
                    
                    if (isInitialized)
                    {
                        Debug.Log($"successfully initialized stateMachine for type {typeof(TStateType)}");
                        _initTaskResult = FSMResult.GetSuccess();
                    }
                    else
                    {
                        _initTaskResult = new FSMResult(false, false, new FsmException(errorMsg));
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                result = new FSMResult(false, false, new FsmException($"Unable to Initialize stateMachine for type {typeof(TStateType)}", ex));
            }
            finally
            {
                result ??= new FSMResult(false, false, new FsmException("Unknown error during setup."));
                
                // Set completion sources and flags here
                _initAwaitableCompletionSource.SetResult(result);
                _initTaskResult = result;
                initializedSuccessfully?.Invoke(isInitialized);
            }
            
            return result;
        }
        #endregion StateObjectMethods

        #region InterfaceOverrides

        public virtual async Awaitable Initialize(IState<TStateType> initState, List<IState<TStateType>> allStates, Action<bool> initializedSuccessfully = null)
        {
            if (_initTaskResult is null)
            {
                try
                {
                    await Setup((TBaseState)initState, allStates.Cast<TBaseState>().ToList(), initializedSuccessfully);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
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
        
        public virtual async Awaitable ChangeState(TStateType targetStateType, Action<bool> onStateChangeSuccess = null)
        {
            if (myStateMachine.IsStateChanging)
            {
                Debug.LogError($"already changing states, cannot change state to {targetStateType}");
                return;
            }
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
