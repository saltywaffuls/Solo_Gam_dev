using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] List<SceneDetails> connectedScenes;
    public bool IsLoaded { get; private set; }

    List<SavableEntity> savableEntities;

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
            var prevSene = GameController.Instance.prevScene;
            if (GameController.Instance.prevScene != null)
            {
                var previoslyloadedScenes = GameController.Instance.prevScene.connectedScenes;
                foreach(var scene in previoslyloadedScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this)
                        scene.UnloadScene();
                }

                if (!connectedScenes.Contains(prevSene))
                    prevSene.UnloadScene();
            }
        }
    }

    // load the scene 1 time
    public void loadScene()
    {
        if(!IsLoaded)
        {
            var operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;

            operation.completed += (AsyncOperation op) =>
            {
                savableEntities = GetSavableEntitiesInScene();
                SavingSystem.i.RestoreEntityStates(savableEntities);
            };
        }
    }

    //unloads scene
    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SavingSystem.i.CaptureEntityStates(savableEntities);

            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }

    List<SavableEntity> GetSavableEntitiesInScene()
    {
        var currScene = SceneManager.GetSceneByName(gameObject.name);
        var savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currScene).ToList();
        return savableEntities;
    }
}
