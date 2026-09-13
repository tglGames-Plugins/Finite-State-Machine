using System;
using System.Threading.Tasks;
using UnityEngine;

namespace TGL.FSM
{
    public class ConcreteState<TStateEnumType> :  IState<TStateEnumType>
        where TStateEnumType : Enum 
    {
        #region MyVariables
        private TStateEnumType myStateType;
        #endregion MyVariables

        #region Interface Properties

        public TStateEnumType GetStateType => myStateType;
        public StateMachine<TStateEnumType> GetStateMachine { get; private set; }
        public IStateObject<TStateEnumType> GetStateObject { get; private set; }
        public bool IsInitialized { get; private set; }
        #endregion Interface Properties

        #region MyMethods

        public ConcreteState(TStateEnumType stateType)
        {
            myStateType = stateType;
        }
        
        public ConcreteState(TStateEnumType stateType, StateMachine<TStateEnumType> stateMachine, IStateObject<TStateEnumType> stateObject)
        {
            myStateType = stateType;
            Initialize(stateMachine, stateObject);
        }

        #endregion MyMethods

        public void Initialize(StateMachine<TStateEnumType> stateMachine, IStateObject<TStateEnumType> stateObject)
        {
            GetStateMachine = stateMachine;
            GetStateObject = stateObject;
            IsInitialized = true;
        }
        
        #region EnterCycle

        /// <summary>
        /// before entering the state
        /// </summary>
        public async Awaitable PreEnter()
        {
            try
            {
                await Awaitable.MainThreadAsync();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        public async Awaitable Enter()
        {
            try
            {
                Debug.Log($"Entering state : {this.GetType().Name}");
                await Awaitable.MainThreadAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion EnterCycle

        /// <summary>
        /// equivalent to Update in Unity thread, Could be called by Update in main thread
        /// </summary>
        /// <param name="deltaTime">the time between last frame and current</param>
        public void LogicUpdate(float deltaTime) { }

        #region ExitCycle
        public async Awaitable Exit()
        {
            try
            {
                await Awaitable.MainThreadAsync();
                Debug.Log($"Exit state : {this.GetType().Name}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// After exiting the state
        /// </summary>
        public async Awaitable PostExit()
        {
            try
            {
                await Awaitable.MainThreadAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion ExitCycle

        public bool Equals(IState<TStateEnumType> other)
        {
            return this.GetStateType.Equals(other.GetStateType);
        }
        
        public virtual async Awaitable ChangeStateTo(TStateEnumType screenType)
        {
            try
            {
                await GetStateObject.ChangeState(screenType, (screenChanged) =>
                {
                    if (screenChanged)
                    {
                        Debug.Log($"State changed : {GetStateMachine.PrevStateType} -> {GetStateMachine.CurrentStateType} successfully");
                    }
                    else
                    {
                        Debug.LogError($"Failed to change to {screenType} page");
                    }
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
