using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public static List<GameObject> AllyShips = new List<GameObject>();
    public static List<GameObject> EnemyShips = new List<GameObject>();

    public static GameObject ShipUIPrefab;
    public GameObject _ShipUIPrefab;

    public static GameObject ShipUIHeader;
    public GameObject _ShipUIHeader;

    void Awake()
    {
        ShipUIPrefab = _ShipUIPrefab;
        ShipUIHeader = _ShipUIHeader;
    }
	
	void Update() {
		if(Input.GetKeyDown(KeyCode.Escape) == true)
		 {
			Application.Quit();
		 }
	}
}
