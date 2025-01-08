namespace CarServiceProject.Model
{
    public class Wallet
    {
        public Wallet(float amount = 0) 
        { 
            Amount = amount;
        }

        public float Amount { get; private set; }

        public void IncreaseAmount(float value) 
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            Amount += value;
        }

        public bool TryWithdrawMoney(float amount) 
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(amount, 0);

            if (amount <= Amount) 
            { 
                Amount -= amount;
                return true;
            }

            return false;
        }
    }
}
