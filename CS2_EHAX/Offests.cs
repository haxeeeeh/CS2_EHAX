using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2_EHAX
{
    public class Offests
    {
        // offsets.cs
        public static int dwViewAngles = 0x1A23848;
        public static int dwLocalPlayerPawn = 0x181A9C8;
        public static int dwEntityList = 0x19B49B8;

        // client.dll
        public static int m_hPlayerPawn = 0x7DC;
        public static int m_iHealth = 0x324;
        public static int m_vOldOrigin = 0x1274;
        public static int m_iTeamNum = 0x3C3;
        public static int m_vecViewOffset = 0xC50;
        public static int m_lifeState = 0x328;
        public static int m_modelState = 0x170;
        public static int m_pGameSceneNode = 0x308;
        public static int m_entitySpottedState = 0x10F8;
        public static int m_bStopped = 0x220;
        public static int dwViewMatrix = 0x1A16A60;
    }
}
