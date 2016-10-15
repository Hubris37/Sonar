using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/// Source
/// https://upsidedownbird.com/how-to-make-unity-game-look-better-scene-fade/
public class NiceSceneTransition : MonoBehaviour {

    public static NiceSceneTransition instance;

    public float transitionTime = 1.0f;
    public bool fadeIn;
    public bool fadeOut;
    public Image fadeImg;
    
    private Transform playerTrans;
    private float time = 0f;

    // Use this for initialization
    void Awake()
    {
        playerTrans = GameObject.FindWithTag("Player").transform;

        if (instance == null) {
            DontDestroyOnLoad(transform.gameObject);
            instance = this;
            if (fadeIn) {
                fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, 1.0f);
            }
        } else {
            Destroy(transform.gameObject);
        }
    }

    void OnEnable()
    {
        playerTrans = GameObject.FindWithTag("Player").transform;
        
        if (fadeIn)
        {
            StartCoroutine(StartScene());
        }
    }

    public void LoadScene(string level)
    {
        StartCoroutine(EndScene(level));
    }

    IEnumerator StartScene()
    {
        playerTrans = GameObject.FindWithTag("Player").transform;
        MoveInfrontOfPlayer();

        time = 1.0f;
        yield return null;
        while (time >= 0.0f)
        {
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, time);
            time -= Time.deltaTime * (1.0f / transitionTime);
            yield return null;
        }
        fadeImg.gameObject.SetActive(false);
    }

    IEnumerator EndScene(string nextScene)
    {
        playerTrans = GameObject.FindWithTag("Player").transform;
        MoveInfrontOfPlayer();

        fadeImg.gameObject.SetActive(true);
        time = 0.0f;
        yield return null;
        while (time <= 1.0f)
        {
            fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, time);
            time += Time.deltaTime * (1.0f/transitionTime);
            yield return null;
        }
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
        StartCoroutine(StartScene());
    }

    void MoveInfrontOfPlayer()
    {   
        Vector3 f = playerTrans.forward;
        transform.position = playerTrans.position + (new Vector3(f.x, f.y, f.z) * 0.5f);
        transform.rotation = playerTrans.rotation;
    }
}
