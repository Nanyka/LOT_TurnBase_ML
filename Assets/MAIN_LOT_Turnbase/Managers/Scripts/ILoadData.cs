namespace JumpeeIsland
{
    public interface ILoadData
    {
        public void StartUpLoadData<T>(T data);
        public void PlaceNewObject<T>(T data);
    }
}