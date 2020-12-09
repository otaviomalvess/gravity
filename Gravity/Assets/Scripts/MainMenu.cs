using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject btStart;
    [SerializeField] GameObject btExit;
    [SerializeField] GameObject player;
    [SerializeField] Sprite     btSel;
    [SerializeField] Sprite     btUns;
    [SerializeField] Text       txtStart;
    [SerializeField] Text       txtExit;

    bool     startSelected   = true;
    Vector3  rot             = new Vector3(0f, 0f, .2f);
    Image    imgStart;
    Image    imgExit;
    Animator anim;

    void Start()
    {
        btStart.SetActive(true);
        btExit.SetActive(true);

        imgStart = btStart.GetComponent<Image>();
        imgExit  =  btExit.GetComponent<Image>();
        anim     = GetComponent<Animator>();
    }

    void Update()
    {
        player.transform.Rotate(rot);

        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow)) {
            startSelected = !startSelected;
            ChangeButton();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return)) {
            
            if (startSelected)  StartGame();
            else                Exit(); 
        }
    }

    void ChangeButton()
    {
        if (startSelected)
        {
            imgStart.sprite = btSel;
            imgExit.sprite  = btUns;
            txtStart.color  = Color.black;
            txtExit.color   = Color.white;
        } 
        else {
            imgStart.sprite = btUns;
            imgExit.sprite  = btSel;
            txtStart.color  = Color.white;
            txtExit.color   = Color.black;
        }
    }

    public void StartGame()
    {
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        anim.SetTrigger("fade");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
