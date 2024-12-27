using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipieSpawned;
    public event EventHandler OnRecipieCompleted;
    public event EventHandler OnRecipieSuccess;
    public event EventHandler OnRecipieFailed;
    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipieListSO recipieListSO;
    private List<RecipieSO> waitingRecipieSOList;
    private float spawnRecipieTimer;
    private float spawnRecipieTimerMax = 4f;
    private int waitingRecipiesMax = 4;
    private int successfulRecipiesAmount = 0;
    private void Awake()
    {
        waitingRecipieSOList = new List<RecipieSO>();
        Instance = this;
    }
    private void Update()
    {
        spawnRecipieTimer -= Time.deltaTime;
        if (spawnRecipieTimer <= 0f)
        {
            spawnRecipieTimer = spawnRecipieTimerMax;
            if (waitingRecipieSOList.Count < waitingRecipiesMax)
            {
                RecipieSO waitingRecipieSO = recipieListSO.recipieSOList[UnityEngine.Random.Range(0, recipieListSO.recipieSOList.Count)];
                waitingRecipieSOList.Add(waitingRecipieSO);

                OnRecipieSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipie(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipieSOList.Count; i++)
        {
            RecipieSO waitingRecipieSO = waitingRecipieSOList[i];
            if (waitingRecipieSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                //Has the same no of ingredients
                bool plateContentsMatchesRecipie = true;
                foreach (KitchenObjectSO recipieKitchenObjectSO in waitingRecipieSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;
                    //Cyclying Through the items on the recipie and comparing with plate
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        if (plateKitchenObjectSO == recipieKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound)
                    {
                        //This recipie ingredient was not on the plate
                        plateContentsMatchesRecipie = false;
                    }
                }

                if (plateContentsMatchesRecipie)
                {
                    //Player delivered the correct recipie
                    successfulRecipiesAmount++;
                    OnRecipieCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipieSuccess?.Invoke(this, EventArgs.Empty);
                    waitingRecipieSOList.RemoveAt(i);
                    return;
                }
            }
            else
            {
                //Nope
            }
        }
        //No matches
        OnRecipieFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipieSO> GetWaitingRecipieSOList()
    {
        return waitingRecipieSOList;
    }

    public int GetSuccessfulRecipiesAmount(){
        return successfulRecipiesAmount;
    }
}
