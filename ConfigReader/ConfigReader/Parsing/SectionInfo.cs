﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigReader.Parsing
{
    class SectionInfo
    {
        internal readonly QualifiedSectionName Name;
        internal readonly string DefaultComment;
        internal readonly IEnumerable<OptionInfo> Options;

        internal SectionInfo(QualifiedSectionName name, List<OptionInfo> options,string defaultComment)
        {            
            Name = name;
            Options =new List<OptionInfo>(options);
            DefaultComment = defaultComment;
        }
    }
}
