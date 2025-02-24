using Assets.Scripts.Game_Management;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Enemy.Sub_Enemies
{
    public class FinalBoss : MachineGunner
    {
        protected override void onDeath()
        {
            base.onDeath();
            GameManager.instance.onWin();
        }

        public override void Start()
        {
            base.Start();
            healthBar.GetComponent<RectTransform>().localPosition = Vector3.up * 3;
        }

    }
}