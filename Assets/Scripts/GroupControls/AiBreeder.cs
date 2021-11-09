using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiBreeder : MonoBehaviour
{
    [SerializeField] List<GameObject> bugPrefabs;
    [SerializeField] List<int> playerWeights;
    [SerializeField] int maxSteps = 100;
    [SerializeField] int maxN = 1;
    [SerializeField] int maxM = 1;
    [SerializeField] int randomRange = 0;

    int currentStep = 0;
    int currentBreed = 0;
    BugTypeEnum[] bugTypes;
    private void Awake()
    {
        var c = FindObjectsOfType<AiBreeder>().Length;
        if (c > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        bugTypes = bugPrefabs.Select(b => b.GetComponent<AttackableBug>().GetBugType()).ToArray();
    }

    private void Update()
    {
        if (maxSteps <= currentStep)
        {
            Breed();
            currentStep = 0;
        }
        else
        {
            currentStep++;
        }
    }
    public void Clear()
    {
        FindObjectOfType<AiEntityCollection>().AwardHordePool();
        FindObjectOfType<AiEntityCollection>().ClearBugs();
        FindObjectOfType<AiEntityCollection>().ResetHordePool();

        FindObjectOfType<AiEntityCollection>().EndHordePoolEpisode();
    }

    public int GetMaxN()
    {
        return maxN;
    }
    public int GetMaxM()
    {
        return maxM;
    }
    public int GetPlayersNum()
    {
        return playerWeights.Count;
    }

    public int GetCurrentBreed()
    {
        return currentBreed;
    }

    public void Breed()
    {
        currentBreed++;

        Clear();

        for (int n = 0; n < maxN; n++)
        {
            for (int m = 0; m < maxM; m++)
            {
                var deltaN = 35 * n;
                var deltaM = 25 * m;

                var j = 0;
                j = Random.Range(0, 3);
                for (int i = 0; i < playerWeights.Count; i++)
                {
                    var r1 = Random.Range(-randomRange, randomRange);
                    var index = i;
                    Breed(index, playerWeights[index] + r1, deltaN, deltaM);
                }
            }
        }

        System.Threading.Thread.Sleep(2);
        FindObjectOfType<AiEntityCollection>().CalculateHordes();

    }
    public void Breed(int playerIndex, int playerWeight, int deltaN, int deltaM)
    {
        for (int i = 0; i < playerWeight; i++)
        {
            int j = Random.Range(0, 5);
            BreedOne(bugPrefabs[j], playerIndex, deltaN, deltaM);
        }
    }

    private void BreedOne(GameObject gameObject, int playerIndex, int deltaN, int deltaM)
    {
        var p = gameObject.transform.position;
        var bug = Instantiate(gameObject, new Vector3(p.x + Random.Range(-2, 2) + 3 * playerIndex + deltaN, p.y + Random.Range(-2, 2) + deltaM, p.z), Quaternion.identity);
        bug.GetComponent<SelectableObject>().SetOwnerId(playerIndex.ToString());

        bug.GetComponentInChildren<SpriteRenderer>().color = playerIndex == 0 ? Color.white : Color.blue;
        var c = bug.GetComponentInChildren<SpriteRenderer>().color;
        bug.GetComponentInChildren<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 1);

        FindObjectOfType<AiEntityCollection>().AddBug(bug);
    }
}
