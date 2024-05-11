using Game;
using UnityEngine;

namespace Land
{
    internal class LandTile : MonoBehaviour
    {
        [SerializeField]
        private LandType landType;

        public LandType LandType => landType;

        public LoadingPriority Priority => LoadingPriority.Low;
    }
}
