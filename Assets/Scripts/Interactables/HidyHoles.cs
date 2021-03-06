using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidyHoles : MonoBehaviour
{

    [SerializeField] private bool canDo = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        var player = collider.attachedRigidbody.GetComponent<Player>();
        if (player)
        {
            canDo = true;
            player.UpdateHide(canDo);
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        canDo = false;
        var player = collider.attachedRigidbody.GetComponent<Player>();
        if (player)
        {
            player.UpdateHide(canDo);
        }
    }

}
