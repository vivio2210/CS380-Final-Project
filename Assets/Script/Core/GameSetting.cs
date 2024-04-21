using System;
using System.Collections.Generic;

namespace Script
{
    public static class GameSetting
    {
        public enum SceneEnum
        {
            MainMenu = 0,
            Game = 1
        }
        
        public enum PlayerMode
        {
            Manual = 0,
            Auto = 1
        }
        
        public enum EnemyMode
        {
            Independent = 0,
            Cooperative = 1
        }
        
        public enum EnemyCaptureMode
        {
            Touch = 0,
            Surround = 1
        }

        public enum EnemyVisionMode
        {
            Always = 0,
            LineOfSight = 1,
            LOS_Propagation = 2
        }
    }
}