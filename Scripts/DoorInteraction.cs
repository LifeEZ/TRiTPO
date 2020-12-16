using UnityEngine;

namespace Assets.Scripts {
    public class DoorInteraction : MonoBehaviour {
        [SerializeField] private Animator m_animator;//wrong naming, no prefix('m_') needed
        private bool m_state;//wrong naming, no prefix('m_') needed

        public void HandleDoor() {////wrong naming, should be DoorOpen
            m_animator.SetTrigger("Open");
            m_state = !m_state;
        }

    }
}
