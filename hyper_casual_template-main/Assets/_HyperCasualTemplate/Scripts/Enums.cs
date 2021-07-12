namespace HyperCasualTemplate
{
    public static class Enums
    {
        /// <summary>
        /// タッチイベントの種別
        /// </summary>
        public enum TouchInfo
        {
            None = 99,
            Began = 0,
            Moved = 1,
            Stationary = 2,
            Ended = 3,
            Canceled = 4,
        }
    }
}
