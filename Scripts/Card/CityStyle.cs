using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityStyle : MonoBehaviour
{
    public GameObject selfF;


    private void Start()
    {
        Map_Manager.registerCityStyle(this);
        selfF.SetActive(false);
    }
}
