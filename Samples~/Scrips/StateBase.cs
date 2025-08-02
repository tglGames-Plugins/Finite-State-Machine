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

        public override void PreEnter()
        {
            base.PreEnter();
            Show();
        }

        public override void PostExit()
        {
            base.PostExit();
            Hide();
        }

        public abstract void Show();
        public abstract void Hide();
    }
}