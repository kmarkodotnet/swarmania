using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Control : MonoBehaviour
{
    [SerializeField] Dictionary<string, Vector3> basePoint;
    [SerializeField] Dictionary<string, Vector3> baseSize;
    [SerializeField] GameObject bugTypePlaceholderPrefab;

    [SerializeField] Texture2D menuTexture;
    Dictionary<int, GameObject> selectedBugs;

    public static bool ControlClickLocked = false;
    GameObject SelectedCastle;

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
        if (SelectedCastle != null)
        { 
            var csh = SelectedCastle.GetComponent<CastleStateHandler>();
            RefreshBugInQueueCounters(new int[3] { csh.GetLevel1BugNumber(), csh.GetLevel2BugNumber(), csh.GetLevel3BugNumber() });
            GetBugCreationPercentage();
        }
    }

    private void FixedUpdate()
    {
        RefreshResourcesNumber(SelectedCastle);
    }

    private void GetBugCreationPercentage()
    {
        var csh = SelectedCastle.GetComponent<CastleStateHandler>();
        var typeAndPercentage = csh.GetActualBugTypeAndPercentage();
        if(typeAndPercentage != null && typeAndPercentage.Item2 < 1)
        {
            RefreshInprogressBugAndPercentage(typeAndPercentage.Item1, typeAndPercentage.Item2);
        }
        else
        {
            RefreshNoInprogressBug();
        }
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
    internal void RemoveResources()
    {
        var cs = new CommonService();
        var resources = cs.GetChildrenByName(transform, "resources");
        resources.gameObject.SetActive(false);
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
        ControlContext.SetupCastle(gameObject, bugPrefabs);
        SelectedCastle = gameObject;
        RefreshResourcesNumber(SelectedCastle);
    }

    private void RefreshResourcesNumber(GameObject selectedCastle)
    {
        if(selectedCastle != null && selectedCastle.GetComponent<CastleStateHandler>() != null)
        {
            var cs = new CommonService();
            var resources = cs.GetChildrenByName(transform, "resources");
            var canvas = cs.GetChildrenByName(resources, "Canvas");

            var resourceTypes = cs.GetResourceTypes();
            var ors = selectedCastle.GetComponent<CastleStateHandler>().GetOwnedResources();
            var x = 0;
            foreach (var item in resourceTypes)
            {
                switch (item)
                {
                    case ResourceTypeEnum.Honey:
                        var honeyText = cs.GetChildrenByName(canvas, "HoneyText");
                        honeyText.GetComponent<Text>().text = ors[x].ToString();
                        break;
                    case ResourceTypeEnum.Meat:
                        var meatText = cs.GetChildrenByName(canvas, "MeatText");
                        meatText.GetComponent<Text>().text = ors[x].ToString();
                        break;
                    case ResourceTypeEnum.Seed:
                        var seedText = cs.GetChildrenByName(canvas, "SeedText");
                        seedText.GetComponent<Text>().text = ors[x].ToString();
                        break;
                }
                x++;
            }
        }
    }

    internal void SetupResources()
    {
        var cs = new CommonService();
        var resources = cs.GetChildrenByName(transform, "resources");
        resources.gameObject.SetActive(true);
    }

    internal void ClearCastleBugs()
    {

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

            placeholder.localScale = new Vector3(1f, 1f, 1);

            placeholder.gameObject.SetActive(true);

            var sprite = selectedBug.Value.GetComponent<SelectableBug>().GetMenuIcon();
            var spriteHover = selectedBug.Value.GetComponent<SelectableBug>().GetMenuIconRo();

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
    private void RefreshNoInprogressBug()
    {
        RefreshInprogressBugAndPercentage(-1, 0);
    }
    private void RefreshInprogressBugAndPercentage(int num, float percent)
    {
        SetControl();
        var cs = new CommonService();
        var castle = cs.GetChildrenByName(transform, "castle");
        var bugTypeChooser = cs.GetChildrenByName(castle, "bugTypeChooser");
        for (int i = 0; i < 3; i++)
        {
            var bugSpritePlaceholder = cs.GetChildrenByName(bugTypeChooser, "bugTypePlaceholder" + i);
            var counter = cs.GetChildrenByName(bugSpritePlaceholder, "counter");
            var canvas = cs.GetChildrenByName(counter, "Canvas");
            var slider = cs.GetChildrenByName(canvas, "Slider");
            if(i == num - 1)
            {
                slider.gameObject.SetActive(true);
                slider.gameObject.GetComponent<Slider>().value = percent;
            }else
            {
                slider.gameObject.SetActive(false);
            }
        }
    }
    private void RefreshBugInQueueCounters(int[] nums)
    {
        SetControl();
        var cs = new CommonService();
        var castle = cs.GetChildrenByName(transform, "castle");
        var bugTypeChooser = cs.GetChildrenByName(castle, "bugTypeChooser");
        for (int i = 0; i < 3; i++)
        {
            var bugSpritePlaceholder = cs.GetChildrenByName(bugTypeChooser, "bugTypePlaceholder" + i);
            var counter = cs.GetChildrenByName(bugSpritePlaceholder, "counter");
            var canvas = cs.GetChildrenByName(counter, "Canvas");
            var text = cs.GetChildrenByName(canvas, "Text");
            text.gameObject.GetComponent<Text>().text = nums[i].ToString();
        }
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
            bugSpritePlaceholder.gameObject.SetActive(true);
            bugSpritePlaceholder.GetComponent<SpriteRenderer>().sprite = null;
            bugSpritePlaceholder.GetComponent<SpriteRenderer>().size = new Vector2(0.5f, 0.5f);
            var counter = cs.GetChildrenByName(bugSpritePlaceholder, "counter");
            var canvas = cs.GetChildrenByName(counter, "Canvas");
            var text = cs.GetChildrenByName(canvas, "Text");
            text.gameObject.GetComponent<Text>().text = "1";
            Debug.LogWarning($"canvas {canvas==null} - text {text == null} - text.gameObject.GetComponent<Text>() {text.gameObject.GetComponent<Text>() == null}");
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
            bugSpritePlaceholder.gameObject.SetActive(false);
        }
        RemoveResources();
        RemoveControl();
        SelectedCastle = null;
    }


    private void OnMouseOver()
    {
        var cs = new CommonService();
        var currentState = cs.GetCursorState();
        if (!ControlContext.IsContextActive() && currentState != CursorStateEnum.Default)
        {
            cs.SetCursorState(CursorStateEnum.Default, FindObjectOfType<Config>().GetDefaultCursorTexture());
        }
        if (Input.GetMouseButtonDown(0) && !Control.ControlClickLocked)
        {
            ControlContext.FinishContext();
            Control.ControlClickLocked = true;
        }
    }
}