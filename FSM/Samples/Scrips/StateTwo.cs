using TGL.FSM.MonoBehaviourFSM;
using UnityEngine;

namespace TGL.FSM.Sample
{
    public class StateTwo : StateBase
    {
        public override void Hide()
        {
            Debug.Log($"Hiding StateTwo");
        }

        public override void Show()
        {
            Debug.Log($"Showing StateTwo");
        }
    }
}