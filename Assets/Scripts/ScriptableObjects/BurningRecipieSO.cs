using UnityEngine;


[CreateAssetMenu]
public class BurningRecipieSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float burningTimerMax;
}

