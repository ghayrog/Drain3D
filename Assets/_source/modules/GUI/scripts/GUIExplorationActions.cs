using System;
using UnityEngine;
using UnityEngine.UI;
using DI;
using Land;

namespace GUI
{
    [Serializable]
    internal sealed class GUIExplorationActions
    {
        [SerializeField]
        private Button _seismicButton;

        [SerializeField]
        private Button _wellButton;

        private LandExplorer _landExplorer;

        [Inject]
        private void Construct(LandExplorer landExplorer)
        { 
            _landExplorer = landExplorer;
        }

        internal void OnEnable()
        {
            _seismicButton.onClick.AddListener(DoSeismic);
            _wellButton.onClick.AddListener(DoExplorationWell);
        }

        internal void OnDisable()
        {
            _seismicButton.onClick.RemoveListener(DoSeismic);
            _wellButton.onClick.RemoveListener(DoExplorationWell);
        }

        private void DoSeismic()
        { 
            _landExplorer.Explore();
        }

        private void DoExplorationWell()
        { 
            _landExplorer.Drill();
        }
    }
}
