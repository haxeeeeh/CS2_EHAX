using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CS2_EHAX
{
    public class Entity
    {
        public IntPtr pawnAddress {  get; set; }
        public IntPtr controllerAddress { get; set; }
        public Vector3 origin { get; set; }
        public Vector3 position { get; set; }
        public Vector3 view { get; set; }
        public Vector3 head { get; set; }
        public Vector2 head2D { get; set; }
        public Vector2 position2D { get; set; }
        public Vector2 viewPosition2D { get; set; }
        public int health { get; set; }
        public int team { get; set; }
        public uint lifeState {  get; set; }
        public float distance {  get; set; } // from localplayer

        public float pixelDistance { get; set; }
    }
}
