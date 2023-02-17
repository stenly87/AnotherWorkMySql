using System;

namespace AnotherWorkMySql
{
    internal class DBTableAttribute : Attribute
    {
        public DBTableAttribute(string title)
        {
            Title = title;
        }

        public string Title { get; }
    }
}