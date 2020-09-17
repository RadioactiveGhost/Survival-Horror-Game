using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct TabAndAssociate
{
    public GameObject tab;
    public GameObject item;
}

public class TabManager : MonoBehaviour
{
    public List<TabAndAssociate> TAs;

    private void Start()
    {
        Activate(TAs[0]);

        StartCoroutine(DelayOneFrame());
    }

    IEnumerator DelayOneFrame()
    {
        yield return 0;

        for (int i = 1; i < TAs.Count; i++)
        {
            Deactivate(TAs[i]);
        }
    }

    public void FindButtonOnList(Button b)
    {
        int i = 0;

        foreach (TabAndAssociate tas in TAs) //Find right member on list from button
        {
            if (tas.tab.GetComponent<Button>() == b)
            {
                break;
            }
            i++;
        }
        Activate(TAs[i]); //Activate member

        for (int f = 0; f < TAs.Count; f++) //Deactivate all others
        {
            if (f != i)
            {
                Deactivate(TAs[f]);
            }
        }
    }

    public void Activate(TabAndAssociate TA)
    {
        TA.item.SetActive(true);
        TA.tab.GetComponent<Image>().color = Color.grey;
        TA.tab.GetComponent<Button>().enabled = false;
        //Debug.Log("Activated " + TA.item.name);
    }

    public void Deactivate(TabAndAssociate TA)
    {
        TA.item.SetActive(false);
        TA.tab.GetComponent<Image>().color = Color.white;
        TA.tab.GetComponent<Button>().enabled = true;
        //Debug.Log("Deactivated " + TA.item.name);
    }
}