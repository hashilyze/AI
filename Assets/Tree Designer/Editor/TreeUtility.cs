using UnityEngine;

namespace TreeDesigner {
    public static class TreeUtility {
        public const string Namespace = "TreeDesigner";

        // Predefined nodes
        public const int ReserveCount = 1;

        public const int EmptyID = -1;
        public const int EntryID = 0;
        public const string EmptyName = "Empty";
        public const string EntryName = "Entry";
        

        public static Node CreateEmptyNode() => new Node(m_emptyNodeTemplate);
        public static Node CreateEntryNode() => new Node(m_entryNodeTemplate);

        private static readonly Node m_emptyNodeTemplate = new Node() { ID = EmptyID, Title = EmptyName, Size = Vector2.one * 50f };
        private static readonly Node m_entryNodeTemplate = new Node() { ID = EntryID, Title = EntryName, Size = Vector2.one * 50f };

        public static readonly Vector2 DefaultNodeSize = new Vector2(50f, 50f);
    }
}