using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
    string characterName;
    Dictionary<string,List<string>> dialogue;

    public string CharacterName {
        get {
            return characterName;
        }

        set {
            characterName = value;
        }
    }

    public Dictionary<string, List<string>> Dialogue {
        get {
            return dialogue;
        }

        set {
            dialogue = value;
        }
    }
}
