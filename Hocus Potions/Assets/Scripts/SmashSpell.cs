﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SmashSpell : MonoBehaviour, IPointerDownHandler {
    ResourceLoader rl;
    Mana mana;
    // Use this for initialization
    Ingredient[][] spawnOptions;
    void Start() {
        rl = GameObject.FindObjectOfType<ResourceLoader>();
        mana = GameObject.FindObjectOfType<Mana>();
        spawnOptions = new Ingredient[4][];
        spawnOptions[0] = new Ingredient[] { rl.ingredients["amethyst"], rl.ingredients["emerald"] }; 
        spawnOptions[1] = new Ingredient[] { rl.ingredients["garnet"], rl.ingredients["jet"] };
        spawnOptions[2] = new Ingredient[] { rl.ingredients["amber"] };
        spawnOptions[3] = new Ingredient[] { rl.ingredients["amber"] };
    }

    private void OnMouseEnter() {
        if (rl.activeSpell != null && rl.activeSpell.SpellName.Equals("Smash")) {
            Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Smash Mouse"), Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnMouseExit() {
        Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
    }
    public void OnPointerDown(PointerEventData eventData) {
        if(eventData.button == PointerEventData.InputButton.Right && rl.activeSpell != null && rl.activeSpell.SpellName.Equals("Smash") && mana.CurrentMana >= rl.activeSpell.Cost) {
            int type;

            if (gameObject.name.Contains("Mountain")) {
                type = 0;
            } else if (gameObject.name.Contains("Forest")) {
                type = 1;
            } else if (gameObject.name.Contains("Pine")) {
                type = 2;
            } else if (gameObject.name.Contains("Oak")) {
                type = 3;
            } else {
                return;
            }
            //Play animation

            GetComponent<SpriteRenderer>().enabled = false;
            BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
            foreach(BoxCollider2D b in colliders) {
                b.enabled = false;
            }

            float chance;

            switch (type) {
                case 0:
                    chance = Random.Range(0, 1.0f);
                    if (chance < 0.5f) {
                        SpawnItem(spawnOptions[0][0]);
                    }else if(chance < 0.8f) {
                        SpawnItem(spawnOptions[0][1]);
                    }
                    break;
                case 1:
                    chance = Random.Range(0, 1.0f);
                    if (chance < 0.4f) {
                        SpawnItem(spawnOptions[1][0]);
                    } else if (chance < 0.6f) {
                        SpawnItem(spawnOptions[1][1]);
                    }
                    break;
                case 2:
                    chance = Random.Range(0, 1.0f);
                    if (chance < 0.5f) {
                        SpawnItem(spawnOptions[2][0]);
                    } 
                    break;
                case 3:
                    chance = Random.Range(0, 1.0f);
                    if (chance < 0.5f) {
                        SpawnItem(spawnOptions[3][0]);
                    }
                    break;
                default:
                    break;
            }
            mana.UpdateMana(rl.activeSpell.Cost);
        }
    }

    void SpawnItem(Ingredient item) {
        GameObject go = new GameObject();
        go.name = item.name;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>(item.imagePath);
        sr.sortingOrder = 10;
        go.transform.position = gameObject.transform.position;
        Vector2 bounds = new Vector2(sr.bounds.size.x, sr.bounds.size.y);
        BoxCollider2D c = go.AddComponent<BoxCollider2D>();
        c.size = bounds;
        c.isTrigger = true;
        sr.sortingLayerName = "InFrontOfPlayer";
        sr.sortingOrder = 10;
        Pickups p = go.AddComponent<Pickups>();
        p.Item = item;
        p.Count = 1;
        p.Data = new GarbageCollecter.DroppedItemData(item, 1, go.transform.position.x, go.transform.position.y, go.transform.position.z, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        GameObject.Find("GarbageCollector").GetComponent<GarbageCollecter>().droppedItems.Add(p.Data);
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn() {
        yield return new WaitForSeconds(GameObject.FindObjectOfType<MoonCycle>().CLOCK_SPEED * 144);
        GetComponent<SpriteRenderer>().enabled = true;
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D b in colliders) {
            b.enabled = true;
        }
    }
}
