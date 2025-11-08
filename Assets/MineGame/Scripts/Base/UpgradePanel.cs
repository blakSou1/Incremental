using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    public List<OneUpgradeButton> upgradeButtons;
    public class OneUpgradeButton
    {
        public LocString label;
        public GeneralButton button;
        public Slider bar;
        public TMP_Text text;
        public int startCost;
        public int currentCost;
    }
    
    private void Awake()
    {
        List<Action<OneUpgradeButton>> dels = new List<Action<OneUpgradeButton>>()
        {
            BuyEffUp,
            BuyRareUp,
            BuyNewSlot,
            BuyAutoSpawn,
            BuyAutoRemove
        };
        upgradeButtons = new List<OneUpgradeButton>()
        {
            new () {label = new LocString("+Power","+Заряд"), startCost = 50},
            new () {label = new LocString("+Luck","+Удача"), startCost = 50},
            new () {label = new LocString("+Slot","+Слот"), startCost = 25},
            new () {label = new LocString("+AutoSpawn","+АвтоСпавн"), startCost = 150},
            new () {label = new LocString("+Auto-Remove Batteries","+Автоснятие батареек"), startCost = 1500}
        };
        for (var i = 0; i < upgradeButtons.Count; i++)
        {
            upgradeButtons[i].button = transform.GetChild(i).transform.GetComponentInChildren<GeneralButton>();
            upgradeButtons[i].text = transform.GetChild(i).transform.GetComponentInChildren<TMP_Text>();
            upgradeButtons[i].bar = transform.GetChild(i).transform.GetComponentInChildren<Slider>();
            upgradeButtons[i].currentCost = upgradeButtons[i].startCost;

            int t = i;
            upgradeButtons[i].button.OnClick.AddListener(() => dels[t](upgradeButtons[t]));
            upgradeButtons[i].button.OnClick.AddListener(() => UpdateUI(upgradeButtons[t]));
            UpdateUI(upgradeButtons[i]);
        }
    }

    public void BuyEffUp(OneUpgradeButton b)
    {
        if (G.GameState.EffLevels != G.GameState.EffLevelsMax && TryPay(b.currentCost))
        {
            R.Audio.ApplySound.PlayAsSound();
            G.GameState.EffLevels++;
            b.bar.value = G.GameState.EffLevels * 1f/ G.GameState.EffLevelsMax;
            b.currentCost = (int)(b.currentCost * 1.35f + 12);
        }
    }
    public void BuyRareUp(OneUpgradeButton b)
    {
        if (G.GameState.RareLevels != G.GameState.RareLevelsMax && TryPay(b.currentCost))
        {
            R.Audio.ApplySound.PlayAsSound();
            G.GameState.RareLevels++;
            G.Main.RandomSelector.UpdateWeights();
            b.bar.value = G.GameState.RareLevels * 1f/ G.GameState.RareLevelsMax;
            b.currentCost = (int)(b.currentCost * 1.3f + 10);
        }
    }
    public void BuyNewSlot(OneUpgradeButton b)
    {
        if (G.GameState.SlotsLevels != G.GameState.SlotLevelsMax && TryPay(b.currentCost))
        {
            R.Audio.ApplySound.PlayAsSound();
            G.GameState.SlotsLevels++;
            b.bar.value = G.GameState.SlotsLevels * 1f/ G.GameState.SlotLevelsMax;
            G.Main.OpenNewSlot();
            b.currentCost = (int)(b.currentCost * 1.5f + 15);
        }
    }
    public void BuyAutoSpawn(OneUpgradeButton b)
    {
        if (G.GameState.AutoSpawnLevels != G.GameState.AutoSpawnLevelsMax && TryPay(b.currentCost))
        {
            R.Audio.ApplySound.PlayAsSound();
            G.GameState.AutoSpawnLevels++;
            b.bar.value = G.GameState.AutoSpawnLevels * 1f/ G.GameState.AutoSpawnLevelsMax;
            b.currentCost = (int)(b.currentCost * 1.5f + 30);
        }
    }
    public void BuyAutoRemove(OneUpgradeButton b)
    {
        if (!G.GameState.AutoRemoveBroken && TryPay(b.currentCost))
        {
            R.Audio.ApplySound.PlayAsSound();
            G.GameState.AutoRemoveBroken = true;
            b.bar.value = 1;
        }
    }

    public void UpdateUI(OneUpgradeButton b)
    {
        if (!Mathf.Approximately(b.bar.value, 1f))
        {
            b.text.text = b.currentCost.ToString() + ": " + b.label.ToString();
        }
        else
        {
            b.text.text = new LocString("Sold Out!", "Продано!").ToString();
        }
        
    }

    private bool TryPay(int count)
    {
        if (G.GameState.Points - count > 0)
        {
            G.GameState.Points -= count;
            return true;
        }

        return false;
    }
}
