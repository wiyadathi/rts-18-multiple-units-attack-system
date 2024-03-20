using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelect : MonoBehaviour
{
    
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private List<Unit> curUnits; //current selected single unit
    public List<Unit> CurUnits { get { return curUnits; } }
    
    [SerializeField]
    private Building curBuilding; //current selected single building
    public Building CurBuilding { get { return curBuilding; } }

    [SerializeField]
    private ResourceSource curResource; //current selected resource

    private Camera cam;
    private Faction faction;

    public static UnitSelect instance;
    
    void Awake()
    {
        faction = GetComponent<Faction>();
    }

    void Start()
    {
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Unit", "Building", "Resource", "Ground");

        instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        //mouse down
                if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    
                    ClearEverything();
                }
        
                // mouse up
                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log("1");
                    TrySelect(Input.mousePosition);
                }

    }
    
    private void SelectUnit(RaycastHit hit)
    {
        Unit unit = hit.collider.GetComponent<Unit>();

        Debug.Log("Selected Unit");

        if (GameManager.instance.MyFaction.IsMyUnit(unit))
        {
            curUnits.Add(unit);
            unit.ToggleSelectionVisual(true);
            ShowUnit(unit);
        }
    }
    
    private void TrySelect(Vector2 screenPos)
    {
        Debug.Log("2");
        
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        //if we left-click something
        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            Debug.Log("3");
            
            switch (hit.collider.tag)
            {
                case "Unit":
                    SelectUnit(hit);
                    break;
                case "Building":    
                    BuildingSelect(hit);    
                    break;
                case "Resource":
                    ResourceSelect(hit);
                    break;
                
            }
        }
    }

    private void ClearAllSelectionVisual()
    {
        foreach (var u in  curUnits)
        {
            u.ToggleSelectionVisual(false);
        }

        if (curBuilding != null)
        {
            curBuilding.ToggleSelectionVisual(false);
        }

        if (curResource != null)
        {
            curResource.ToggleSelectionVisual(false);
        }

    }

    private void ClearEverything()
        {
            ClearAllSelectionVisual();
            curUnits.Clear();
            curBuilding = null;

            //Clear UI
            InfoManager.instance.ClearAllInfo();
            ActionManager.instance.ClearAllInfo();
        }

        private void ShowBuilding(Building b)
        {
            InfoManager.instance.ShowAllInfo(b);
            ActionManager.instance.ShowCreateUnitMode(b);
        }

        private void BuildingSelect(RaycastHit hit)
        {
            curBuilding = hit.collider.GetComponent<Building>();
            curBuilding.ToggleSelectionVisual(true);

            if (GameManager.instance.MyFaction.IsMyBuilding(curBuilding))
            {
                //Debug.Log("my building");
                ShowBuilding(curBuilding); //Show building info
            }
        }

        private void ShowUnit(Unit u)
        {
            InfoManager.instance.ShowAllInfo(u);

            if (u.IsBuilder)  //ถ้า unit เป็น Builder ให้้โชว์ปุ่มสร้างบ้าน
            {
                ActionManager.instance.ShowBuilderMode(u);
            }
        }
        
        private void ShowResource()
        {
            InfoManager.instance.ShowAllInfo(curResource);//Show resource info in Info Panel
        }
        
        private void ResourceSelect(RaycastHit hit)
        {
            curResource = hit.collider.GetComponent<ResourceSource>();
            if (curResource == null)
                return;

            curResource.ToggleSelectionVisual(true);
            ShowResource();//Show resource info
        }

}
