using System;
using TGL.FSM.MonoBehaviourFSM;
using UnityEngine;

namespace TGL.FSM.Sample
{
    public abstract class StateBase : GenericMonoBehaviorFSMState<StateEnum>
    {
        public virtual void PrintStateData()
        {
            Debug.Log($"we are in {GetStateMachine.CurrentStateType} state");
        }
        
        public abstract StateEnum GetNextStateEnum();
        public abstract StateEnum GetPrevStateEnum();

        public override async Awaitable PreEnter()
        {
            try
            {
                await base.PreEnter();
                Show();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        public override async Awaitable PostExit()
        {
            try
            {
                await base.PostExit();
                Hide();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        public abstract void Show();
        public abstract void Hide();
    }
}
