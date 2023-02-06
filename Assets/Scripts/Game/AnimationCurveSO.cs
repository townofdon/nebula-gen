using UnityEngine;

[CreateAssetMenu(menuName = "AnimationCurve")]
public class AnimationCurveSO : ScriptableObject
{
    public AnimationCurveType type;
    public AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
