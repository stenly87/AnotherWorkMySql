using System;

namespace AnotherWorkMySql
{
    internal class DBPrimaryColumnAttribute : DBColumnAttribute
    {
        public DBPrimaryColumnAttribute(string title, bool createByUser = false) : base (title)
        {
            CreateByUser = createByUser;
        }

        public bool CreateByUser { get; }
    }
}