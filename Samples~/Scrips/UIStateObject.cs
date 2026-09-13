using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
        [CanBeNull] private StateBase _currentState => (MyStateMachine?.CurrentState as StateBase);


        public void Awake()
        {
            showStateButton.onClick.AddListener(ButtonClicked);
            prevStateBtn.onClick.AddListener(PrevState);
            nextStateBtn.onClick.AddListener(NextState);
        }

        private void ButtonClicked()
        {
            _currentState?.PrintStateData();
        }

        void OnDestroy()
        {
            showStateButton.onClick.RemoveListener(ButtonClicked);
            prevStateBtn.onClick.RemoveListener(PrevState);
            nextStateBtn.onClick.RemoveListener(NextState);
        }

        private async void PrevState()
        {
            try
            {
                StateEnum targetState = StateEnum.NONE;
                switch (MyStateMachine.CurrentStateType)
                {
                    case StateEnum.NONE:
                        Debug.Log($"cannot go to prev or next state from {StateEnum.NONE} state");
                        break;
                    default:
                        targetState = _currentState?.GetPrevStateEnum() ?? StateEnum.NONE;
                        break;
                }

                await ChangeStateTo(targetState);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private async Awaitable ChangeStateTo(StateEnum targetState)
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
            try
            {
                StateEnum targetState = StateEnum.NONE;
                switch (MyStateMachine.CurrentStateType)
                {
                    case StateEnum.NONE:
                        Debug.Log($"cannot go to prev or next state from {StateEnum.NONE} state");
                        break;
                    default:
                        targetState = _currentState?.GetNextStateEnum() ?? StateEnum.NONE;
                        break;
                }

                await ChangeStateTo(targetState);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}