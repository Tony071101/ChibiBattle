using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStatUI : MonoBehaviour
{
    [SerializeField] private Slider dmgSlider;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpValue_txt;
    [SerializeField] private TextMeshProUGUI dmgValue_txt;
    [SerializeField] private TextMeshProUGUI totalCoin_txt;
    [SerializeField] private TextMeshProUGUI upgradeCostDMG_txt;
    [SerializeField] private TextMeshProUGUI upgradeCostHP_txt;
    private int baseUpgradeCost = 20;
    private int totalCoin;
    private int upgradeCostDMG;
    private int upgradeCostHP;
    private void Start() {
        totalCoin = PlayerPrefs.GetInt("TotalCoin");

        upgradeCostDMG = PlayerPrefs.GetInt("UpgradeCostDMG", baseUpgradeCost);
        upgradeCostHP = PlayerPrefs.GetInt("UpgradeCostHP", baseUpgradeCost);

        dmgSlider.value = PlayerPrefs.GetFloat("DMGSliderValue", 0);
        hpSlider.value = PlayerPrefs.GetFloat("HPSliderValue", 0);
    }

    private void Update() {
        hpValue_txt.text = hpSlider.value + "/" + hpSlider.maxValue;
        dmgValue_txt.text = dmgSlider.value + "/" + dmgSlider.maxValue; 
        totalCoin_txt.text = "Total Coin: " + totalCoin.ToString();
        upgradeCostDMG_txt.text = upgradeCostDMG.ToString();
        upgradeCostHP_txt.text = upgradeCostHP.ToString();
    }

    public void UpgradeDMG() {
        if (totalCoin >= upgradeCostDMG && dmgSlider.value < dmgSlider.maxValue)
        {
            totalCoin -= upgradeCostDMG; // Trừ coin
            dmgSlider.value += 1;       // Tăng giá trị slider
            upgradeCostDMG *= 2;        // Nhân đôi chi phí nâng cấp

            // Lưu coin và chi phí nâng cấp DMG
            PlayerPrefs.SetInt("TotalCoin", totalCoin);
            PlayerPrefs.SetFloat("DMGSliderValue", dmgSlider.value);
            PlayerPrefs.SetInt("UpgradeCostDMG", upgradeCostDMG);
            PlayerPrefs.Save();         // Lưu thay đổi
            Debug.Log($"DMG upgraded. Total Coin: {totalCoin}, Next Cost: {upgradeCostDMG}");
        }
        else
        {
            Debug.Log("Not enough coins or max level reached for DMG.");
        }
    }

    public void UpgradeHP() {
        if (totalCoin >= upgradeCostHP && hpSlider.value < hpSlider.maxValue)
        {
            totalCoin -= upgradeCostHP; // Trừ coin
            hpSlider.value += 1;       // Tăng giá trị slider
            upgradeCostHP *= 2;        // Nhân đôi chi phí nâng cấp

            // Lưu coin và chi phí nâng cấp HP
            PlayerPrefs.SetInt("TotalCoin", totalCoin);
            PlayerPrefs.SetFloat("HPSliderValue", hpSlider.value);
            PlayerPrefs.SetInt("UpgradeCostHP", upgradeCostHP);
            PlayerPrefs.Save();         // Lưu thay đổi
            Debug.Log($"HP upgraded. Total Coin: {totalCoin}, Next Cost: {upgradeCostHP}");
        }
        else
        {
            Debug.Log("Not enough coins or max level reached for HP.");
        }
    }
}
