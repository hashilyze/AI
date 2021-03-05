using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEditor;

namespace AI.BT {
    public class BTCompiler : MonoBehaviour {
        [MenuItem("BT/Test")]
        public static void Test() {
            Compile("Assets/BT/Sample.xml", "Assets/BT/Sample.asset");
        }

        public static void Compile(string scriptaName, string assetName) {
            // Load script file
            XmlDocument doc = new XmlDocument();
            doc.Load(scriptaName);
            
            // Create asset
            List<Behavior> behaviorTable = new List<Behavior>();
            List<int> parnetsTable = new List<int>();
            List<List<int>> childrenTable = new List<List<int>>();
            
            XmlNode root = doc.DocumentElement.FirstChild;
            Stack<XmlNode> stack = new Stack<XmlNode>();
            stack.Push(root);

            while (stack.Count > 0) {
                XmlNode cursor = stack.Pop();

                XmlNodeList children = cursor.ChildNodes;
                for (int beg = children.Count - 1, end = -1; beg != end; --beg) {
                    stack.Push(children[beg]);
                }

                XmlAttributeCollection attributes = cursor.Attributes;
                
            }

            // Save asset to disk
            BehaviorTree bt = ScriptableObject.CreateInstance<BehaviorTree>();
            bt.BehaviorTable = behaviorTable.ToArray();
            bt.ParentsTable = parnetsTable.ToArray();
            bt.ChildrenTable = new int[childrenTable.Count][];
            for(int beg = 0, end = childrenTable.Count; beg != end; ++beg) {
                bt.ChildrenTable[beg] = childrenTable[beg].ToArray();
            }

            AssetDatabase.CreateAsset(bt, assetName);
            AssetDatabase.SaveAssets();
        }
    }
}