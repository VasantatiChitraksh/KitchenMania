using UnityEngine;


[CreateAssetMenu]
public class FryingRecipieSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float fryingTimerMax;
}

