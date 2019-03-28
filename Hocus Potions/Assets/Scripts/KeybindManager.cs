using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeybindManager : MonoBehaviour {
    public string key;
    InputManager im;
	// Use this for initialization
	void Start () {
        im = GameObject.FindObjectOfType<InputManager>();
	}
	
    public void SetKeybind() {
        StopAllCoroutines();
        StartCoroutine(WaitForKey());
    }

    IEnumerator WaitForKey() {
        while (!Input.anyKeyDown || Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse2)) {
            yield return null;
        }
        string oldText = GetComponentInChildren<Text>().text;
        GetComponentInChildren<Text>().text = "";
        KeyCode newKey = KeyCode.RightCommand;
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKeyDown(kcode)) {
                newKey = kcode;
                break;
            }
        }

        if (im.keybinds.Contains(newKey)) {
            GetComponentInChildren<Text>().text = oldText;
        } else {
           
            GetComponentInChildren<Text>().text = GetString(newKey);
            switch (key) {
                case "invToggle":
                    im.inventoryKey = newKey;
                    im.keybinds.RemoveAt(0);
                    im.keybinds.Insert(0, newKey);
                    break;
                case "inv1":
                    im.inventory1 = newKey;
                    im.keybinds.RemoveAt(1);
                    im.keybinds.Insert(1, newKey);
                    break;
                case "inv2":
                    im.inventory2 = newKey;
                    im.keybinds.RemoveAt(2);
                    im.keybinds.Insert(2, newKey);
                    break;
                case "inv3":
                    im.inventory3 = newKey;
                    im.keybinds.RemoveAt(3);
                    im.keybinds.Insert(3, newKey);
                    break;
                case "inv4":
                    im.inventory4 = newKey;
                    im.keybinds.RemoveAt(4);
                    im.keybinds.Insert(4, newKey);
                    break;
                case "inv5":
                    im.inventory5 = newKey;
                    im.keybinds.RemoveAt(5);
                    im.keybinds.Insert(5, newKey);
                    break;
                case "inv6":
                    im.inventory6 = newKey;
                    im.keybinds.RemoveAt(6);
                    im.keybinds.Insert(6, newKey);
                    break;
                case "inv7":
                    im.inventory7 = newKey;
                    im.keybinds.RemoveAt(7);
                    im.keybinds.Insert(7, newKey);
                    break;
                case "inv8":
                    im.inventory8 = newKey;
                    im.keybinds.RemoveAt(8);
                    im.keybinds.Insert(8, newKey);
                    break;
                case "inv9":
                    im.inventory9 = newKey;
                    im.keybinds.RemoveAt(9);
                    im.keybinds.Insert(9, newKey);
                    break;
                case "inv10":
                    im.inventory10 = newKey;
                    im.keybinds.RemoveAt(10);
                    im.keybinds.Insert(10, newKey);
                    break;
                case "spellToggle":
                    im.spellMenuKey = newKey;
                    im.keybinds.RemoveAt(11);
                    im.keybinds.Insert(11, newKey);
                    break;
                case "wildGrowth":
                    im.spellKey1 = newKey;
                    im.keybinds.RemoveAt(12);
                    im.keybinds.Insert(12, newKey);
                    break;
                case "Ignite":
                    im.spellKey2 = newKey;
                    im.keybinds.RemoveAt(13);
                    im.keybinds.Insert(13, newKey);
                    break;
                case "Smash":
                    im.spellKey3 = newKey;
                    im.keybinds.RemoveAt(14);
                    im.keybinds.Insert(14, newKey);
                    break;
                case "Dredge":
                    im.spellKey4 = newKey;
                    im.keybinds.RemoveAt(15);
                    im.keybinds.Insert(15, newKey);
                    break;
                case "mainMenu":
                    im.mainMenuKey = newKey;
                    im.keybinds.RemoveAt(16);
                    im.keybinds.Insert(16, newKey);
                    break;
                case "pause":
                    im.pauseKey = newKey;
                    im.keybinds.RemoveAt(17);
                    im.keybinds.Insert(17, newKey);
                    break;
                case "walkF":
                    im.walkForwardKey = newKey;
                    im.keybinds.RemoveAt(18);
                    im.keybinds.Insert(18, newKey);
                    break;
                case "walkB":
                    im.walkBackwardKey = newKey;
                    im.keybinds.RemoveAt(19);
                    im.keybinds.Insert(19, newKey);
                    break;
                case "walkL":
                    im.walkLeftKey = newKey;
                    im.keybinds.RemoveAt(20);
                    im.keybinds.Insert(20, newKey);
                    break;
                case "walkR":
                    im.walkRightKey = newKey;
                    im.keybinds.RemoveAt(21);
                    im.keybinds.Insert(21, newKey);
                    break;
                default:
                    break;
            }
        }
    }

    string GetString(KeyCode key) {
        switch (key) {
            case KeyCode.LeftAlt:
                return "LAlt";
            case KeyCode.RightAlt:
                return "RAlt";
            case KeyCode.LeftControl:
                return "LCtrl";
            case KeyCode.RightControl:
                return "RCtrl";
            case KeyCode.LeftShift:
                return "LShift";
            case KeyCode.RightShift:
                return "RShift";
            case KeyCode.LeftWindows:
                return "LWinKey";
            case KeyCode.RightWindows:
                return "RWinKey";
            case KeyCode.LeftCommand:
                return "LCmd";
            case KeyCode.RightCommand:
                return "RCmd";
            case KeyCode.CapsLock:
                return "Caps";
            case KeyCode.Return:
                return "Enter";
            case KeyCode.Backspace:
                return "Back";
            case KeyCode.Slash:
                return "/";
            case KeyCode.Period:
                return ".";
            case KeyCode.Comma:
                return ",";
            case KeyCode.Quote:
                return "\"";
            case KeyCode.Semicolon:
                return ";";
            case KeyCode.LeftBracket:
                return "[";
            case KeyCode.RightBracket:
                return "]";
            case KeyCode.Backslash:
                return "\\";
            case KeyCode.Equals:
                return "=";
            case KeyCode.Minus:
                return "-";
            case KeyCode.BackQuote:
                return "`";
            case KeyCode.Escape:
                return "Esc";
            case KeyCode.PageDown:
                return "PgDown";
            case KeyCode.PageUp:
                return "PgUp";
            case KeyCode.Numlock:
                return "Num";
            case KeyCode.Keypad0:
                return "Num0";
            case KeyCode.Keypad1:
                return "Num1";
            case KeyCode.Keypad2:
                return "Num2";
            case KeyCode.Keypad3:
                return "Num3";
            case KeyCode.Keypad4:
                return "Num4";
            case KeyCode.Keypad5:
                return "Num5";
            case KeyCode.Keypad6:
                return "Num6";
            case KeyCode.Keypad7:
                return "Num7";
            case KeyCode.Keypad8:
                return "Num8";
            case KeyCode.Keypad9:
                return "Num9";
            case KeyCode.KeypadDivide:
                return "/";
            case KeyCode.KeypadEnter:
                return "NumEnt";
            case KeyCode.KeypadMinus:
                return "Num -";
            case KeyCode.KeypadPeriod:
                return "Num .";
            case KeyCode.KeypadPlus:
                return "Num +";
            case KeyCode.KeypadMultiply:
                return "Num *";
            case KeyCode.LeftArrow:
                return "Left";
            case KeyCode.RightArrow:
                return "Right";
            case KeyCode.UpArrow:
                return "Up";
            case KeyCode.DownArrow:
                return "Down";
            case KeyCode.ScrollLock:
                return "ScrLk";
            case KeyCode.SysReq:
                return "Print";
            default:
                return key.ToString();
        }
    }
}
