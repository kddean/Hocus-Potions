using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DredgeSpell : MonoBehaviour, IPointerDownHandler {

    ResourceLoader rl;
    Mana mana;
    void Start() {
        rl = GameObject.FindObjectOfType<ResourceLoader>();
        mana = GameObject.FindObjectOfType<Mana>();
    }


    public void OnPointerDown(PointerEventData eventData) {
        Player player = GameObject.FindObjectOfType<Player>();
        if (eventData.button != PointerEventData.InputButton.Right || player.Status.Contains(Player.PlayerStatus.asleep) || player.Status.Contains(Player.PlayerStatus.transformed) || Vector3.Distance(player.transform.position, transform.position) > 3f) {
            return;
        }

        if (rl.activeSpell != null && rl.activeSpell.SpellName.Equals("Dredge") && mana.CurrentMana >= rl.activeSpell.Cost) {
            string key;
            do {
                int i = Random.Range(0, rl.ingredientCount);
                key = rl.ingredients.Keys.ToArray()[i];
            } while (!rl.ingredients[key].imagePath.Contains("Plant"));
            StartCoroutine(SpawnItem(rl.ingredients[key]));
        }

    }

    IEnumerator SpawnItem(Ingredient item) {
        Animator anim = GetComponent<Animator>();
        anim.SetBool("Smash", true);  //Change name
        float time = 1.5f; //change time
        RuntimeAnimatorController ac = anim.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++) {
            if (ac.animationClips[i].name.Contains("smash")) { //change name
                time = ac.animationClips[i].length;
            }
        }
        yield return new WaitForSeconds(time);
        anim.SetBool("Smash", false); // change name
        GetComponent<SpriteRenderer>().enabled = false;
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
