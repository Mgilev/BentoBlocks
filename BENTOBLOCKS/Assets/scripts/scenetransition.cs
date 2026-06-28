using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scenetransition : MonoBehaviour
{
    [SerializeField] private Animator transitionanim;
    private bool isTransitioning = false;

   
    public void scenechange(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(scenechangeCoroutine(sceneName));
        }
    }

    private IEnumerator scenechangeCoroutine(string sceneName)
    {
        isTransitioning = true;

      
        if (Soundms.Instance != null)
        {
            Soundms.Instance.playsfx(Soundms.Instance.transitionwhoosh);
        }
        else
        {
            Debug.LogWarning("Soundms Instance not found in the scene!");
        }

        if (transitionanim != null)
        {
            transitionanim.SetTrigger("end");
        }

        yield return new WaitForSeconds(1f);

        SceneManager.LoadSceneAsync(sceneName);
    }
}