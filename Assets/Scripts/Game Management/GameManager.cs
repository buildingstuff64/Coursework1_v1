using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Game_Management
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager instance;


        private void Awake()
        {
            instance = this;
        }
        public void onWin()
        {
            UIManager.Instance.createStatpage(true);
        }

        public void onDeath()
        {

            UIManager.Instance.createStatpage(false);
        }

        public void onRetry()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

    }
}