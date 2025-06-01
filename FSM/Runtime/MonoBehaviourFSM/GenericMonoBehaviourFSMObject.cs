using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        #endregion MyVariables
        
        #region Interface Properties
        bool IStateObject<TStateEnumType>.IsInitialized => isInitialized;
        // Interface Properties
        public StateMachine<TStateEnumType> MyStateMachine => myStateMachine;
        IState<TStateEnumType> IStateObject<TStateEnumType>.InitializationState => initializationState;
        List<IState<TStateEnumType>> IStateObject<TStateEnumType>.AllPossibleStates => allPossibleStates.Cast<IState<TStateEnumType>>().ToList();
        #endregion
        
        private Task initTask;

        #region AwakeMethods

        protected virtual void PreAwake() { }
        
        private async void Awake()
        {
            PreAwake();
            /*
            initTask = Initialize(
                initState: initializationState as IState<TStateEnumType>,
                allStates: allPossibleStates.Cast<IState<TStateEnumType>>().ToList(),
                initializedSuccessfully: (initializedSuccessfully) =>
                {
                    Debug.Log(initializedSuccessfully ? 
                        $"State object ({gameObject.name}) initialized successfully" : 
                        $"State object ({gameObject.name}) not initialized, error of some sort", gameObject);
                });
            */
            initTask = Setup(
            initState: initializationState,
            allStates: allPossibleStates,
            initializedSuccessfully: (initializedSuccessfully) =>
            {
                Debug.Log(initializedSuccessfully ? 
                    $"State object ({gameObject.name}) initialized successfully" : 
                    $"State object ({gameObject.name}) not initialized, error of some sort", gameObject);
            });
            await initTask;
            PostAwake();
        }
        
        protected virtual void PostAwake() { }

        #endregion AwakeMethods
        
        private IEnumerator Start()
        {
            if (!isInitialized)
            {
                WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
                while (!isInitialized && 
                       !initTask.IsCanceled && 
                       !initTask.IsCompleted &&
                       !initTask.IsFaulted)
                {
                    yield return waitForEndOfFrame;
                }
            }

            if (initTask.IsCompleted && !initTask.IsCanceled && !initTask.IsFaulted)
            {
                Debug.Log($"{this.GetType().Name} is initialized successfully", gameObject);
            }
            else if (initTask.IsFaulted)
            {
                Debug.LogError($"{this.GetType().Name} encountered fault during initialization", gameObject);
            }
            else if (initTask.IsCanceled)
            {
                Debug.LogWarning($"{this.GetType().Name} has canceled initialization", gameObject);
            }
            else
            {
                Debug.LogWarning($"{this.GetType().Name} has some weird data during initialization: " +
                                 $"IsCompleted:[{initTask.IsCompleted}], IsCanceled:[{initTask.IsCanceled}], IsFaulted:[{initTask.IsFaulted}]", gameObject);
            }
        }
        
        private void Update()
        {
            LogicUpdate(Time.deltaTime);
        }
        
        #region StateMethods
        
        private async Task Setup(TBaseState initState, 
            List<TBaseState> allStates,
            Action<bool> initializedSuccessfully = null)
        {
            isInitialized = false;
            try
            {
                initializationState = initState;
                myStateMachine = new StateMachine<TStateEnumType>();
                allPossibleStates = allStates;
                
                if (allPossibleStates is not { Count: not 0 })
                {
                    Debug.LogError($"The passed states list is null or empty. for type {typeof(TStateEnumType)}", gameObject);
                }
                else
                {
                    if (allPossibleStates.Any(x => x == null))
                    {
                        Debug.LogError($"There are null in {nameof(allPossibleStates)}, cannot Initialize", gameObject);
                    }
                    else
                    {
                        allPossibleStates.ForEach(x => x.Initialize(myStateMachine, this));
                        await myStateMachine.Initialize(initializationState, (initSuccess) =>
                        {
                            Debug.Log(initSuccess ? $"successfully initialized stateMachine for type {typeof(TStateEnumType)}" : $"Unable to Initialize stateMachine for type {typeof(TStateEnumType)}", gameObject);
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
        async Task IStateObject<TStateEnumType>.Initialize(IState<TStateEnumType> initState, List<IState<TStateEnumType>> allStates, Action<bool> initializedSuccessfully)
        {
            await Setup((TBaseState)initState, allStates.Cast<TBaseState>().ToList(), initializedSuccessfully);
        }

        public virtual async Task ChangeState(TStateEnumType targetStateType, 
            Action<bool> onStateChangeSuccess = null)
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
                throw;
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
                Debug.LogError($"The state object is not initialized", gameObject);
                return;
            }
            
            myStateMachine.LogicUpdate(deltaTime);
        }
        #endregion InterfaceOverrides
    }
}