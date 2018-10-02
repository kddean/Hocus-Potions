using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC {
    string name;
    Sprite portrait, character;
    Queue<Action> schedule;
    Relationship[] relations;

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
}
