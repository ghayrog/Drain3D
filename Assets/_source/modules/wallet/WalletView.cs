using DI;
using Game;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Wallet
{
    [Serializable]
    internal sealed class WalletView:
        IGameFinishListener, IGameStartListener
    {
        [SerializeField]
        private Text _walletText;

        private WalletManager _walletManager;

        public LoadingPriority Priority => LoadingPriority.Low;

        [Inject]
        public void Construct(WalletManager walletManager)
        { 
            _walletManager = walletManager;
            Debug.Log("Injected wallet manager into view");
        }

        public void OnGameStart()
        {
            UpdateWalletText();
            _walletManager.OnChangeAmount += UpdateWalletText;
        }

        public void OnGameFinish()
        {
            _walletManager.OnChangeAmount -= UpdateWalletText;
        }

        private void UpdateWalletText()
        { 
            if (_walletText != null)
            {
                _walletText.text = _walletManager.CurrentAmount.ToString();
            }
        }
    }
}
