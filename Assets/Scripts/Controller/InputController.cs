using System;
using UnityEngine;

namespace Controller
{
    public class InputController : MonoBehaviour
    {
        private const float PRESS_TIME = 0.15f;
        private const float DELAY_TIME = 0.2f;
        private bool _press;

        Line _selectingLine;
        private float _timeDown;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && Time.time - _timeDown > DELAY_TIME)
            {
                _selectingLine = GetSelectingLine();
                _timeDown = Time.time;
            }

            if (_selectingLine == null) return;

            if (Input.GetMouseButtonUp(0))
            {
                if (Time.time < _timeDown + PRESS_TIME)
                {
                    PressLine();
                }

                _selectingLine = null;
                Conveyor.Instance.SetBlocking(false);
            }

            if (Input.GetMouseButton(0) && Time.time > _timeDown + PRESS_TIME)
            {
                HoldLine();
            }
        }

        void PressLine()
        {
            _selectingLine.PushToConveyor();
        }

        void HoldLine()
        {
            Conveyor.Instance.SetBlocking(true);
            _selectingLine.TryPushToConveyor();
        }

        Line GetSelectingLine()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return hit.transform.GetComponent<Line>();
            }

            return null;
        }
    }
}