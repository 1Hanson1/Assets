using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Facility : MonoBehaviour
{
    public GameObject selfF;
    public Text winpoint;
    public int winPoint;

    public int whoseFacility; // 0, 1->4
    public int color; // 0, 1->3, 4
    public int index = -1; //索引，没被买-1

    public List<int> payment1;
    public List<int> payment2;

    public Facility top;
    public Facility right;
    public Facility bottom;
    public Facility left;

    private void Start()
    {
        selfF.SetActive(false);
        //winpoint.text = winPoint.ToString();
        if (color != 0 && index == -1)
        {
            Map_Manager.registerFacility(this);
        }
    }

    public void pushToGM()
    {

    }
    public void pushToGMSkill1()
    {

    }
    public void pushToGMSkill2()
    {

    }
    public void pushToGMPayment1()
    {

    }
    public void pushToGMPayment2()
    {

    }

}
