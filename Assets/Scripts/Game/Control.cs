using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    [SerializeField] Dictionary<string, Vector3> basePoint;
    [SerializeField] Dictionary<string, Vector3> baseSize;
    [SerializeField] GameObject bugTypePlaceholderPrefab;

    [SerializeField] Texture2D menuTexture;
    Dictionary<int, GameObject> selectedBugs;

    public static bool ControlClickLocked = false;


    private void Awake()
    {
        var c = FindObjectsOfType<Control>().Length;
        if (c > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    //public bool IsSelectionActive()
    //{
    //    return selectedBugs != null && selectedBugs.Count > 0;
    //}

    //private void OnGUI()
    //{
    //    //if (selectionMemory)
    //    //{
    //    //    FindObjectOfType<SelectionCoreComponent>().SetMemoryBack(selectedBugsMemory, selectedCastleMemory);
    //    //}
    //    if(selectedBugs != null && selectedBugs.Count > 0)
    //    {
    //        GUIStyle container = new GUIStyle();
    //        container.normal.background = menuTexture;

    //        var width = menuTexture.width * (0.8f);
    //        var height = menuTexture.height * (0.6f);

    //        GUI.Box(new Rect(Screen.width / 2 - (width / 2), Screen.height - (height + 30), width, height), "", container);

    //        int baseXOffset = 100;
    //        int baseYOffset = 100;
    //        int offset = 48;
    //        int j = 0;
    //        foreach (var item in selectedBugs)
    //        {
    //            var bugGO = item.Value;
    //            GUIStyle icon = new GUIStyle();
    //            icon.normal.background = bugGO.GetComponent<SelectableBug>().GetMenuIcon();
    //            icon.hover.background = bugGO.GetComponent<SelectableBug>().GetMenuIconRo();

    //            if (GUI.Button(new Rect(Screen.width / 2 - (width / 2) + j * offset + baseXOffset, Screen.height - (height + 30) + + baseYOffset, offset - 1, offset - 1), "", icon))
    //            {
    //                Debug.LogError("sdfsfsdf");
    //            }

    //            j++;
    //        }
    //    }
    //}

    //Dictionary<int, GameObject> selectedBugsMemory;
    //GameObject selectedCastleMemory;
    //bool selectionMemory = false;
    //internal void SetSelectionMemory(Dictionary<int, GameObject> selectedBugs, GameObject selectedCastle)
    //{
    //    selectedBugsMemory = selectedBugs;
    //    selectedCastleMemory = selectedCastle;
    //    selectionMemory = true;
    //}

    private void Start()
    {
        basePoint = new Dictionary<string, Vector3>();
        baseSize = new Dictionary<string, Vector3>();
        var srs = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in srs)
        {
            var path = sr.GetPath();
            basePoint.Add(path, Camera.main.WorldToViewportPoint(sr.transform.position));
            var size = (Camera.main.transform.position - sr.transform.position).magnitude / Camera.main.orthographicSize;
            var defaultSize = 2f;
            if (path.Contains("bugTypePlaceholder"))
            {
                defaultSize = 1f;
            }
            baseSize.Add(path, new Vector3(size * defaultSize, size * defaultSize, size));
        }

        RemoveBugIcons();
        RemoveCastleIcon();
    }

    void Update()
    {
        var srs = gameObject.GetComponentsInChildren<SpriteRenderer>();//.ToList().Where(sr => sr.gameObject.tag == "castleBaseSprite").ToArray();
        for (int i = 0; i < srs.Length; i++)
        {
            var path = srs[i].GetPath();
            srs[i].transform.position = Camera.main.ViewportToWorldPoint(basePoint[path]);
            srs[i].transform.localScale = (baseSize[path] * Camera.main.orthographicSize) / 10;
        }
        //foreach (var sr in srs)
        //{
        //    if(Camera.main.orthographicSize> 0)
        //        sr.transform.localScale = new Vector3(20 * (1 / Camera.main.orthographicSize), 20 * (1 / Camera.main.orthographicSize), sr.transform.localScale.z);
        //}
    }


    private void SetActionHarvestIcon(bool active)
    {
        SetCommandIcon("harvestCommand", active);
    }

    private void SetActionAttackIcon(bool active)
    {
        SetCommandIcon("attackCommand", active);
    }

    private void SetActionMoveIcon(bool active)
    {
        SetCommandIcon("moveCommand", active);
    }

    private void SetActionStopIcon(bool active)
    {
        SetCommandIcon("stopCommand", active);
    }
    private void SetCommandIcon(string commandIconName, bool active)
    {
        var cs = new CommonService();
        var bugs = cs.GetChildrenByName(transform, "bugs");
        var commands = cs.GetChildrenByName(bugs, "commands");
        var commandIcon = cs.GetChildrenByName(commands, commandIconName);
        commandIcon.gameObject.SetActive(active);
    }

    public void SetControl()
    {
        var cs = new CommonService();
        var background = cs.GetChildrenByName(transform, "background");
        background.gameObject.SetActive(true);
    }
    public void RemoveControl()
    {
        var cs = new CommonService();
        var background = cs.GetChildrenByName(transform, "background");
        background.gameObject.SetActive(false);
    }

    internal void SetupCastleBugs(GameObject gameObject)
    {
        var cs = new CommonService();
        var castle = cs.GetChildrenByName(transform, "castle");
        var bugTypeChooser = cs.GetChildrenByName(castle, "bugTypeChooser");
        GameObject[] bugPrefabs = gameObject.GetComponent<CastleStateHandler>().GetBugPrefabs();
        for (int i = 0; i < bugPrefabs.Length; i++)
        {
            var sprite = bugPrefabs[i].GetComponent<SelectableBug>().GetMenuIcon();
            var spriteHover = bugPrefabs[i].GetComponent<SelectableBug>().GetMenuIconRo();

            var bugSpritePlaceholder = cs.GetChildrenByName(bugTypeChooser, "bugTypePlaceholder" + i);

            bugSpritePlaceholder.GetComponent<BaseCommand>().SetSprite(sprite);
            bugSpritePlaceholder.GetComponent<BaseCommand>().SetSpriteHover(spriteHover);

            bugSpritePlaceholder.GetComponent<SpriteRenderer>().sprite = sprite;
            bugSpritePlaceholder.GetComponent<SpriteRenderer>().size = new Vector2(0.5f, 0.5f);
        }
        Context.SetupCastle(gameObject, bugPrefabs);
    }

    internal void ClearCastleBugs()
    {

    }

    public void RemoveBugIcon(int id)
    {
        SetBugIcon(null, id);
    }

    internal void SetBugIcons(Dictionary<int, GameObject> selectedBugs)
    {        
        SetControl();
        this.selectedBugs = selectedBugs;
        var cs = new CommonService();

        var bugs = cs.GetChildrenByName(transform, "bugs");
        bugs.gameObject.SetActive(true);

        var iconsPlaceholder = cs.GetChildrenByName(bugs, "iconsPlaceholder");
        for (int i = selectedBugs.Count; i < 20; i++)
        {
            var placeholder = cs.GetChildrenByName(iconsPlaceholder, "bugTypePlaceholder" + i);
            placeholder.gameObject.SetActive(false);
        }

        int j = 0;
        foreach (var selectedBug in selectedBugs)
        {
            var placeholder = cs.GetChildrenByName(iconsPlaceholder, "bugTypePlaceholder" + j);
            placeholder.gameObject.SetActive(true);

            //var bugIcon = cs.GetChildrenByName(placeholder, "bugIcon");
            placeholder.localScale = new Vector3(1f, 1f, 1);

            placeholder.gameObject.SetActive(true);

            var sprite = selectedBug.Value.GetComponent<SelectableBug>().GetMenuIcon();
            var spriteHover = selectedBug.Value.GetComponent<SelectableBug>().GetMenuIconRo();
            //var bug = cs.GetChildrenByName(selectedBug.Value.transform, "Bug");
            //var sprite = bug.GetComponent<SpriteRenderer>().sprite;

            //var bugHover = cs.GetChildrenByName(selectedBug.Value.transform, "BugHover");
            //var spriteHover = bug.GetComponent<SpriteRenderer>().sprite;

            placeholder.GetComponent<BaseCommand>().SetSprite(sprite);
            placeholder.GetComponent<BaseCommand>().SetSpriteHover(spriteHover);

            placeholder.GetComponent<SpriteRenderer>().sprite = sprite;            
            placeholder.GetComponent<SpriteRenderer>().size = new Vector2(0.2f, 0.2f);

            j++;
        }

        var castle = cs.GetChildrenByName(transform, "castle");
        if (castle.gameObject.activeSelf)
        {
            RemoveCastleIcon();
        }

        SetActionStopIcon(true);
        SetActionMoveIcon(true);
        SetActionAttackIcon(true);
        SetActionHarvestIcon(true);
    }

    public void SetBugIcon(Sprite bugSprite, int id)
    {
        //var cs = new CommonService();

        //var castle = cs.GetChildrenByName(transform, "castle");
        //if(castle.gameObject.activeSelf)
        //{
        //    RemoveCastleIcon();
        //}

        //var bugs = cs.GetChildrenByName(transform, "bugs");
        //bugs.gameObject.SetActive(transform);

        //var iconsPlaceholder = cs.GetChildrenByName(bugs, "iconsPlaceholder");
        //int index = GetNextEmptyPlaceholderIndex(iconsPlaceholder);
        //var bugTypePlaceholder = cs.GetChildrenByName(iconsPlaceholder, "bugTypePlaceholder" + index);

        //bugTypePlaceholder.gameObject.GetComponent<BugPlaceholder>().SetId(id);
        //var bugIcon = cs.GetChildrenByName(bugTypePlaceholder, "bugIcon");
        //bugTypePlaceholder.localScale = new Vector3(1f, 1f, 1);

        //bugIcon.GetComponent<SpriteRenderer>().sprite = bugSprite;
        //bugIcon.GetComponent<SpriteRenderer>().size = new Vector2(0.5f, 0.5f);
    }

    private int GetNextEmptyPlaceholderIndex(Transform iconsPlaceholder)
    {
        var cs = new CommonService();
        int? placeholderIndex = null;
        for (int i = 0; i < 20; i++)
        {
            var placeholder = cs.GetChildrenByName(iconsPlaceholder, "bugTypePlaceholder" + i);
            var id = placeholder.gameObject.GetComponent<BugPlaceholder>().GetId();
            if (!id.HasValue && !placeholderIndex.HasValue)
            {
                placeholderIndex = i;
            }
        }
        return placeholderIndex.Value;
    }


    public void RemoveBugIcons()
    {
        var cs = new CommonService();

        var bugs = cs.GetChildrenByName(transform, "bugs");
        bugs.gameObject.SetActive(transform);

        var iconsPlaceholder = cs.GetChildrenByName(bugs, "iconsPlaceholder");
        for (int i = 0; i < 20; i++)
        {
            var placeholder = cs.GetChildrenByName(iconsPlaceholder, "bugTypePlaceholder" + i);
            placeholder.gameObject.SetActive(false);
        }

        SetActionStopIcon(false);
        SetActionMoveIcon(false);
        SetActionAttackIcon(false);
        SetActionHarvestIcon(false);

        RemoveControl();
    }
    

    public void SetupCastleIcon(Sprite castleSprite)
    {
        SetControl();
        var cs = new CommonService();
        var bugs = cs.GetChildrenByName(transform, "bugs");
        bugs.gameObject.SetActive(false);
        var castle = cs.GetChildrenByName(transform, "castle");
        castle.gameObject.SetActive(true);
        var iconPlaceholder = cs.GetChildrenByName(castle, "iconPlaceholder");
        var castleIcon = cs.GetChildrenByName(iconPlaceholder, "castleIcon");

        castleIcon.GetComponent<SpriteRenderer>().sprite = castleSprite;
        castleIcon.GetComponent<SpriteRenderer>().size = new Vector2(1, 1);

        var bugTypeChooser = cs.GetChildrenByName(castle, "bugTypeChooser");
        for (int i = 0; i < 3; i++)
        {
            var bugSpritePlaceholder = cs.GetChildrenByName(bugTypeChooser, "bugTypePlaceholder" + i);
            //var bugIcon = cs.GetChildrenByName(bugSpritePlaceholder.transform, "bugIcon");
            bugSpritePlaceholder.gameObject.SetActive(true);
            bugSpritePlaceholder.GetComponent<SpriteRenderer>().sprite = null;
            bugSpritePlaceholder.GetComponent<SpriteRenderer>().size = new Vector2(0.5f, 0.5f);
        }
    }

    public void RemoveCastleIcon()
    {
        var cs = new CommonService();
        var bugs = cs.GetChildrenByName(transform, "bugs");
        bugs.gameObject.SetActive(false);
        var castle = cs.GetChildrenByName(transform, "castle");
        castle.gameObject.SetActive(false);
        var iconPlaceholder = cs.GetChildrenByName(castle, "iconPlaceholder");
        var castleIcon = cs.GetChildrenByName(iconPlaceholder, "castleIcon");

        castleIcon.GetComponent<SpriteRenderer>().sprite = null;
        castleIcon.GetComponent<SpriteRenderer>().size = new Vector2(1, 1);

        var bugTypeChooser = cs.GetChildrenByName(castle, "bugTypeChooser");
        for (int i = 0; i < 3; i++)
        {
            var bugSpritePlaceholder = cs.GetChildrenByName(bugTypeChooser, "bugTypePlaceholder" + i);
            //var bugIcon = cs.GetChildrenByName(bugSpritePlaceholder.transform, "bugIcon");
            bugSpritePlaceholder.gameObject.SetActive(false);
        }
        RemoveControl();
    }


    private void OnMouseOver()
    {
        var cs = new CommonService();
        var currentState = cs.GetCursorState();
        if (!Context.IsContextActive() && currentState != CursorStateEnum.Default)
        {
            cs.SetCursorState(CursorStateEnum.Default, FindObjectOfType<Config>().GetDefaultCursorTexture());
        }
        if (Input.GetMouseButtonDown(0) && !Control.ControlClickLocked)
        {
            Context.FinishContext();
            Control.ControlClickLocked = true;
        }
    }
}