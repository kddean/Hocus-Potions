using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
    private string characterName;
    private Sprite character;
    private Sprite portrait;

    //Queue<Action> schedule;
    //Relationship[] relations;

    public class Action {
        int hour;
        int minute;
        //Finish this when we figure out how to handle it;
    }

    public class Relationship {
        NPC otherNPC;
        enum relationshipType { };
        int level;
        //probably needs more fields; depends on depth of the system
    }

    public string CharacterName {
        get {
            return characterName;
        }

        set {
            characterName = value;
        }
    }

    public Sprite Portrait {
        get {
            return portrait;
        }

        set {
            portrait = value;
        }
    }

    public Sprite Character {
        get {
            return character;
        }

        set {
            character = value;
        }
    }
}
