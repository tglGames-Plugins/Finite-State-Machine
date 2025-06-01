using System;
using System.Threading.Tasks;

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
        public void PreEnter() { }

        public Task Enter()
        {
            return Task.CompletedTask;
        }
        #endregion EnterCycle

        /// <summary>
        /// equivalent to Update in Unity thread, Could be called by Update in main thread
        /// </summary>
        /// <param name="deltaTime">the time between last frame and current</param>
        public void LogicUpdate(float deltaTime) { }

        #region ExitCycle
        public Task Exit()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// After exiting the state
        /// </summary>
        public void PostExit() { }
        #endregion ExitCycle

        public bool Equals(IState<TStateEnumType> other)
        {
            return this.GetStateType.Equals(other.GetStateType);
        }
        
        public virtual async Task ChangeStateTo(TStateEnumType screenType)
        {
            await GetStateObject.ChangeState(screenType, (screenChanged) =>
            {
                if (screenChanged)
                {
                    Console.WriteLine($"State changed : {GetStateMachine.PrevStateType} -> {GetStateMachine.CurrentStateType} successfully");
                }
                else
                {
                    Console.WriteLine($"Failed to change to {screenType} page");
                }
            });
        }
    }
}