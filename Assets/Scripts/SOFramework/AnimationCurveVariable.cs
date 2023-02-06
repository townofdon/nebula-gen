using UnityEngine;

public enum AnimationCurveType
{
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut,
    BellCurve,
    SCurve,
}

namespace CyberneticStudios.SOFramework
{
    [CreateAssetMenu(menuName = "Variables/AnimationCurve Variable")]
    public class AnimationCurveVariable : Variable<AnimationCurveSO>
    {
    }
}
