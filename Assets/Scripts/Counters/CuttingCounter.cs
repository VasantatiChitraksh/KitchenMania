using UnityEngine;
using System;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;
    public static event EventHandler OnAnyCut;

    [SerializeField] private CuttingRecipieSO[] cuttingRecipieSOArray;
    private int cuttingProgress;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                // Player is carrying something
                if (HasRecipieWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    // Player has a cuttable ingredient
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingRecipieSO cuttingRecipieSO = GetRecipieForInput(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalised = (float)cuttingProgress / cuttingRecipieSO.cuttingProgressMax
                    });
                }
            }
            else
            {
                // Player doesn't have anything
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
            else
            {
                if (HasKitchenObject())
                {
                    GetKitchenObject().SetKitchenObjectParent(player);
                }
                else
                {
                    // Player has nothing and counter is empty
                }
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipieWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CuttingRecipieSO cuttingRecipieSO = GetRecipieForInput(GetKitchenObject().GetKitchenObjectSO());

            if (cuttingProgress >= cuttingRecipieSO.cuttingProgressMax)
            {
                return;
            }

            cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalised = (float)cuttingProgress / cuttingRecipieSO.cuttingProgressMax
            });

            // Check again if the cutting progress has reached the maximum
            if (cuttingProgress == cuttingRecipieSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                // Destroy the current KitchenObject
                GetKitchenObject().DestroySelf();

                // Spawn the new KitchenObject and set it to the counter
                KitchenObject newKitchenObject = KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }

    private bool HasRecipieWithInput(KitchenObjectSO inputkitchenObjectSO)
    {
        foreach (CuttingRecipieSO cuttingRecipieSO in cuttingRecipieSOArray)
        {
            if (cuttingRecipieSO.input == inputkitchenObjectSO)
            {
                return true;
            }
        }
        return false;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipieSO cuttingRecipieSO = GetRecipieForInput(inputKitchenObjectSO);
        if (cuttingRecipieSO != null)
        {
            return cuttingRecipieSO.output;
        }
        else
        {
            return null;
        }
    }

    private CuttingRecipieSO GetRecipieForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipieSO cuttingRecipieSO in cuttingRecipieSOArray)
        {
            if (cuttingRecipieSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipieSO;
            }
        }
        return null;
    }
}
