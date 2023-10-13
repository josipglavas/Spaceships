using Unity.Netcode;

using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
    [SerializeField]
    private Animator m_menuAnimator;

    [SerializeField]
    private CharacterDataSO[] m_characterDatas;

    [SerializeField]
    private AudioClip m_confirmClip;

    private bool m_pressAnyKeyActive = true;
    private const string k_enterMenuTriggerAnim = "enter_menu";

    [SerializeField]
    private SceneName nextScene = SceneName.CharacterSelection;

    private IEnumerator Start() {
        ClearAllCharacterData();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Wait for the network Scene Manager to start
        yield return new WaitUntil(() => NetworkManager.Singleton.SceneManager != null);

        // Set the events on the loading manager
        // Doing this because every time the network session ends the loading manager stops
        // detecting the events
        LoadingSceneManager.Instance.Init();
    }

    private void Update() {
        if (m_pressAnyKeyActive) {
            if (Input.anyKey) {
                TriggerMainMenuTransitionAnimation();

                m_pressAnyKeyActive = false;
            }
        }
    }

    public void OnClickHost() {
        NetworkManager.Singleton.StartHost();
        AudioManager.Instance.PlaySoundEffect(m_confirmClip);
        LoadingSceneManager.Instance.LoadScene(nextScene);
    }

    public void OnClickJoin() {
        AudioManager.Instance.PlaySoundEffect(m_confirmClip);
        StartCoroutine(Join());
    }

    public void OnClickQuit() {
        AudioManager.Instance.PlaySoundEffect(m_confirmClip);
        Application.Quit();
    }

    private void ClearAllCharacterData() {
        // Clean the all the data of the characters so we can start with a clean slate
        foreach (CharacterDataSO data in m_characterDatas) {
            data.EmptyData();
        }
    }

    private void TriggerMainMenuTransitionAnimation() {
        m_menuAnimator.SetTrigger(k_enterMenuTriggerAnim);
        AudioManager.Instance.PlaySoundEffect(m_confirmClip);
    }

    // We use a coroutine because the server is the one who makes the load
    // we need to make a fade first before calling the start client
    private IEnumerator Join() {
        LoadingFadeEffect.Instance.FadeAll();

        yield return new WaitUntil(() => LoadingFadeEffect.s_canLoad);

        NetworkManager.Singleton.StartClient();
    }
}