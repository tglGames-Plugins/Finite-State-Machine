using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TGL.FSM.Exceptions;
using TGL.FSM.Threads;
using UnityEngine;

namespace TGL.FSM.MonoBehaviourFSM
{
    public class GenericMonoBehaviourFSMObject<TStateEnumType, TBaseState> : 
        MonoBehaviour, IStateObject<TStateEnumType>
        where TBaseState : GenericMonoBehaviorFSMState<TStateEnumType>
        where TStateEnumType : Enum
    {
        #region MyVariables
        private StateMachine<TStateEnumType> myStateMachine;
        [SerializeField] private TBaseState initializationState;
        [SerializeField] private List<TBaseState> allPossibleStates;
        private bool isInitialized;
        
        
        private FSMResult _initTaskResult = null;
        private AwaitableCompletionSource<FSMResult> _initAwaitableCompletionSource;
        #endregion MyVariables
        
        #region Interface Properties
        bool IStateObject<TStateEnumType>.IsInitialized => isInitialized;
        // Interface Properties
        public StateMachine<TStateEnumType> MyStateMachine => myStateMachine;
        IState<TStateEnumType> IStateObject<TStateEnumType>.InitializationState => initializationState;
        List<IState<TStateEnumType>> IStateObject<TStateEnumType>.AllPossibleStates => allPossibleStates.Cast<IState<TStateEnumType>>().ToList();
        
        FSMResult IStateObject<TStateEnumType>.InitTaskResult => _initTaskResult;
        #endregion
        
        
        private async void Start()
        {
            try
            {
                _initTaskResult = await Setup(
                    initState: initializationState,
                    allStates: allPossibleStates,
                    initializedSuccessfully: (initializedSuccessfully) =>
                    {
                        isInitialized = initializedSuccessfully;
                        Debug.Log(
                            initializedSuccessfully
                                ? $"State object ({gameObject.name}) initialized successfully"
                                : $"State object ({gameObject.name}) not initialized, error of some sort", gameObject);
                    });

                if (_initTaskResult?.IsCompletedSuccessfully ?? false)
                {
                    Debug.Log($"{this.GetType().Name} is initialized successfully", gameObject);
                }
                else if (_initTaskResult?.IsCancelled ?? false)
                {
                    Debug.LogWarning($"{this.GetType().Name} has canceled initialization", gameObject);
                }
                else if (_initTaskResult?.HasException ?? false)
                {
                    Debug.LogError(
                        $"{this.GetType().Name} encountered exception during initialization: " +
                        _initTaskResult.Exception.Message, gameObject);
                }
                else
                {
                    Debug.LogWarning($"{this.GetType().Name} has some weird data during initialization: " +
                                     $"IsCompleted:[{(_initTaskResult?.IsCompletedSuccessfully ?? false)}], " +
                                     $"IsCanceled:[{(_initTaskResult?.IsCancelled ?? false)}], " +
                                     $"HasException:[{(_initTaskResult?.HasException ?? false)}]", gameObject);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        private void Update()
        {
            LogicUpdate(Time.deltaTime);
        }
        
        #region StateMethods

        /// <summary>
        /// Sets up the FSM Object, will return Awaitable FSMResult
        /// Populates <see cref="_initTaskResult"/> for anyone who needs to confirm setup is complete.
        /// </summary>
        /// <param name="initState">The initial State the object will be in</param>
        /// <param name="allStates">All states the object can be in</param>
        /// <param name="initializedSuccessfully">func informing the caller if the request ended successfully</param>
        /// <returns>Awaitable result of <see cref="FSMResult"/> type</returns>
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
                myStateMachine = new StateMachine<TStateEnumType>();
                allPossibleStates = allStates;
                
                if (allPossibleStates is not { Count: not 0 })
                {
                    errorMsg = $"The passed states list is null or empty. for type {typeof(TStateEnumType)}";
                    Debug.LogError(errorMsg, gameObject);
                    result = new FSMResult(false, false, new FsmException(errorMsg));
                }
                else if (allPossibleStates.Any(x => x == null))
                {
                    errorMsg = $"There are null items in {nameof(allPossibleStates)}, cannot Initialize";
                    Debug.LogError(errorMsg, gameObject);
                    result = new FSMResult(false, false, new FsmException(errorMsg));
                }
                else
                {
                    allPossibleStates.ForEach(x => x.Initialize(myStateMachine, this));
                    
                    errorMsg = $"Unable to Initialize stateMachine for type {typeof(TStateEnumType)}";
                    await myStateMachine.Initialize(initializationState, (initSuccess) =>
                    {
                        isInitialized = initSuccess;
                    });
                    
                    if (isInitialized)
                    {
                        Debug.Log($"successfully initialized stateMachine for type {typeof(TStateEnumType)}", gameObject);
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
                result = new FSMResult(false, false, new FsmException($"Unable to Initialize stateMachine for type {typeof(TStateEnumType)}", ex));
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
        
        #endregion StateMethods
        
        
        #region InterfaceOverrides
        
        /// <summary>
        /// Do not call this method, The code in <see cref="GenericMonoBehaviourFSMObject"/> will auto call it in <see cref="Awake"/> method,<br/>
        /// or we can directly do Initialization by other methods
        /// </summary>
        /// <param name="initState">initialization state</param>
        /// <param name="allStates">all states in this state machine</param>
        /// <param name="initializedSuccessfully">callback action</param>
        /// <returns></returns>
        async Awaitable IStateObject<TStateEnumType>.Initialize(IState<TStateEnumType> initState, List<IState<TStateEnumType>> allStates, Action<bool> initializedSuccessfully)
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

        public virtual async Awaitable ChangeState(TStateEnumType targetStateType, Action<bool> onStateChangeSuccess = null)
        {
            if (myStateMachine.IsStateChanging)
            {
                Debug.LogError($"already changing states, cannot change state to {targetStateType}", gameObject);
                return;
            }
            bool stateChanged = false;
            
            if (!isInitialized)
            {
                Debug.LogError($"The state object is not initialized, cannot change state to {targetStateType}", gameObject);
                onStateChangeSuccess?.Invoke(stateChanged);
                return;
            }
            
            try
            {
                TBaseState targetState = allPossibleStates.Find(x=> (Equals(x.GetStateType, targetStateType)));
                if (targetState is null)
                {
                    Debug.LogError($"The available states list({allPossibleStates?.Count} items) does not have '{targetStateType}' state, did you Initialize the object before changing states?", gameObject);
                }
                else
                {
                    await myStateMachine.ChangeState(targetState, (changedSuccessfully) =>
                    {
                        stateChanged = changedSuccessfully;
                        if (changedSuccessfully)
                        {
                            Debug.Log($"Successfully changed state from {myStateMachine.PrevStateType} to {myStateMachine.CurrentStateType}", gameObject);
                        }
                        else
                        {
                            Debug.LogError($"Could not change state to {targetStateType}", gameObject);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                onStateChangeSuccess?.Invoke(stateChanged);
            }
        }

        public virtual void LogicUpdate(float deltaTime)
        {
            if (!isInitialized)
            {
                Debug.LogWarning($"The state object is not initialized", gameObject);
                return;
            }
            
            myStateMachine.LogicUpdate(deltaTime);
        }
        
        #endregion InterfaceOverrides
    }
}
