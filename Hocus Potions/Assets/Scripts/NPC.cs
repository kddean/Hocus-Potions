using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
    string characterName;
    Dictionary<string,List<string>> dialogue;

    //Status 
    //Queue<Action> schedule;
    //Relationship[] relations;


    /* TODO: figure out how this will work later
    public class Relationship {
        NPC otherNPC;
        enum relationshipType { };
        int level;
        //probably needs more fields; depends on depth of the system
    }
    */


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
