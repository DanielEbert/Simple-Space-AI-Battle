using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;


public class BetterSave : MonoBehaviour
{
    public Transform t;

    public int abc = 1324;

    public int count = 0;


    void Start()
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
            Component[] myComponents = go.GetComponents(typeof(Component));
            //foreach (Component myComp in myComponents)
            //{
            for(int i = 0; i < myComponents.Length; i++) {
                Component myComp = myComponents[i];
                Type myObjectType = myComp.GetType();
                foreach (var thisVar in myComp.GetType().GetMembers())
                {
                    try
                    {
                        //2 Component:  New Sprite        Var Name:  isActiveAndEnabled         Type:  System.Boolean       Value:  True
                        Debug.Log(i + " Component:  " + myComp.name + "        Var Name:  " + thisVar.Name );//+ "         Type:  " + thisVar.PropertyType + "       Value:  " + thisVar.GetValue(myComp,null) + "\n" );
                        /* if (i == 2 && myComp.name == "New Sprite" && thisVar.Name == "isActiveAndEnabled") {
                            print("SETTING NOW " + thisVar.GetValue(myComp,null));
                            //thisVar.SetValue(thisVar, true, null);
                            //myComp.GetType().GetProperty(thisVar.Name).SetValue(myComp, true);
                            myComp.isActiveAndEnabled = true;
                            print("READ NOW " + thisVar.GetValue(myComp,null));
                        }*/
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
                /*foreach (var thisVar in myComp.GetType().GetFields())
                {
                    try
                    {
                        Debug.Log("2: " + i + " Component:  " + myComp.name + "        Var Name:  " + thisVar.Name + "         Type:  " + thisVar.MemberType + "       Value:  " + thisVar.GetValue(myComp) + "\n" );
                        count += 1;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }*/
            }
        }
    }
}
