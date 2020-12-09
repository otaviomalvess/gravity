using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    #region Serialized
        [SerializeField] GameObject  btResume;
        [SerializeField] GameObject  btQuit;
        [SerializeField] GameControl gc;
        [SerializeField] Sprite      btSel;
        [SerializeField] Sprite      btUns;
        [SerializeField] Text        txtResume;
        [SerializeField] Text        txtQuit;
    #endregion

    #region Vars
        bool    resumeSelected = true;
        Image   imgResume;
        Image   imgQuit;
    #endregion
    
    #region Public
        public bool IsPaused = false;
    #endregion

    void Start()
    {
        btResume.SetActive(true);
        btQuit.SetActive(true);

        imgResume = btResume.GetComponent<Image>();
        imgQuit   = btQuit.GetComponent<Image>();
    }

    void Update()
    {
        if (!IsPaused) return;

        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow)) {
            resumeSelected = !resumeSelected;
            ChangeButton();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return)) {
            
            if (resumeSelected) Resume();
            else                Quit(); 
        }
    }

    void ChangeButton() {
        if (resumeSelected) {

            imgResume.sprite = btSel;
            imgQuit.sprite   = btUns;
            txtResume.color  = Color.black;
            txtQuit.color    = Color.white;

        } else {

            imgResume.sprite = btUns;
            imgQuit.sprite   = btSel;
            txtResume.color  = Color.white;
            txtQuit.color    = Color.black;
        }
    }

    public void Resume() {
        gc.Pause();
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
}
