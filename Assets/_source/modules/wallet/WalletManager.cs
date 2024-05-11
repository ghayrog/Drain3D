using Game;
using System;
using UnityEngine;

namespace Wallet
{
    [Serializable]
    public sealed class WalletManager:
        IGameStartListener
    {
        [SerializeField]
        private long _initialAmount = 0;

        public delegate void OnChangeAmountHandler();
        public event OnChangeAmountHandler OnChangeAmount;

        private long _currentAmount;
        public long CurrentAmount
        {
            get
            {
                return _currentAmount;
            }
            private set
            {
                if (value < 0) return;
                var previousAmount = _currentAmount;
                _currentAmount = value;
                if (_currentAmount != previousAmount)
                {
                    OnChangeAmount?.Invoke();
                }
            }
        }

        public LoadingPriority Priority => LoadingPriority.Medium;
            
        public void OnGameStart()
        {
            CurrentAmount = _initialAmount;
        }

        public bool RequestAmount(long amount)
        { 
            if (amount<0 || _currentAmount < amount)
            {
                return false;
            }
            CurrentAmount -= amount;
            return true;
        }

        public bool AddAmount(long amount)
        {
            if (amount < 0)
            {
                return false;
            }
            CurrentAmount+= amount;
            return true;
        }
    }
}
