using System.Collections.Generic;
using UnityEngine;
using CyberneticStudios.SOFramework;
using UnityEngine.Assertions;

public class DropdownAnimationCurve : DropdownBase
{
    [SerializeField] AnimationCurveVariable animationCurve;
    [SerializeField] List<AnimationCurveSO> animationCurves = new List<AnimationCurveSO>();

    Dictionary<AnimationCurveType, AnimationCurveSO> animationCurveMap = new Dictionary<AnimationCurveType, AnimationCurveSO>();

    protected override void Init()
    {
        animationCurve.ResetVariable();
        foreach (var curve in animationCurves)
        {
            animationCurveMap[curve.type] = curve;
        }
        var options = System.Enum.GetNames(typeof(AnimationCurveType));
        for (int i = 0; i < options.Length; i++)
        {
            Assert.IsNotNull(animationCurveMap[(AnimationCurveType)i]);
        }
    }

    protected override System.Type GetEnumType()
    {
        return typeof(AnimationCurveType);
    }

    protected override string[] GetExcludedEnumNames()
    {
        return new string[0];
    }

    protected override int GetValue()
    {
        return (int)animationCurve.value.type;
    }

    protected override void OnChange(int enumValue)
    {
        animationCurve.value = animationCurveMap[(AnimationCurveType)enumValue];
        AfterChange();
    }
}
