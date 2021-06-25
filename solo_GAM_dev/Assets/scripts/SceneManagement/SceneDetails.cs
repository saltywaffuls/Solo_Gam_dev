using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    public bool IsLoaded { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Debug.Log($"enterd {gameObject.name}");

            loadScene();
            GameController.Instance.SetCurrentScene(this);

            //load all connected scenes
            foreach(var scene in connectedScenes)
            {
                scene.loadScene();
            }

            // unload scenes that r not connected
            if(GameController.Instance.prevScene != null)
            {
                var previoslyloadedScenes = GameController.Instance.prevScene.connectedScenes;
                foreach(var scene in previoslyloadedScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this)
                        scene.UnloadScene();
                }
            }
        }
    }

    // load the scene 1 time
    public void loadScene()
    {
        if(!IsLoaded)
            {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
        }
    }

    //unloads scene
    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }
}
