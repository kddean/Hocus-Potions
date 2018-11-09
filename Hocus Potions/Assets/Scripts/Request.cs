using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request {
    string key;
    float strength, healing, invisibility, mana, none, poison, sleep, speed, transformation;

    public Request(string key, float strength, float healing, float invisibility, float mana, float none, float poison, float sleep, float speed, float transformation) {
        this.key = key;
        this.Strength = strength;
        this.Healing = healing;
        this.Invisibility = invisibility;
        this.Mana = mana;
        this.None = none;
        this.Poison = poison;
        this.Sleep = sleep;
        this.Speed = speed;
        this.Transformation = transformation;
    }

    public string Key {
        get {
            return key;
        }

        set {
            key = value;
        }
    }

    public float Strength {
        get {
            return strength;
        }

        set {
            strength = value;
        }
    }

    public float Healing {
        get {
            return healing;
        }

        set {
            healing = value;
        }
    }

    public float Invisibility {
        get {
            return invisibility;
        }

        set {
            invisibility = value;
        }
    }

    public float Mana {
        get {
            return mana;
        }

        set {
            mana = value;
        }
    }

    public float None {
        get {
            return none;
        }

        set {
            none = value;
        }
    }

    public float Poison {
        get {
            return poison;
        }

        set {
            poison = value;
        }
    }

    public float Sleep {
        get {
            return sleep;
        }

        set {
            sleep = value;
        }
    }

    public float Speed {
        get {
            return speed;
        }

        set {
            speed = value;
        }
    }

    public float Transformation {
        get {
            return transformation;
        }

        set {
            transformation = value;
        }
    }

    public float GetValue(string s) {
        switch (s) {
            case "healing":
                return healing;
            case "invisibility":
                return invisibility;
            case "mana":
                return mana;
            case "none":
                return none;
            case "poison":
                return poison;
            case "sleep":
                return sleep;
            case "speed":
                return speed;
            case "transformation":
                return transformation;
            default:
                return 0;
        }
    }
}

