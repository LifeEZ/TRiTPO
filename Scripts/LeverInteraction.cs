using UnityEngine;

namespace Assets.Scripts {
    public class LeverInteraction : MonoBehaviour {
        [SerializeField]
        private DoorInteraction m_doorInteraction;//wrong naming, no prefix('m_') needed
        [SerializeField]
        private Animator m_animator;//wrong naming, no prefix('m_') needed

        [SerializeField] private Material m_default;//wrong naming, no prefix('m_') needed
        [SerializeField] private Material m_outlined;//wrong naming, no prefix('m_') needed
        private bool m_materialState;//wrong naming, no prefix('m_') needed

        private void Start() {
            GetComponent<Renderer>().material = m_default;
        }

        public void HandleLever() {//Wrong naming, should be PullLever
            m_doorInteraction.HandleDoor();
            m_animator.SetTrigger("Open");
        }

        public void ChangeMaterial() {
            if (!m_materialState) GetComponent<Renderer>().material = m_outlined;
            if (m_materialState) GetComponent<Renderer>().material = m_default;
            m_materialState = !m_materialState;
        }
        private void Update() {//unused method, should be deleted

        }
    }
}