using UnityEngine;
using Game;

namespace GUI
{
    internal sealed class GUIModuleInstaller : GameModuleInstaller
    {
        [SerializeField]
        private GUIExplorationActions _explorationActions;

        [SerializeField]
        private GUINavigation _mainNavigation;

        private void OnEnable()
        {
            _mainNavigation.OnEnable();
            _explorationActions.OnEnable();
        }

        private void OnDisable()
        {
            _mainNavigation.OnDisable();
            _explorationActions.OnDisable();
        }
    }
}
