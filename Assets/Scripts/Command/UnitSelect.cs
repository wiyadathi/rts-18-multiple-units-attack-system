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
    private List<Unit> curUnits = new List<Unit>(); //current selected single unit
    public List<Unit> CurUnits { get { return curUnits; } }
    
    [SerializeField]
    private Building curBuilding; //current selected single building
    public Building CurBuilding { get { return curBuilding; } }

    [SerializeField]
    private ResourceSource curResource; //current selected resource
    
    [SerializeField] private Unit curEnemy;
    
    [SerializeField]
    private RectTransform selectionBox;
    private Vector2 oldAnchoredPos;//Box old anchored position
    private Vector2 startPos;//point where mouse is down

    
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

        selectionBox = MainUI.instance.SelectionBox;
        
        instance = this;
    }


    // *****************************************
    void Update()
    {
        //mouse down
                if (Input.GetMouseButtonDown(0))
                {
                    startPos = Input.mousePosition;
                    
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    
                    ClearEverything();
                }
        
                //Drag mouse
                if (Input.GetMouseButton(0))
                {
                    UpdateSelectionBox(Input.mousePosition);
                }
                
                // mouse up
                if (Input.GetMouseButtonUp(0))
                {
                   //release mouse after drag
                   ReleaseSelectionBox(Input.mousePosition);
                    
                    Debug.Log("1 - mouse up - try select");
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
        else //single enemy
        {
            curEnemy = unit;
            curEnemy.ToggleSelectionVisual(true);
            ShowEnemyUnit(unit);
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
        
        if (curEnemy != null)
        {
            curEnemy.ToggleSelectionVisual(false);
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

        private void UpdateSelectionBox(Vector2 mousePos)
        {
            //Debug.Log("Mouse Pos - " + curMousePos);
            if (!selectionBox.gameObject.activeInHierarchy && curBuilding == null)
                selectionBox.gameObject.SetActive(true);

            float width = mousePos.x - startPos.x;
            float height = mousePos.y - startPos.y;

            selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            selectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);

            //store old position for real unit selection
            oldAnchoredPos = selectionBox.anchoredPosition;
        }
        
        private void ReleaseSelectionBox(Vector2 mousePos)
        {
            //Debug.Log("Step 2 - " + _doubleClickMode);
            Vector2 min; //down-left corner
            Vector2 max; //top-right corner

            selectionBox.gameObject.SetActive(false);

            min = oldAnchoredPos - (selectionBox.sizeDelta / 2);
            max = oldAnchoredPos + (selectionBox.sizeDelta / 2);

            //Debug.Log("min = " + min);
            //Debug.Log("max = " + max);

            foreach (Unit unit in GameManager.instance.MyFaction.AliveUnits)
            {
                Vector2 unitPos = cam.WorldToScreenPoint(unit.transform.position);

                if (unitPos.x > min.x && unitPos.x < max.x && unitPos.y > min.y && unitPos.y < max.y)
                {
                    curUnits.Add(unit);
                    unit.ToggleSelectionVisual(true);
                }
            }
            selectionBox.sizeDelta = new Vector2(0, 0); //clear Selection Box's size;
        }

        void ShowEnemyUnit(Unit u)
        {
            InfoManager.instance.ShowEnemyAllInfo(u);
        }
}
