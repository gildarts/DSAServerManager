﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevComponents.AdvTree;

namespace DSAServerManager
{
    public class KeyNode : Node
    {
        public KeyNode()
            : base()
        {
            Catalog = null;
        }

        public KeyNode(string text)
            : base(text)
        {
            Catalog = null;
        }

        public KeyCatalog Catalog { get; set; }

        ~KeyNode()
        {
            if (Catalog != null)
                Catalog.Clear();
        }
    }
}