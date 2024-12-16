using Project.UserTest;
using UnityEngine;

namespace Project
{
    public class UserTestManager : MonoBehaviour
    {
        private float _timerStartValue;
        private float _timerEndValue;
        private int _wrongItemCounter; 

        private bool _timerRunning = false;

        public void StartTimer() {
            _timerRunning = true;
            _timerStartValue = Time.time;
        }

        private void OnDestroy() {
            Debug.Log($"Timer: {_timerEndValue - _timerStartValue} -- Wrong Item Counter: {_wrongItemCounter}");
        }

        public void EndTimer() {
            _timerRunning = false;
            _timerEndValue = Time.time;
            Debug.Log($"Timer: {_timerEndValue - _timerStartValue} -- Wrong Item Counter: {_wrongItemCounter}");
        }

        public void RegisterGrabbedItem(UserTestItem item) {
            if (item.IsStarter && !_timerRunning)
                StartTimer();

            else if (!item.IsTarget && _timerRunning) 
                _wrongItemCounter++;

            else if (_timerRunning)
                EndTimer();
        }
    }
}
