using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Sleep : MonoBehaviour {
    MoonCycle mc;
    Mana mana;
    GameObject canvas;
    GameObject fadeScreen;
    Player player;
    bool done, sleeping;

    
    void Start () {
        mc = GameObject.Find("Clock").GetComponent<MoonCycle>();
        mana = GameObject.FindObjectOfType<Mana>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        canvas = Resources.FindObjectsOfTypeAll<SleepCanvas>()[0].gameObject;
        canvas.SetActive(false);
        done = false;
        sleeping = false;
    }
	
    private void OnTriggerEnter2D(Collider2D collision) {
        if(mc.Hour >= 20 || mc.Hour < 6) {
            fadeScreen = canvas.GetComponentInChildren<Image>().gameObject;
            canvas.SetActive(true);
            canvas.GetComponentsInChildren<Button>()[0].onClick.RemoveAllListeners();
            canvas.GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners();
            canvas.GetComponentsInChildren<Button>()[0].onClick.AddListener(FallAsleep);
            canvas.GetComponentsInChildren<Button>()[1].onClick.AddListener(DontSleep);
            canvas.GetComponentsInChildren<CanvasGroup>()[1].alpha = 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        canvas.GetComponentsInChildren<CanvasGroup>()[1].alpha = 0;
        canvas.SetActive(false);
    }

    private void Update() {
        if (done) {
            Time.timeScale = 10f;
            done = false;
            sleeping = true;
            switch (mc.Hour) {
                case 20:
                    mana.UpdateMana(-200f);
                    break;
                case 21:
                    mana.UpdateMana(-180f);
                    break;
                case 22:
                    mana.UpdateMana(-160f);             
                    break;
                case 23:
                    mana.UpdateMana(-140f);
                    break;
                default:
                    mana.UpdateMana(-1 * ((6 - mc.Hour) * 20));  
                    break;
            }
        }
        if (sleeping && mc.Hour == 6) {
            sleeping = false;
            Time.timeScale = 1f;
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
