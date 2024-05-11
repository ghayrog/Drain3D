using DI;
using Game;
using UnityEngine;

namespace Wallet
{
    internal sealed class WalletModuleInstaller : GameModuleInstaller
    {
        [SerializeField,Listener,Service(typeof(WalletManager))]
        private WalletManager _moneyManager;

        [SerializeField,Listener]
        private WalletView _moneyView;
    }
}
