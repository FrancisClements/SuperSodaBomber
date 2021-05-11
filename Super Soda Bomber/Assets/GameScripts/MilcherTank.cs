using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//MILCHER TANK
namespace SuperSodaBomber.Enemies{
    public class MilcherTank: BaseEnemy{
        private Milcher outer;

        void Init(Milcher outerMilcher){
            outer = outerMilcher;
        }

        public override void CueAttack()
        {
            throw new System.NotImplementedException();
        }

        protected override void CueFlipEvent(){

        }

        /// <summary>
        /// The tank moves left to right. If it bumps to something, attack.
        /// </summary>
        public class Phase1: IEnemyInner{
            public void CallState(){

            }
        }        
    }
}

