using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request {
    string key;
    string item;

    public Request(string key, string item) {
        this.Key = key;
        this.Item = item;
    }

    public string Key {
        get {
            return key;
        }

        set {
            key = value;
        }
    }

    public string Item {
        get {
            return item;
        }

        set {
            item = value;
        }
    }
}
