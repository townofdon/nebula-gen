using System;
using UnityEngine;

public enum TabType
{
    Main,
    Noise,
    Mask,
    Draw,
    Help,
    Border,
}

public class MainTabs : MonoBehaviour
{
    [SerializeField] TabType initialTabSelected = TabType.Main;

    public Action<TabType> OnTabChange;
    public TabType InitialTab => initialTabSelected;
    public bool CanUserMove => currentTab == TabType.Main || currentTab == TabType.Help;
    public bool ShouldViewReset => currentTab == TabType.Noise || currentTab == TabType.Mask || currentTab == TabType.Draw;

    TabType currentTab;

    public void ChangeTab(TabType tab)
    {
        if (tab == currentTab) return;

        Debug.Log(Enum.GetName(typeof(TabType), tab));

        currentTab = tab;
        if (OnTabChange != null) OnTabChange.Invoke(tab);
    }

    void Start()
    {
        currentTab = initialTabSelected;
        if (OnTabChange != null) OnTabChange.Invoke(initialTabSelected);
    }
}
