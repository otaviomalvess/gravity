using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    [SerializeField] RectTransform pl;

    Vector3 rot = new Vector3(0f, 0f, 1.2f);

    void Update()
    {
        if (Input.anyKey) SceneManager.LoadScene(0);
    }

    void FixedUpdate()
    {
        pl.Rotate(rot);
    }
}
