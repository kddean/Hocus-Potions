using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineManager : MonoBehaviour {
    public bool social, order, nature;
    public Dictionary<string, string[]> dialogue;
    public void Awake() {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    void Start() {
        social = order = nature = false;
        dialogue = new Dictionary<string, string[]>();
        SetupDialogue();
        StartCoroutine(SetState());
    }

    IEnumerator SetState() {
        if (GameObject.FindObjectOfType<NPCController>().npcData["Black_Robed_Traveler"].affinity >= 3) {
            order = true;
        }

        if (GameObject.FindObjectOfType<NPCController>().npcData["Red_Robed_Traveler"].affinity >= 3) {
            social = true;
        }

        if (GameObject.FindObjectOfType<NPCController>().npcData["White_Robed_Traveler"].affinity >= 3) {
            nature = true;
        }
        yield return new WaitForSeconds(5);
        StartCoroutine(SetState());
    }

    void SetupDialogue() {
        dialogue.Add("order", new string[] { "You walk the path of power and control, if you’ve made it this far then I’m sure you know", "What duties the Priestess of the Deep must hold", "For your final task you must brew a concoction", 
             "Combine three ingredients to make it so, to finally join the holy echelon", "First, a symbol of eternal life, ripped from the earth to power your will","Second, the roar of the common lion, slumbering ready for its tasks to fulfill",
             "Finally, a bloom of powers three: poison, sleep, and invisibility." });

        dialogue.Add("social", new string[] { "My dear young witch, you’ve grown strong under my watchful eye", "I think you are ready for something new to try", "Would you like to brew a potion that will bring great joy?",
            "If so listen carefully, as magic this powerful is no toy...", "First you’ll need a drop of liquid sunshine, frozen in the heart of an ancient tree", "Second, the ghostly guardian of the moonless night, holding spirits at bay through the danger of the dark",
            "Finally, find what makes a homecooked meal complete, a healing presence of love and light.", "...", "Ohh ho ho, would you like me to be more specific, my dear?", "Alas, this is your challenge to face." });


        dialogue.Add("nature", new string[] {"Dear little witch, do you wish to learn?", "A potion that shall give chaos a turn?", "You must combine these ingredients three, if you want the earth to be truly free:",
            "First, find a cap of red and white, where the toads sit in the dark of night", " Second, a solid moonbeam rose, caught in the ground and pale as cream",
            "Finally, a bloom of a plant that you know to have the powers of poison, mana, and transform inside.", "Brew together and you will find the power to free the world with a chaotic tide" });
    }
}
