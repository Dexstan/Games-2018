using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DexCode
{
    [CreateAssetMenu(fileName ="newPlayerData",menuName = "Data/Player Data/Base Data")]
    public class FinitePlayerData : ScriptableObject
    {
        [Header("Movement Parametars")]
        public float airSpeed;
        public float walkSpeed;
        public float runSpeed;
        public float duckSpeed;
        public float jumpForce;
        public float slideSpeed;
        [Space][Header("Gravity Modifiers")]
        public float fallMultiplier = 8f;
        public float risingGravity = 6f;
        public float maxJumpVelocty = 10;
    }
}
