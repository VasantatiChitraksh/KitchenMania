using UnityEngine;


[CreateAssetMenu]
public class CuttingRecipieSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingProgressMax;
}

