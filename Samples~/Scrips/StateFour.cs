using TGL.FSM.MonoBehaviourFSM;
using UnityEngine;

namespace TGL.FSM.Sample
{
    public class StateFour : StateBase
    {
        public override void Hide()
        {
            Debug.Log($"hiding some weird text in StateFour");
        }

        public override void Show()
        {
            Debug.Log($"Showing some weird text in StateFour");
        }

        public override StateEnum GetNextStateEnum()
        {
            return StateEnum.STATE_1;
        }

        public override StateEnum GetPrevStateEnum()
        {
            return StateEnum.STATE_3;
        }
    }
}