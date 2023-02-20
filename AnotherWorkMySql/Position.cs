using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnotherWorkMySql
{
    [DBTable("tbl_position")]
    public class Position
    {
        [DBPrimaryColumn("id")]
        public int Id { get; set; }

        [DBColumn("title")]
        public string Title { get; set; }

        [DBColumn("responsibilites")]
        public string Responsibilites { get; set; }
    }

    [DBTable("tbl_table")]
    public class Table
    {
        [DBPrimaryColumn("id")]
        public int Id { get; set; }

        [DBColumn("title")]
        public string Title { get; set; }

        [DBColumn("number")]
        public int Number { get; set; }

        [DBColumn("count")]
        public int Count { get; set; }
    }
}
