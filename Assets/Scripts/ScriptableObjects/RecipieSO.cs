using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu()]
public class RecipieSO : ScriptableObject
{
    public List<KitchenObjectSO> kitchenObjectSOList;
    public string recipieName;
}
