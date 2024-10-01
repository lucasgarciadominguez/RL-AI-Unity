using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class restart : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "BulletMortar")
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //if (other.GetComponent<DashToGoalAgent>())
            //{
            //    other.GetComponent<DashToGoalAgent>().RestartEpisode();
            //}
        }
    }
    public void RestartGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
