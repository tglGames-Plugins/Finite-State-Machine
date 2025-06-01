using System;
using System.Threading.Tasks;

namespace TGL.FSM
{
    public interface IState<TStateType> : IEquatable<IState<TStateType>> where TStateType : Enum
    {
        public TStateType GetStateType { get; }
        public StateMachine<TStateType> GetStateMachine { get; }
        public IStateObject<TStateType> GetStateObject { get; }
        public bool IsInitialized { get; }

        public void Initialize(StateMachine<TStateType> stateMachine, IStateObject<TStateType> stateObject);

        #region EnterCycle

        /// <summary>
        /// During <see cref="PreEnter"/>, the <see cref="StateMachine{StateEnumType}.CurrentState"/> is not same as this state.<br/>
        /// This state will be set as the <see cref="StateMachine{StateEnumType}.CurrentState"/> after <see cref="PreEnter"/> is called.<br/>
        /// We use this method to set up data before we activate this state 
        /// </summary>
        public void PreEnter();
        
        /// <summary>
        /// During <see cref="Enter"/>, the <see cref="StateMachine{StateEnumType}.CurrentState"/> is same as this state, <br/>
        /// before <see cref="Enter"/> was called, we already called prev state's <see cref="PostExit"/><br/>
        /// We use this method to do all changes that need a time component. We can wait for it and verify it was successfully activated
        /// </summary>
        /// <returns>Task with no data</returns>
        public Task Enter();
        
        #endregion EnterCycle
        
        /// <summary>
        /// Consider this as the Update method of the <see cref="StateMachine{StateEnumType}"/>, <br/>
        /// This is called by the Object using the state machine to run any and all update logic.  <br/>
        /// Replacement for Update method
        /// </summary>
        /// <param name="deltaTime"></param>
        public void LogicUpdate(float deltaTime);
        
        #region ExitCycle
        
        /// <summary>
        /// During <see cref="Exit"/>, the <see cref="StateMachine{StateEnumType}.CurrentState"/> is same as this state, <br/>
        /// after <see cref="Exit"/> ends, we call next state's <see cref="PreEnter"/>, <br/>
        /// and then, the <see cref="StateMachine{StateEnumType}.CurrentState"/> will be changed to something else. <br/>
        /// We are using it to change data so we can safely leave the state
        /// </summary>
        /// <returns>yield statements while this method runs</returns>
        public Task Exit();
        
        /// <summary>
        /// During <see cref="PostExit"/>, the <see cref="StateMachine{StateEnumType}.CurrentState"/> is not same as this state.<br/>
        /// This state is set as the <see cref="StateMachine{StateEnumType}.PrevState"/> before <see cref="PostExit"/> was called. <br/>
        /// We have exited the state, so any data changes needed to be done, can be done now
        /// </summary>
        public void PostExit();
        #endregion ExitCycle
        
        public new bool Equals(IState<TStateType> other)
        {
            return this.GetStateType.Equals(other.GetStateType);
        }

        Task ChangeStateTo(TStateType screenType);
    }
}