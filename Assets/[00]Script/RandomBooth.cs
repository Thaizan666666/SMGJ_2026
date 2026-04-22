using UnityEngine;
using System.Collections.Generic;

public class RandomBooth : MonoBehaviour
{
    [System.Serializable]
    public class BoothEntry
    {
        public GameObject Booth;
        [Range(0f, 100f)] public float Weight = 100f;
    }

    public List<BoothEntry> BoothList = new List<BoothEntry>();

    public void Awake()
    {
        GameObject selected = GetRandomBooth();
        if (selected != null)
        {
            Instantiate(selected, this.transform.position, Quaternion.identity);
        }
        Destroy(this.gameObject);
    }

    private GameObject GetRandomBooth()
    {
        float totalWeight = 0f;
        foreach (var entry in BoothList)
            totalWeight += entry.Weight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in BoothList)
        {
            cumulative += entry.Weight;
            if (roll < cumulative)
                return entry.Booth;
        }

        return null;
    }
}