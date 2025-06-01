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
    }
}