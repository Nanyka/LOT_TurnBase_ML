namespace JumpeeIsland
{
    public struct JIItemAndAmountSpec
    {
        public string id;
        public int amount;

        public JIItemAndAmountSpec(string id, int amount)
        {
            this.id = id;
            this.amount = amount;
        }

        public override string ToString()
        {
            return $"{id}:{amount}";
        }
    }
}