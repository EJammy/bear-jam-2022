using System;
using UnityEngine;
using UnityEngine.UIElements;


public class UIManager : MonoBehaviour
{
    private void onEnable() {
        var VE_Root = GetComponent<UIDocument>().rootVisualElement;

        //How to get elements:
        tile1 = VE_Root.Q<GroupBox>("Tile1"); //Name in quotes is found in the UI Builder

        //Different events:
        somebutton.RegisterCallback<ClickEvent>(ev => FunctionCall());

    }
}
