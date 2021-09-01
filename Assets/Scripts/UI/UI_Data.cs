using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Data : MonoBehaviour
{
    public Transform target;
    Transform previousTarget;
    public Camera playerCam;

    public GameObject uiData;
    public TextMeshProUGUI dataText;

    // Start is called before the first frame update
    void Start()
    {
        if(PMovement.Player != null)
        {
            playerCam = PMovement.Player.GetPlayerCam();
        }

        uiData.SetActive(false);
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
            }
        }

        UpdateUITips();
    }

    void UpdateUITips()
    {
        if(target == null)
        {
            if(uiData.activeSelf) uiData.SetActive(false);
            dataText.text = "";
            return;
        }
        
        EntityType targetInfo = target.GetComponent<EntityType>();

        if (previousTarget != target)
        {
            previousTarget = target;

            uiData.SetActive(true);
            dataText.text = target.gameObject.name + "\n";
            dataText.text += targetInfo.stats.race.raceName;
        }

        //Stuff that needs checked every frame
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
