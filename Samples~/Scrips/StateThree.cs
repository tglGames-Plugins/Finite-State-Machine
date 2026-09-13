using System;
using TGL.FSM.MonoBehaviourFSM;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TGL.FSM.Sample
{
    public class StateThree : StateBase
    {
        Color color = Color.cyan;
        Image myImage;

        public override async Awaitable PreEnter()
        {
            try
            {
                if (myImage == null)
                {
                    myImage = GetComponent<Image>();
                }
                await base.PreEnter();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
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

        public override StateEnum GetNextStateEnum()
        {
            return StateEnum.STATE_4;
        }

        public override StateEnum GetPrevStateEnum()
        {
            return StateEnum.STATE_2;
        }
    }
}