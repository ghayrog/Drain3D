using UnityEngine;

namespace Game
{
    public class GlobalBlinkingMaterial : GameModuleInstaller,
        IGameUpdateListener
    {
        private const float MAX_ALPHA = 0.5f;
        private const float MIN_ALPHA = 0.1f;
        [SerializeField]
        private Material _blinkingMaterial;

        [SerializeField]
        private float _blinkTime;

        private float _intensityMultiplier = 1;

        public LoadingPriority Priority => LoadingPriority.Low;

        public void OnUpdate(float deltaTime)
        {
            Color color = _blinkingMaterial.color;
            float alpha = color.a;

            alpha += deltaTime / _blinkTime * _intensityMultiplier;

            if (alpha <= MIN_ALPHA) {
                _intensityMultiplier = 1;
                alpha = MIN_ALPHA;
            }

            if (alpha >= MAX_ALPHA)
            { 
                _intensityMultiplier = -1;
                alpha = MAX_ALPHA;
            }

            color.a = alpha;
            _blinkingMaterial.color = color;
        }
    }
}
