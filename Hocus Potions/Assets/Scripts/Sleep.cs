using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Sleep : MonoBehaviour {
    MoonCycle mc;
    Mana mana;
    public GameObject canvas;
    public GameObject fadeScreen;
    Player player;
    bool done, sleeping;
    float temp;

    
    void Start () {
        mc = GameObject.Find("Clock").GetComponent<MoonCycle>();
        mana = GameObject.FindObjectOfType<Mana>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        temp = mc.CLOCK_SPEED;
        canvas.SetActive(false);
        done = false;
        sleeping = false;
	}
	
    private void OnTriggerEnter2D(Collider2D collision) {
        if(mc.Hour >= 20 || mc.Hour < 6) {
            canvas.SetActive(true);
            canvas.GetComponentsInChildren<CanvasGroup>()[1].alpha = 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        canvas.GetComponentsInChildren<CanvasGroup>()[1].alpha = 0;
        canvas.SetActive(false);
    }

    private void Update() {
        if (done) {
            Time.timeScale = Time.timeScale / (0.1f / temp);
            done = false;
            sleeping = true;
            switch (mc.Hour) {
                case 22:
                    GameObject.FindObjectOfType<Mana>().CurrentMana = Mathf.Clamp(GameObject.FindObjectOfType<Mana>().CurrentMana + 40 + ((6 - mc.Hour) * 20), 0, GameObject.FindObjectOfType<Mana>().MaxMana);
                    break;
                case 23:
                    GameObject.FindObjectOfType<Mana>().CurrentMana = Mathf.Clamp(GameObject.FindObjectOfType<Mana>().CurrentMana + 20 + ((6 - mc.Hour) * 20), 0, GameObject.FindObjectOfType<Mana>().MaxMana);
                    break;
                default:
                    GameObject.FindObjectOfType<Mana>().CurrentMana = Mathf.Clamp(GameObject.FindObjectOfType<Mana>().CurrentMana + ((6 - mc.Hour) * 20), 0, GameObject.FindObjectOfType<Mana>().MaxMana);
                    break;
            }
            GameObject.FindObjectOfType<Mana>().UpdateMana();
        }
        if (sleeping && mc.Hour == 6) {
            sleeping = false;
            Time.timeScale = Time.timeScale * (0.1f / temp);
            StartCoroutine(FadeScreen(-1));
            canvas.GetComponentsInChildren<CanvasGroup>()[0].blocksRaycasts = false;
        }
    }

    public void FallAsleep() {
        canvas.GetComponentsInChildren<CanvasGroup>()[1].alpha = 0;
        canvas.GetComponentsInChildren<CanvasGroup>()[0].blocksRaycasts = true;
        player.Speed = 0;
        player.Status.Add(Player.PlayerStatus.asleep);
        StartCoroutine(FadeScreen(1));
        player.GetComponentInChildren<Animator>().SetBool("Sleep", true);
    }
     
    public void DontSleep() {
        canvas.SetActive(false);
    }

    IEnumerator FadeScreen(int i) {
        CanvasGroup cg = fadeScreen.GetComponent<CanvasGroup>();
        cg.alpha += i * (Time.deltaTime / 2);
        while (cg.alpha > 0 && cg.alpha < 1) {
            cg.alpha += i * (Time.deltaTime / 2);
            yield return null;
        }
        if (cg.alpha == 1) {
            done = true;
        }

        if(cg.alpha == 0) {
            canvas.SetActive(false);
            player.Status.Remove(Player.PlayerStatus.asleep);
            player.Speed = player.defaultSpeed;
            player.GetComponentInChildren<Animator>().SetBool("Sleep", false);
        }
    }
}
