using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    [SerializeField]
    private ResourceSource curResourceSource; //������·���Ҩз��� �������еѴ �ͧ���Тش
    public ResourceSource CurResourceSource { get { return curResourceSource; } set { curResourceSource = value; } }

    [SerializeField]
    private float gatherRate = 0.5f; //����������ҨеѴ���/�ش�ͧ �� �ء  0.5 ��
    [SerializeField]
    private int gatherAmount = 1; // An amount unit can gather every "gatherRate" second(s)  //�ӹǹ���еѴ/�ش��㹷ء gatherRate �� �ء 0.5 �� �ѹ 1 �� �� 1 �ѹ

    [SerializeField]
    private int amountCarry; //amount currently carried �ӹǹ�����������㹵��
    public int AmountCarry { get { return amountCarry; } set { amountCarry = value; } }

    [SerializeField]
    private int maxCarry = 25; //max amount to carry  �ӹǹ��������٧�ش
    public int MaxCarry { get { return maxCarry; } set { maxCarry = value; } }

    [SerializeField]
    private ResourceType carryType; //���ѧ carry ��Ѿ�ҡû������˹����
    public ResourceType CarryType { get { return carryType; } set { carryType = value; } }

    private float lastGatherTime;  //���ա�äӹǳ� code��͸Ժ���ա��
    private Unit unit; //script unit ����ͧ��Ŵ���ѹ


    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (unit.State)
        {
            case UnitState.MoveToResource:
                MoveToResourceUpdate();
                break;
            case UnitState.Gather:
                GatherUpdate();
                break;
            case UnitState.DeliverToHQ:
                DeliverToHQUpdate();
                break;
            case UnitState.StoreAtHQ:
                StoreAtHQUpdate();
                break;
        }
    }
    // move to a resource and begin to gather it
    public void ToGatherResource(ResourceSource resource, Vector3 pos)
    {
        curResourceSource = resource;

        //if gather a new type of resource, reset amount to 0
        if (curResourceSource.RsrcType != carryType)
        {
            carryType = CurResourceSource.RsrcType;
            amountCarry = 0;
        }

        unit.SetState(UnitState.MoveToResource);

        unit.NavAgent.isStopped = false;
        unit.NavAgent.SetDestination(pos);
    }

    private void MoveToResourceUpdate()
    {
        CheckForResource();
        
        if (Vector3.Distance(transform.position, unit.NavAgent.destination) <= 2f)
        {
            if (curResourceSource != null)
            {
                unit.LookAt(curResourceSource.transform.position);
                unit.NavAgent.isStopped = true;
                unit.SetState(UnitState.Gather);
            }
        }
    }

    private void GatherUpdate()
    {
        //������ҷ����Թ��ҹ� ź�Ѻ ���ҷ��Ѵ����͹˹�ҹ��  �ҡ���� �ѵ�Ҥ������㹡�õѴ��� (=0.5��)        
        if (Time.time - lastGatherTime > gatherRate)
        {
            lastGatherTime = Time.time; //����鵵���� �����Ҥ����ش���·��Ѵ���

            //�������õѴ
            if (amountCarry < maxCarry) //��һ���ҳ�ͷ������������ �ѧ����Թ max�������
            {
                if (curResourceSource != null) //��ҵ�����ѧ���� ���µѴ
                {
                    curResourceSource.GatherResource(gatherAmount);

                    carryType = curResourceSource.RsrcType;
                    amountCarry += gatherAmount; //�����������ӹǹ���Ѵ����
                }
                else
                {
                    CheckForResource();
                }
            }
            else //amount is full, go back to deliver at HQ  �������������
                unit.SetState(UnitState.DeliverToHQ);
        }
    }

    private void DeliverToHQUpdate()
    {
        if (Time.time - unit.LastPathUpdateTime > unit.PathUpdateRate)
        {
            unit.LastPathUpdateTime = Time.time;

            unit.NavAgent.SetDestination(unit.Faction.GetHQSpawnPos());
            unit.NavAgent.isStopped = false;
        }

        if (Vector3.Distance(transform.position, unit.Faction.GetHQSpawnPos()) <= 1f)
            unit.SetState(UnitState.StoreAtHQ);
    }

    private void StoreAtHQUpdate()
    {
        unit.LookAt(unit.Faction.GetHQSpawnPos());  //�ѹ��ͧ

        if (amountCarry > 0) //����բͧ�����
        {
            // Deliver the resource to Faction
            unit.Faction.GainResource(carryType, amountCarry);
            amountCarry = 0; //���������� ���ͧ������ 0

            //Debug.Log("Delivered");
        }
        
        CheckForResource();
    }

    private void CheckForResource()
    {
        if (curResourceSource != null) //that resource still exists
            ToGatherResource(curResourceSource, curResourceSource.transform.position);
        else
        {
            //try to find a new resource
            curResourceSource = unit.Faction.GetClosestResource(transform.position, carryType);

            //CheckAgain, if found a new one, go to it
            if (curResourceSource != null)
                ToGatherResource(curResourceSource, curResourceSource.transform.position);
            else //can't find a new one
            {
                Debug.Log($"{unit.name} can't find a new tree");
                unit.SetState(UnitState.Idle);
            }
        }
    }



}

