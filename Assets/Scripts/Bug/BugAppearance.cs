using UnityEngine;

public class BugAppearance : MonoBehaviour
{
    [SerializeField] GameObject bug;

    private void Start()
    {
        SetVisible();
    }

    public void SetVisible()
    {
        var c = bug.GetComponentInChildren<SpriteRenderer>().color;
        bug.GetComponentInChildren<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 1);
    }
    public void SetHidden()
    {
        var c = bug.GetComponentInChildren<SpriteRenderer>().color;
        bug.GetComponentInChildren<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 0);
    }
}
