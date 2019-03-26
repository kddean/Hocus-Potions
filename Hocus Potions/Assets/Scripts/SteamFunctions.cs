using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamFunctions : MonoBehaviour {
    Callback<GameOverlayActivated_t> m_GameOverlayActivated;
    // Use this for initialization

    private void OnEnable() {
        if (SteamManager.Initialized) {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
        }
    }


    private void OnGameOverlayActivated(GameOverlayActivated_t pCallback) {
        if (pCallback.m_bActive != 0) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }

}
