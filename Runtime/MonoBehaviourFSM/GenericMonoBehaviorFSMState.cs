using System;
using System.Threading.Tasks;
using TGL.FSM.Exceptions;
using TGL.FSM.Threads;
using UnityEngine;

namespace TGL.FSM.MonoBehaviourFSM
{
    /// <summary>
    /// This is the base state class for all states that are going to use pre and post for enter and exit
    /// </summary>
    public class GenericMonoBehaviorFSMState<TStateEnumType> : MonoBehaviour, IState<TStateEnumType>
        where TStateEnumType : Enum 
    {
        #region MyVariables
        [SerializeField] protected TStateEnumType myStateType; // as the state is a MonoBehaviour, we can auto set the myStateType in inspector
        private StateMachine<TStateEnumType> myStateMachine;
        private GenericMonoBehaviourFSMObject<TStateEnumType, GenericMonoBehaviorFSMState<TStateEnumType>> myStateObject;
        private bool isInitialized;
        #endregion MyVariables

        #region Interface Properties
        // public getters
        public TStateEnumType GetStateType => myStateType;
        public StateMachine<TStateEnumType> GetStateMachine => myStateMachine;
        public IStateObject<TStateEnumType> GetStateObject => (IStateObject<TStateEnumType>)myStateObject;
        public bool IsInitialized => isInitialized;
        #endregion Interface Properties
        
        #region Interface Methods
        
        public virtual void Initialize(StateMachine<TStateEnumType> _stateMachine, IStateObject<TStateEnumType> _stateObject)
        {
            myStateMachine = _stateMachine;
            myStateObject = _stateObject as GenericMonoBehaviourFSMObject<TStateEnumType, GenericMonoBehaviorFSMState<TStateEnumType>>;
            isInitialized = true;
        }

        #region EnterCycle
        
        public virtual void PreEnter()
        {
            // Dispatch to main thread
            UnityMainThreadDispatcher.Instance.Enqueue(EnableGameObject);
        }

        public Task Enter()
        {
            if (GetStateMachine.CurrentState != null && !GetStateMachine.CurrentState.GetStateType.Equals(GetStateType))
            {
                Debug.LogError($"current state {GetStateMachine.CurrentState.GetType().Name} is still not updated to {this.GetType().Name} state in {nameof(PreEnter)} call");
                return Task.FromException(new FsmException($"state machine's Current state is {GetStateMachine.CurrentState} still not updated to {GetStateType}, StateMachine should not have called {nameof(PreEnter)}"));
            }
            
            PostEnter();
            return Task.CompletedTask;
        }
        
        #endregion EnterCycle
        
        public virtual void LogicUpdate(float deltaTime)
        {
            // do logic Update, we can also get deltaTime in this
        }

        #region ExitCycle
        
        public Task Exit()
        {
            PreExit();
            if (!myStateMachine.CurrentStateType.Equals(this.GetStateType))
            {
                Debug.LogError($"current state {myStateMachine.CurrentState.GetType().Name} is already different than {this.GetType().Name} state in {nameof(PreEnter)} call");
                return Task.FromException(new FsmException($"State Machine's current state is {myStateMachine.CurrentStateType}. It should have been {GetStateType} when we are in {this.GetType()}.{nameof(Exit)}"));
            }
            
            // confirmation that our current state is same as this state while exiting.
            return Task.CompletedTask;
        }
        
        public virtual void PostExit()
        {
            // will run on main thread
            UnityMainThreadDispatcher.Instance.Enqueue(DisableGameObject);
        }
        
        #endregion ExitCycle

        public bool Equals(IState<TStateEnumType> other)
        {
            return GetStateType.Equals(other.GetStateType);
        }
        
        public virtual async Task ChangeStateTo(TStateEnumType screenType)
        {
            await GetStateObject.ChangeState(screenType, (screenChanged) =>
            {
                if (screenChanged)
                {
                    Debug.Log($"State changed : {GetStateMachine.PrevStateType} -> {GetStateMachine.CurrentStateType} successfully", gameObject);
                }
                else
                {
                    Debug.LogError($"Failed to change to {screenType} page");
                }
            });
        }
        #endregion Interface Methods
        
        #region MyMethods
        private void EnableGameObject()
        {
            if (!gameObject) return;
            if (gameObject.activeInHierarchy)
            {
                // Do we need to do anything before we activate this state?
                // How is this state's gameObject already active?
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// During <see cref="PostEnter"/>, the <see cref="StateMachine{GetStateType}.CurrentState"/> is same as this state.<br/>
        /// This method is called at the end of <see cref="Enter"/> method. 
        /// </summary>
        protected virtual void PostEnter()
        {
            // add listeners
            // do What is normally used in Start, if this need not be an enumerator
        }
        
        /// <summary>
        /// During <see cref="PreExit"/>, the <see cref="StateMachine{GetStateType}.CurrentState"/> is same as this state.<br/>
        /// This method is called at the start of <see cref="Exit"/> method. 
        /// </summary>
        protected virtual void PreExit()
        {
            // reset the values, or save whatever data was updated, before the screen stops
            // remove all listeners
        }
        
        private void DisableGameObject()
        {
            if (!gameObject) return;
                
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
            else
            {
                // Do we need to do anything after we de-activate this state?
            }
        }
        #endregion MyMethods
    }
}
