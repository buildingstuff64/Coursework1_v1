using Assets.Scripts.Game_Management;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class StatsPage : MonoBehaviour
    {
        [SerializeField] private TMP_Text exploreation;
        [SerializeField] private TMP_Text killcount;
        [SerializeField] private TMP_Text time;
        [SerializeField] private TMP_Text WinLose;

        [SerializeField] private Button retryButton;

        public void create(stats stats)
        {
            WinLose.text = string.Format(stats.win_lose ? "You Win" : "You Lose");
            exploreation.text = string.Format("Exploration - {0}%", stats.exploreation_percent.ToString("F2"));
            killcount.text = string.Format("Enemies Killed - {0}", stats.killcount.ToString());
            TimeSpan t = TimeSpan.FromSeconds(stats.time);
            time.text = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                t.Hours,
                t.Minutes,
                t.Seconds);

            retryButton.onClick.AddListener(GameManager.instance.onRetry);

        }


    }

    public class stats
    {
        public float exploreation_percent;
        public int killcount;
        public float time;
        public bool win_lose;

        public stats(List<Island> islands, int kills, bool win_lose)
        {
            int explored = 0;
            foreach (Island island in islands)
            {
                if (island.isTriggered) explored++;
            }
            Debug.Log(explored);
            Debug.Log(islands.Count);
            exploreation_percent = ((float) explored / (float) islands.Count) * 100f;
            Debug.Log(exploreation_percent);
            killcount = kills;
            time = Time.time;
            this.win_lose = win_lose;
        }
    }
}