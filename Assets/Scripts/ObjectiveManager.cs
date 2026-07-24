using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    private readonly HashSet<IObjective> activeObjectives = new HashSet<IObjective>();
    private int totalObjectives;

    public int Remaining { get { return activeObjectives.Count; } }
    public int Total { get { return totalObjectives; } }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterObjective(IObjective objective)
    {
        if (activeObjectives.Add(objective))
        {
            totalObjectives++;
            RefreshUI();
        }
    }

    public void NotifyDestroyed(IObjective objective)
    {
        if (activeObjectives.Remove(objective))
        {
            RefreshUI();

            if (activeObjectives.Count <= 0 && totalObjectives > 0 && GameManager.Instance != null)
            {
                GameManager.Instance.EndGame(true);
            }
        }
    }

    private void RefreshUI()
    {
        if (HUDController.Instance != null)
        {
            HUDController.Instance.SetObjectives(Remaining, Total);
        }
    }
}