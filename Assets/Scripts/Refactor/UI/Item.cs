using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Upgrades upgrade;
    public int item;

    public TMP_Text nameText;
    public Image icon;
    public TMP_Text descriptionText;

    public void UpdateItem()
    {
        upgrade = GameManager.instance.upgrades[item].GetComponent<Upgrades>();
        if (upgrade != null)
        {
            nameText.text = upgrade.upgradeName;
            descriptionText.text = upgrade.description;
            icon.sprite = upgrade.icon;
        }
    }

    public void SpawnUpgrade()
    {
        GameManager.instance.SpawnUpgrade(item);
    }
}
