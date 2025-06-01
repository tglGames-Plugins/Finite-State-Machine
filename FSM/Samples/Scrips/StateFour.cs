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
    }
}