using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
    public class HealthBar : MonoBehaviour {
        [SerializeField] private Slider m_slider;//wrong naming, no prefix('m_') needed

        public void SetMaxHealth(int health) {//wrong naming, should be SetMaxHealthValue;incomplete naming of parametr, should be healthValue
            m_slider.maxValue = health;
            m_slider.value = health;
        }

        public void SetHealth(int healthValue) {//wrong naming, SetMaxHealthValue
            m_slider.value = healthValue;
        }
    }
}
