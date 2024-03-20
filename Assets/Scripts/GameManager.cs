using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Faction myFaction;
    public Faction MyFaction { get { return myFaction; } }

    [SerializeField] private Faction enemyFaction;
    public Faction EnemyFaction { get { return enemyFaction; } }

    //All factions in this game (2 factions for now)
    [SerializeField] private Faction[] factions;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        MainUI.instance.UpdateAllResource(myFaction);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
