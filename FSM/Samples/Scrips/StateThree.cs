using TGL.FSM.MonoBehaviourFSM;
using UnityEngine;
using UnityEngine.UI;

namespace TGL.FSM.Sample
{
    public class StateThree : StateBase
    {
        Color color = Color.cyan;
        Image myImage;

        public override void PreEnter()
        {
            if (myImage == null)
            {
                myImage = GetComponent<Image>();
            }
            base.PreEnter();
        }

        public override void Hide()
        {
            if (myImage != null)
            {
                myImage.color = Color.black;
            }
        }

        public override void Show()
        {
            if (myImage != null)
            {
                myImage.color = color;
            }
        }

        public override void PrintStateData()
        {
            base.PrintStateData();
            if (myImage != null)
            {
                myImage.color = Random.ColorHSV(0.5f, 1, 0.5f, 1, 0.5f, 1, 0.5f, 1);
            }
        }
    }
}