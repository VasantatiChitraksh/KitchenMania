using UnityEngine;
using System;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipieTemplate;

    private void Awake()
    {
        recipieTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipieSpawned += DeliveryManager_OnRecipieSpawned;
        DeliveryManager.Instance.OnRecipieCompleted += DeliveryManager_OnRecipieCompleted;

        UpdateVisual();
    }

    private void DeliveryManager_OnRecipieSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipieCompleted(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            if (child == recipieTemplate)
            {
                continue;
            }
            Destroy(child.gameObject);
        }

        foreach (RecipieSO recipieSO in DeliveryManager.Instance.GetWaitingRecipieSOList())
        {
            Transform recipieTransform = Instantiate(recipieTemplate, container);
            recipieTransform.gameObject.SetActive(true);
            recipieTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipieSO(recipieSO);
        };
    }
}
