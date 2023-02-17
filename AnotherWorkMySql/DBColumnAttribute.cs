using System;

namespace AnotherWorkMySql
{
    internal class DBColumnAttribute : Attribute
    {
        public DBColumnAttribute(string title)
        {
            Title = title;
        }

        public string Title { get; }
    }
}