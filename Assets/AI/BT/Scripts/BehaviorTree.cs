using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BT {
    public class BehaviorTree : Behavior {
        public Behavior[] BehaviorTable { get => m_behaviorTable; set => m_behaviorTable = value; }
        public int[] ParentsTable { get => m_parentsTable; set => m_parentsTable = value; }
        public int[][] ChildrenTable { get => m_childrenTable; set => m_childrenTable = value; }

        [SerializeField] private Behavior[] m_behaviorTable;
        [SerializeField] private int[] m_parentsTable;
        [SerializeField] private int[][] m_childrenTable;

        
        public override EState Update() {
            throw new System.NotImplementedException();
        }
    }
}