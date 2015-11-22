using System.Data;

namespace Utils.Extensions
{
    public interface IDbConverter
    {
        #region - Read from DB routines -

        object ConvertFromDb(string fieldName, object dbValue);
        void AfterRead(DataRow row);

        #endregion

        #region - Write to DB routines -

        void BeforeWrite();
        object ConvertToDb(string fieldName, object objectValue);

        #endregion
    }
}