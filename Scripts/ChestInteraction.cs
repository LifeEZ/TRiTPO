using UnityEngine;

namespace Assets.Scripts {
    public class ChestInteraction : MonoBehaviour {
        [SerializeField]
        private Animator m_animator;//wrong naming, no prefix('m_') needed
        [SerializeField]
        private Material m_default;//wrong naming, no prefix('m_') needed
        [SerializeField]
        private Material m_outlined;//wrong naming, no prefix('m_') needed

        private const int m_gold = 50;//wrong naming, no prefix('m_') needed

        private bool m_materialState;//wrong naming, no prefix('m_') needed
        private bool m_opened;//wrong naming, no prefix('m_') needed

        private void Start() {
            GetComponent<Renderer>().material = m_default;
        }

        public void ChangeMaterial() {
            if (!m_opened) {
                GetComponent<Renderer>().material = m_materialState ? m_default : m_outlined;
                m_materialState = !m_materialState;
            }
        }

        public void Open() {//wrog naming, should be ChestOpen
            m_opened = true;
            GetComponent<Renderer>().material = m_default;
            m_animator.SetTrigger("Open");
        }

        public int GetGold() {
            return m_gold;
        }
    }
}
