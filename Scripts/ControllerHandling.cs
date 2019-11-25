using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/************************************************************************************************************
Class for handling when the controller is plugged in
************************************************************************************************************/
public class ControllerHandling : MonoBehaviour {
    // Is the controller in control
    [HideInInspector]
    public bool controllerInControl = false;
    // A list of the controllers plugged in last frame
    List<string> controllers = new List<string>();

    [HideInInspector]
    // The ui manager
    public UIManager uiManager = null;
    [HideInInspector]
    // The main menu
    public MainMenu mainMenu = null;

    void Start() {
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);

        detectController();
        Time.timeScale = 1;
    }

    void Update() {
        // Find the ui manager if it has not been found and the main menu has not been found
        if (uiManager == null && mainMenu == null) {
            GameObject temp = GameObject.FindGameObjectWithTag("UICanvas");
            if (temp != null) {
                uiManager = temp.GetComponent<UIManager>();
            }
        }
        // Find the main menu if it has not been found and the ui manager has not been found
        if (mainMenu == null && uiManager == null) {
            GameObject temp = GameObject.FindGameObjectWithTag("UICanvas");
            if (temp != null) {
                mainMenu = temp.GetComponent<MainMenu>();
            }
        }

        detectController();
    }

    // Detects whether a controller has been plugged in this frame or not
    // Determines if the controller should be in control or the mouse and keyboard
    void detectController() {
        // Get the controllers currently plugged in
        string[] temp = Input.GetJoystickNames();

        bool controllerPluggedIn = false;
        if (temp.Length != 0) {
            if (string.IsNullOrEmpty(temp[0])) {
                // Keyboard
                controllerPluggedIn = false;
            }
            else {
                // Controller
                controllerPluggedIn = true;
            }
        }

        // Determines if a controller was just plugged in and is not controlling
        if (controllerInControl == false && controllerPluggedIn == true || controllerInControl == true && EventSystem.current.currentSelectedGameObject == null) {
            controllerInControl = true;
            if (uiManager != null) {
                uiManager.selectButton(this);
            }
            else if (mainMenu != null) {
                mainMenu.selectButton(this);
            }
        }
        // Determines if a controller was just unplugged and the controller needs to lose control
        else if (controllerInControl == true && controllerPluggedIn == false) {
            controllerInControl = false;
            if (uiManager != null) {
                deselectButton();
            }
            else if (mainMenu != null) {
                deselectButton();
            }
        }

        // Selects the ui's button if neccesary
        if (controllerInControl == true && EventSystem.current.currentSelectedGameObject == null) {
            if (uiManager != null) {
                uiManager.selectButton(this);
            }
            else if (mainMenu != null) {
                mainMenu.selectButton(this);
            }
        }
    }

    // Deselects buttons, gives the mouse control
    public void deselectButton() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        EventSystem.current.SetSelectedGameObject(null);
    }

    // Selects a button, gives the controller control
    public void selectButton(Button button) {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (button != null) {
            button.Select();
        }
        else {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
