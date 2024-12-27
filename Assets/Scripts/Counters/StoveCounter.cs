using UnityEngine;
using System;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipieSO[] fryingRecipieSOArray;
    [SerializeField] private BurningRecipieSO[] burningRecipieSOArray;

    private float fryingTimer;
    private float burningTimer;
    private FryingRecipieSO fryingRecipieSO;
    private BurningRecipieSO burningRecipieSO;
    private State currentState;

    private void Start()
    {
        currentState = State.Idle;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                break;

            case State.Frying:
                if (fryingRecipieSO == null) return;

                fryingTimer += Time.deltaTime;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalised = fryingTimer / fryingRecipieSO.fryingTimerMax
                });

                if (fryingTimer >= fryingRecipieSO.fryingTimerMax)
                {
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(fryingRecipieSO.output, this);
                    currentState = State.Fried;

                    burningRecipieSO = GetBurningRecipieForInput(GetKitchenObject().GetKitchenObjectSO());
                    burningTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });
                }
                break;

            case State.Fried:
                if (burningRecipieSO == null) return;

                burningTimer += Time.deltaTime;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalised = burningTimer / burningRecipieSO.burningTimerMax
                });

                if (burningTimer >= burningRecipieSO.burningTimerMax)
                {
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(burningRecipieSO.output, this);
                    currentState = State.Burned;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalised = 0f
                    });
                }
                break;

            case State.Burned:
                // No further state change after burned
                break;
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipieWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipieSO = GetFryingRecipieForInput(GetKitchenObject().GetKitchenObjectSO());
                    fryingTimer = 0f;
                    currentState = State.Frying;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalised = 0f
                    });
                }
                else
                {

                }
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
                        currentState = State.Idle;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalised = 0f
                        });
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                currentState = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalised = 0f
                });
            }
        }
    }

    private bool HasRecipieWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipieSO fryingRecipieSO in fryingRecipieSOArray)
        {
            if (fryingRecipieSO.input == inputKitchenObjectSO)
            {
                return true;
            }
        }
        return false;
    }

    private FryingRecipieSO GetFryingRecipieForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipieSO fryingRecipieSO in fryingRecipieSOArray)
        {
            if (fryingRecipieSO.input == inputKitchenObjectSO)
            {
                return fryingRecipieSO;
            }
        }
        return null;
    }

    private BurningRecipieSO GetBurningRecipieForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipieSO burningRecipieSO in burningRecipieSOArray)
        {
            if (burningRecipieSO.input == inputKitchenObjectSO)
            {
                return burningRecipieSO;
            }
        }
        return null;
    }
}
