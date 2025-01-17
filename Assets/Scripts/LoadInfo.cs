using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoadInfo : MonoBehaviour
{
    private GameObject infoPlant;
    [HideInInspector] public string currentTarget;
    private GameObject currentObject;
    public void DisplayInfo()
    {
        Debug.Log(currentTarget);
        currentObject = GameObject.FindGameObjectWithTag(currentTarget);
        infoPlant = currentObject.transform.GetChild(0).gameObject;
        infoPlant.SetActive(true);
        infoPlant.GetComponent<InfoAudio>().VoiceOver.Play();
        Debug.Log(infoPlant.name);
    }

    public void UpdateTargetDiscoBall()
    {
        currentTarget = "DiscoBall";
        Debug.Log(currentTarget);
    }
    public void UpdateTargetAloe()
    {
        currentTarget = "Aloe";
        Debug.Log(currentTarget);
    }
    public void UpdateTargetBasil()
    {
        currentTarget = "Basil";
        Debug.Log(currentTarget);
    }
    public void UpdateTargetCoriander()
    {
        currentTarget = "Coriander";
        Debug.Log(currentTarget);
    }
    public void UpdateTargetBamboo()
    {
        currentTarget = "Bamboo";
        Debug.Log(currentTarget);
    }
    public void UpdateTargetLemon()
    {
        currentTarget = "Lemon";
        Debug.Log(currentTarget);
    }
}
