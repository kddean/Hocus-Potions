using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class GardenPlot : MonoBehaviour, IPointerDownHandler {

    // Use this for initialization
    ResourceLoader rl;
    Player player;
    private void Start() {
        rl = GameObject.FindGameObjectWithTag("loader").GetComponent<ResourceLoader>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //Display plants if there's something growing
        Garden.PlotData temp;
        if (rl.garden.plots.TryGetValue(gameObject.name, out temp)) {
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 1; i < 4; i++) {
                sprites[i].sprite = Resources.LoadAll<Sprite>("Plants/" + temp.type)[temp.index];
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            if (rl.activeItem != null && rl.activeItem.item.item is Seed) {
                PlantSeed(rl.activeItem.item.item as Seed, rl.activeItem);
            } else {
                PlantSeed(null, rl.activeItem);
            }
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            if (player.Status.Contains(Player.PlayerStatus.asleep) || player.Status.Contains(Player.PlayerStatus.transformed) || Vector3.Distance(player.transform.position, transform.position) > 3f) {
                return;
            }

            if (rl.activeSpell != null && GameObject.FindObjectOfType<Mana>().CurrentMana >= rl.activeSpell.Cost && !GameObject.FindObjectOfType<Mana>().InUse) {
                rl.garden.SpellCast(this);
            }
        }
    }

    public void PlantSeed(Seed s, InventorySlot slot) {
        if (player.Status.Contains(Player.PlayerStatus.asleep) || player.Status.Contains(Player.PlayerStatus.transformed) || Vector3.Distance(player.transform.position, transform.position) > 3f) { return; }
        rl.garden.Farm(this, s, slot);
    }
}
