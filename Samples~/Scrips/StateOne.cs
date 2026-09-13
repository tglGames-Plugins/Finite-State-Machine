using TGL.FSM.MonoBehaviourFSM;
using UnityEngine;

namespace TGL.FSM.Sample
{
    public class StateOne : StateBase
    {
        public override void Hide()
        {
            Debug.Log($"Hiding StateOne");
        }

        public override void Show()
        {
            Debug.Log($"Showing StateOne");
        }

        public override StateEnum GetNextStateEnum()
        {
            return StateEnum.STATE_2;
        }

        public override StateEnum GetPrevStateEnum()
        {
            return StateEnum.STATE_4;
        }
    }
}
