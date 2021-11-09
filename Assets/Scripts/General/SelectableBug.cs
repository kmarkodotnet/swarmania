using UnityEngine;

public class SelectableBug : SelectableObject
{
    protected override int GetId()
    {
        return gameObject.GetComponent<Bug>().GetId();
    }

    protected override void SetIcons(Sprite sprite, int id)
    {
    }

    protected override string GetIconGameObjectName()
    {
        return "Bug";
    }

    public void SetPosition(Vector3 position)
    {
        if (selection)
        {
            selection.transform.position = position;
        }
    }
}
