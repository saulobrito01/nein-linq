﻿using System.Collections.Generic;

namespace NeinLinq.Fakes.SelectorTranslator
{
    public class ParentDummy : IDummy
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public IEnumerable<ChildDummy>? Children { get; set; }
    }
}
