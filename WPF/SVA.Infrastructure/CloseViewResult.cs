namespace SVA.Infrastructure
{
    public class CloseViewResult
    {
        /// <summary>
        /// Была ли закрыта VM. Null значит, что запроса на закрытие не было вообще.
        /// </summary>
        public bool? CloseCanceled { get; set; }
    }
}
