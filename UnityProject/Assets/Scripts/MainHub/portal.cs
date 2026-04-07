using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Portal : MonoBehaviour
{
    public string sceneToLoad;   // Name of the scene
    public float interactRange = 3f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Vector3.Distance(player.position, transform.position) <= interactRange)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                LoadScene();
            }
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
