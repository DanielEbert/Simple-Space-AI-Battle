using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour {

    public Transform followObject;
    Ship shipScript;

    public Text nameText;
    public Text healthText;
    public Image healthImg;
    public Image energyImg;

    private void Start()
    {
        transform.SetParent(Manager.ShipUIHeader.transform);

        shipScript = followObject.GetComponent<Ship>();

        if(followObject.tag == "Ally")
        {
            nameText.color = Color.blue;
        }
        else if(followObject.tag == "Enemy")
        {
            nameText.color = Color.red;
        }
    }

    void Update()
    {
        transform.position = followObject.position;

        healthText.text = "Health: "+shipScript.health;

        healthImg.fillAmount = shipScript.health / shipScript.maxHealth / 4f;
    }
}
