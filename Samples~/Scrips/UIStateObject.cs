using System;
using System.Threading.Tasks;
using TGL.FSM.MonoBehaviourFSM;
using UnityEngine;
using UnityEngine.UI;

namespace TGL.FSM.Sample
{
    public class UIStateObject : GenericMonoBehaviourFSMObject<StateEnum, StateBase>
    {
        public Button showStateButton;
        public Button nextStateBtn;
        public Button prevStateBtn;


        protected override void PreAwake()
        {
            showStateButton.onClick.AddListener(ButtonClicked);
            prevStateBtn.onClick.AddListener(PrevState);
            nextStateBtn.onClick.AddListener(NextState);
        }

        private void ButtonClicked()
        {
            (MyStateMachine?.CurrentState as StateBase)?.PrintStateData();
        }

        void OnDestroy()
        {
            showStateButton.onClick.RemoveListener(ButtonClicked);
            prevStateBtn.onClick.RemoveListener(PrevState);
            nextStateBtn.onClick.RemoveListener(NextState);
        }

        private async void PrevState()
        {
            StateEnum targetState = StateEnum.NONE;
            switch (MyStateMachine.CurrentStateType)
            {
                case StateEnum.NONE:
                    Debug.Log($"cannot go to prev or next state from {StateEnum.NONE} state");
                    break;
                case StateEnum.STATE_1:
                    targetState = StateEnum.STATE_4;
                    break;
                case StateEnum.STATE_2:
                    targetState = StateEnum.STATE_1;
                    break;
                case StateEnum.STATE_3:
                    targetState = StateEnum.STATE_2;
                    break;
                case StateEnum.STATE_4:
                    targetState = StateEnum.STATE_3;
                    break;
            }

            await ChangeStateTo(targetState);
        }

        private async Task ChangeStateTo(StateEnum targetState)
        {
            try
            {
                await ChangeState(targetState, (changeSuccess) =>
                {
                    if (changeSuccess)
                    {
                        Debug.Log($"Changed state to {targetState}");
                    }
                    else
                    {
                        Debug.LogError($"Failed to Change state to {targetState}");
                    }
                });
            }
            catch (Exception exc)
            {
                Debug.LogError($"got an exception trying to change state to {targetState} :: {exc.Message}", gameObject);
            }
        }

        private async void NextState()
        {
            StateEnum targetState = StateEnum.NONE;
            switch (MyStateMachine.CurrentStateType)
            {
                case StateEnum.NONE:
                    Debug.Log($"cannot go to prev or next state from {StateEnum.NONE} state");
                    break;
                case StateEnum.STATE_1:
                    targetState = StateEnum.STATE_2;
                    break;
                case StateEnum.STATE_2:
                    targetState = StateEnum.STATE_3;
                    break;
                case StateEnum.STATE_3:
                    targetState = StateEnum.STATE_4;
                    break;
                case StateEnum.STATE_4:
                    targetState = StateEnum.STATE_1;
                    break;
            }

            await ChangeStateTo(targetState);
        }
    }
}