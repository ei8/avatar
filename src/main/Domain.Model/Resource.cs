using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Avatar.Domain.Model
{
    public class Resource
    {
        [PrimaryKey]
        public string PathPattern { get; set; }

        public string InUri { get; set; }

        public string OutUri { get; set; }

        public string Methods { get; set; }
    }
}
