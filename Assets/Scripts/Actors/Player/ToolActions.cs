using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolActions : MonoBehaviour
{
    public Transform target;
    public Vector3Int location;
    Transform previousTarget;
    Camera playerCam;

    public GameObject uiData;
    public TextMeshProUGUI dataText;
    public CursorIcons.TaskList currentTaskSelected;

    public Material cursorMat;
    public CursorIcons[] symbols;
    CursorDraw playerCursorOverlay;

    // Start is called before the first frame update
    void Start()
    {
        if(PMovement.Player != null)
        {
            playerCam = PMovement.Player.GetPlayerCam();
        }

        uiData.SetActive(false);
        playerCursorOverlay = new CursorDraw(cursorMat, symbols, GetComponent<TaskManager>());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (playerCam == null && PMovement.Player != null) playerCam = PMovement.Player.GetPlayerCam();
            if (playerCam != null)
            {
                target = newTarget();

                if(target == null)
                {
                    location = CheckTerrainSpot();
                    if (currentTaskSelected != CursorIcons.TaskList.None)
                    {
                        playerCursorOverlay.AddTask(location, (int)currentTaskSelected - 1);
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            target = null;
        }

        foreach(CursorIcons hotkey in symbols)
        {
            if(Input.GetKeyDown(hotkey.primaryButton) || Input.GetKeyDown(hotkey.alternateButton))
            {
                if (currentTaskSelected == hotkey.taskAction) currentTaskSelected = CursorIcons.TaskList.None;
                else currentTaskSelected = hotkey.taskAction;
            }
        }

        UpdateUITips();
    }

    public void TaskOver(TaskManager.TaskListing tasking)
    {
        playerCursorOverlay.AddTask(new Vector3Int(tasking.location.x, 0, tasking.location.y), (int)CursorIcons.TaskList.Clear - 1);
    }

    void UpdateUITips()
    {
        if(target == null)
        {
            if(uiData.activeSelf) uiData.SetActive(false);
            dataText.text = "";
            previousTarget = null;
            return;
        }
        
        EntityType targetInfo = target.GetComponent<EntityType>();
        StateManager whatchaDoin = target.GetComponent<StateManager>();

        if (previousTarget != target)
        {
            previousTarget = target;

            uiData.SetActive(true);            
        }

        //Stuff that needs checked every frame
        dataText.text = target.gameObject.name + "\n";
        dataText.text += targetInfo.stats.race.raceName + "\n";
        dataText.text += whatchaDoin.currentState;
    }

    Vector3Int CheckTerrainSpot()
    {
        RaycastHit hit;
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

        Vector3Int found = new Vector3Int();
        int layerMask = 1 << 6;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            found = new Vector3Int(MathFun.Floor(hit.point.x), MathFun.Floor(hit.point.y), MathFun.Floor(hit.point.z));
        }

        return found;
    }

    Transform newTarget()
    {
        RaycastHit hit;
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

        Transform found = null;
        int layerMask = 1 << 7;
        layerMask = ~layerMask;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.transform.GetComponent<EntityType>() != null)
            {
                found = hit.collider.transform;
            }
        }

        return found;
    }

}

[System.Serializable]
public class CursorIcons
{
    public string actionName;
    public Color actionColor;
    public int actionTextureID;
    public bool cancelActions;
    public KeyCode primaryButton;
    public KeyCode alternateButton;
    public TaskList taskAction;

    public enum TaskList
    {
        None,
        Mine,
        Attack,
        Build,
        Clear
    }
}