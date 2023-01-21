using UnityEngine;

namespace CyberneticStudios.SOFramework
{
    [CreateAssetMenu(menuName = "Variables/Float Variable")]
    public class FloatVariable : Variable<float>
    {
        public void ApplyChange(float amount)
        {
            value += amount;
        }

        public void ApplyChange(FloatVariable amount)
        {
            value += amount.value;
        }
    }
}