using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DexCode
{
    public abstract class State 
    {
        protected FinitePlayerLogic Player;
        protected FinitePlayerData PlayerData;

        protected float startTime;

        private string AnimBoolName;

        public State(FinitePlayerLogic player,FinitePlayerData playerData,string animBoolName)
        {
            Player = player;
            PlayerData = playerData;
            AnimBoolName = animBoolName;
        }
        public virtual void Exit()
        {
            Player.Anim.SetBool(AnimBoolName, false);
        }
        public virtual void Enter()
        {
            DoChecks();
            Player.Anim.SetBool(AnimBoolName, true);
            startTime = 0;
        }

        public virtual void LogicUpdate()
        {

        }

        public virtual void PhysicsUpdate()
        {
            DoChecks();
        }

        public virtual void DoChecks()
        {

        }

    }
}
