using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI abilityName;
    public void CreateAbility(string abilityName)
    {
        this.abilityName.text = abilityName;
    }
}
