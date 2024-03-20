using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainUI : MonoBehaviour
{
 
    [SerializeField] private GameObject selectionMarker;
    public GameObject SelectionMarker
    {
        get { return selectionMarker; }
    }
    
    [SerializeField] private TextMeshProUGUI unitCountText;
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI stoneText;
    
    [SerializeField] private RectTransform selectionBox;
    public RectTransform SelectionBox { get { return selectionBox; } }

    public static MainUI instance;
    
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void UpdateAllResource(Faction faction)
    {
        unitCountText.text = faction.AliveUnits.Count.ToString();
        foodText.text = faction.Food.ToString();
        woodText.text = faction.Wood.ToString();
        goldText.text = faction.Gold.ToString();
        stoneText.text = faction.Stone.ToString();
    }

    
}
