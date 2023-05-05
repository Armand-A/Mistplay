using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour
{
    public static GameFlow GF;

    public GameObject introHighlight, levelOne, music;

    GameObject m_grid, o_grid, a_grid, EndCard;

    GameObject W, Wselected, O, Oselected, A4, A4selected; 

    TapHandler levelOne_TH, W_TH, O_TH, A4_TH; 

    public int state = 0;

    public bool canClickCTA;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(music);        
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Imposter"){
            m_grid = GameObject.Find("M Grid");
            W = m_grid.transform.Find("W").gameObject;
            W_TH = W.GetComponent<TapHandler>();
            Wselected = m_grid.transform.Find("W - Chosen").gameObject;

            o_grid = GameObject.Find("O Grid");
            O = o_grid.transform.Find("Oh").gameObject;
            O_TH = O.GetComponent<TapHandler>();
            Oselected = o_grid.transform.Find("Oh - Chosen").gameObject;

            a_grid = GameObject.Find("A Grid");
            A4 = a_grid.transform.Find("4").gameObject;
            A4_TH = A4.GetComponent<TapHandler>();
            A4selected = a_grid.transform.Find("4 - Chosen").gameObject;

            o_grid.SetActive(false);
            a_grid.SetActive(false);

            StartCoroutine(popInDelay(3));
        } else if (scene.name == "Ending"){
            EndCard = GameObject.Find("CTA");
            EndCard.SetActive(false);
            StartCoroutine(endingDelay());
        }
    }

    void Start()
    {
        GF = this; 

        levelOne_TH = levelOne.GetComponent<TapHandler>();

        StartCoroutine(beginLevelSelect());
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            Vector2 c = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (canClickCTA){
                Luna.Unity.Playable.InstallFullGame();
            } else if (state == 1 && levelOne_TH.checkTouch(c)){
                state = 2;
                SceneManager.LoadScene("Imposter");
            } else if (state == 3 && W_TH.checkTouch(c)){
                state = 4;
                W.SetActive(false);
                Wselected.SetActive(true);
                Wselected.GetComponent<Animator>().Play("SelectPop", 0, 0);
                StartCoroutine(popOutTrigger(m_grid));
            } else if (state == 5 && O_TH.checkTouch(c)){
                state = 6;
                O.SetActive(false);
                Oselected.SetActive(true);
                Oselected.GetComponent<Animator>().Play("SelectPop", 0, 0);
                StartCoroutine(popOutTrigger(o_grid));
            } else if (state == 7 && A4_TH.checkTouch(c)){
                state = 8;
                A4.SetActive(false);
                A4selected.SetActive(true);
                A4selected.GetComponent<Animator>().Play("SelectPop", 0, 0);
                StartCoroutine(popOutTrigger(a_grid));
            }
        }
    }
    

    IEnumerator beginLevelSelect(){
        yield return new WaitForSeconds(0.5f);
        introHighlight.SetActive(true);
        state = 1; 
    }

    IEnumerator popInDelay(int val){
        yield return new WaitForSeconds(0.25f);
        state = val;
    }

    IEnumerator endingDelay(){
        yield return new WaitForSeconds(1.75f);
        canClickCTA = true;
        EndCard.SetActive(true);
    }

    IEnumerator popOutTrigger(GameObject tileGrid){
        yield return new WaitForSeconds(0.7f);
        for (int i = 0; i < tileGrid.transform.childCount; i++){
            if (tileGrid.transform.GetChild(i).gameObject.activeSelf){
                tileGrid.transform.GetChild(i).GetComponent<Animator>().Play("PopOut", 0, 0);
            }
        }
        yield return new WaitForSeconds(0.5f);
        if (state == 4){
            m_grid.SetActive(false);
            o_grid.SetActive(true);
            StartCoroutine(popInDelay(5));
        } else if (state == 6){
            o_grid.SetActive(false);
            a_grid.SetActive(true);
            StartCoroutine(popInDelay(7));
        } else if (state == 8){
            SceneManager.LoadScene("Ending");
        }
    }
}
