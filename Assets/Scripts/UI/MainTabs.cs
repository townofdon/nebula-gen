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
    Adjust,
}

public class MainTabs : MonoBehaviour
{
    [SerializeField] TabType initialTabSelected = TabType.Main;

    public Action<TabType> OnTabChange;
    public Action<TabType> OnTabFocus;
    public TabType InitialTab => initialTabSelected;
    public bool CanUserMove => currentTab == TabType.Main || currentTab == TabType.Help;
    public bool ShouldViewReset => currentTab == TabType.Noise || currentTab == TabType.Mask || currentTab == TabType.Draw || currentTab == TabType.Border;

    TabType currentTab;

    public TabType CurrentTab => currentTab;

    public void ChangeTab(TabType tab)
    {
        if (tab == currentTab) return;
        currentTab = tab;
        if (OnTabChange != null) OnTabChange.Invoke(tab);
    }

    public void Refocus()
    {
        if (OnTabFocus != null) OnTabFocus.Invoke(currentTab);
    }

    void Start()
    {
        currentTab = initialTabSelected;
        if (OnTabChange != null) OnTabChange.Invoke(initialTabSelected);
    }
}
