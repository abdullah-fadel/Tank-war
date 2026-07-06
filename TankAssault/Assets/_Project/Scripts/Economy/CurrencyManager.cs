using TankAssault.Core;

namespace TankAssault.Economy
{
    /// <summary>Soft (Coins) and hard (Gems) currency wallet, persisted through SaveSystem.</summary>
    public class CurrencyManager : Singleton<CurrencyManager>
    {
        public int Coins => SaveSystem.Instance.Current.Coins;
        public int Gems => SaveSystem.Instance.Current.Gems;

        public void AddCoins(int amount)
        {
            if (amount == 0) return;
            SaveSystem.Instance.Current.Coins += amount;
            Publish();
        }

        public bool TrySpendCoins(int amount)
        {
            if (amount <= 0) return true;
            if (SaveSystem.Instance.Current.Coins < amount) return false;
            SaveSystem.Instance.Current.Coins -= amount;
            Publish();
            return true;
        }

        public void AddGems(int amount)
        {
            if (amount == 0) return;
            SaveSystem.Instance.Current.Gems += amount;
            Publish();
        }

        public bool TrySpendGems(int amount)
        {
            if (amount <= 0) return true;
            if (SaveSystem.Instance.Current.Gems < amount) return false;
            SaveSystem.Instance.Current.Gems -= amount;
            Publish();
            return true;
        }

        private void Publish()
        {
            EventBus.Publish(new CurrencyChangedEvent(Coins, Gems));
        }
    }
}
