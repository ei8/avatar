using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Avatar.Domain.Model
{
    public class Resource
    {
        [PrimaryKey]
        public string Path { get; set; }

        public string InUri { get; set; }

        public string OutUri { get; set; }
    }
}
