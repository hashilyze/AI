using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeDesigner {
    [System.Serializable]
    public sealed class Node {
        // Constructors
        public Node() { }
        public Node(Node source) {
            m_data = source.m_data;

            m_ID = source.m_ID;
            m_parentsID = source.m_parentsID;
            m_childrenIDs.AddRange(source.m_childrenIDs);

            m_rect = source.m_rect;
            m_title = source.m_title;
        }

        // Properties
        public Object Data { get => m_data; set => m_data = value; }

        public int ID { get => m_ID; set => m_ID = value; }
        public int ParentsID { get => m_parentsID; set => m_parentsID = value; }
        public List<int> ChildrenIDs { get => m_childrenIDs; set => m_childrenIDs = value; }

        public Rect Rect { get => m_rect; set => m_rect = value; }
        public Vector2 Position { get => m_rect.position; set => m_rect.position = value; }
        public Vector2 Size { get => m_rect.size; set => m_rect.size = value; }
        public string Title { get => m_title; set => m_title = value; }

        // Variables
        [Header("Data")]
        [SerializeField] private Object m_data;
        [Header("Relation")]
        [SerializeField] private int m_ID = TreeUtility.EmptyID;
        [SerializeField] private int m_parentsID = TreeUtility.EmptyID;
        [SerializeField] private List<int> m_childrenIDs = new List<int>();
        [Header("GUI")]
        [SerializeField] private Rect m_rect;
        [SerializeField] private string m_title;
    }
}