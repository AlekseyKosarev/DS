using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DS.Examples.UI
{
    public class UIControl : MonoBehaviour
    {
        public GameManager gameManager;

        public void InitClick()
        {
            gameManager.Init();
        }

        public void SaveClick()
        {
            gameManager.SavePlayerData().Forget();
        }

        public void UpgradeLevelClick()
        {
            gameManager.UpgradePlayerData();
        }
    }
}