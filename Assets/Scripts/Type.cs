using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Type : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI typeName;
    public void CreateType(string typeName)
    {
        this.typeName.text = typeName;
    }
}
