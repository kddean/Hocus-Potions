using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DredgeSpell : MonoBehaviour, IPointerDownHandler {

    ResourceLoader rl;
    Mana mana;
    List<string> spawnableItems;
    void Start() {
        rl = GameObject.FindObjectOfType<ResourceLoader>();
        mana = GameObject.FindObjectOfType<Mana>();

        spawnableItems = new List<string>();
        spawnableItems.Add("selenite");
        spawnableItems.Add("lapis_lazuli");
    }


    private void OnMouseEnter() {
        if (rl.activeSpell != null && rl.activeSpell.SpellName.Equals("Dredge")){
            Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Water Mouse"), Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnMouseExit() {
        Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerDown(PointerEventData eventData) {
        Player player = GameObject.FindObjectOfType<Player>();
        if (eventData.button != PointerEventData.InputButton.Right || player.Status.Contains(Player.PlayerStatus.asleep) || player.Status.Contains(Player.PlayerStatus.transformed) || Vector3.Distance(player.transform.position, transform.position) > 3f) {
            return;
        }

        if (rl.activeSpell != null && rl.activeSpell.SpellName.Equals("Dredge") && mana.CurrentMana >= rl.activeSpell.Cost) {
            string key;
            /*do {
                int i = Random.Range(0, rl.ingredients.Count);
                key = rl.ingredients.Keys.ToArray()[i];
            } while (!rl.ingredients[key].imagePath.Contains("Plant"));*/

            float i = Random.Range(0f, 1f);
            Debug.Log(i);
            if(i > 0.6)
            {
                key = rl.ingredients["algae"].name;
            }
            else if( i < 0.6 && i > 0.4)
            {
                key = rl.ingredients["snail"].name;
            }
            else if (i < 0.4 && i > 0.2f)
            {
                key = rl.ingredients["lapis_lazuli"].name;               
            }
            else
            {
                key = rl.ingredients["selenite"].name;
            }

            BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D b in colliders) {
                b.enabled = false;
            }

            Cursor.SetCursor(Resources.Load<Texture2D>("Cursors/Default Mouse"), Vector2.zero, CursorMode.Auto);
            mana.UpdateMana(rl.activeSpell.Cost);
            StartCoroutine(SpawnItem(rl.ingredients[key]));
        }
    }

    IEnumerator SpawnItem(Ingredient item) {
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
        go.transform.localScale = new Vector3(0.4f, 0.4f, 1);

        Vector3 target = GameObject.FindGameObjectWithTag("Player").transform.position;
        target.y -= 1;
        while(go.transform.position != target) {
            go.transform.position = Vector3.MoveTowards(go.transform.position, target, Time.deltaTime * 10);
            yield return new WaitForEndOfFrame();
        }

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
