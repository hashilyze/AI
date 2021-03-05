using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TreeDesigner {
    public class BehaviorTreeDesignerWindow : EditorWindow {
        #region Constants
        // Window title
        public const string Title = "Behavior Tree";
        // Context menu names on BlackBoard

        // Context menu names on Node
        private const string Name_CreateEmptyChild = "Create Empty Child";
        private const string Name_DeleteThis = "Delete This";
        private const string Name_DeleteThisAllChildren = "Delete All Children";
        // Message
        private const string Msg_NoExistAsset = "No Exsit Asset";
        #endregion
        #region Variables
        private TreeAsset m_selectedTree;
        private string m_selectedPath;

        private Vector2 m_scrollPos;
        private int m_foucedID = TreeUtility.EmptyID;
        #endregion

        #region Activation
        [MenuItem(TreeUtility.Namespace + "/" + Title)]
        public static void ShowWindow() => GetWindow<BehaviorTreeDesignerWindow>(Title, typeof(SceneView));

        private void OnFocus() { UpdateSelection(); }
        private void OnSelectionChange() { UpdateSelection(); }
        private void OnEnable() { UpdateSelection(); } 
        private void OnDisable() { SaveAsset(); }
        private void OnDestroy() { SaveAsset(); }

        private void UpdateSelection() {
            if (Selection.activeObject is TreeAsset newTree && newTree != m_selectedTree) {
                Select(newTree);
                Repaint();
            }
        }
        private void Select(TreeAsset tree) {
            SaveAsset(); // Before change the selected tree, save current one
            m_selectedTree = tree;
            m_selectedPath = AssetDatabase.GetAssetPath(m_selectedTree);
        }
        private void SaveAsset() {
            if (m_selectedTree == null) return;
            EditorUtility.SetDirty(m_selectedTree);
        }
        #endregion

        #region Draw GUI
        private void OnGUI() {
            if (m_selectedTree != null) {
                DrawTree();
                DrawLog(m_selectedPath);
                EventHandleOnBlackBoard();
            } else {
                DrawEmptyWindow();
            }
        }
        private void DrawTree() {
            using (new EditorGUILayout.HorizontalScope()) {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(m_scrollPos)) {
                    m_scrollPos = scrollView.scrollPosition;

                    BeginWindows();
                    // Draw nodes & connections
                    for (int beg = 0, end = m_selectedTree.Count; beg != end; ++beg) {
                        Node node = m_selectedTree.GetNode(beg);
                        Rect newRect = GUI.Window(node.ID, node.Rect, DrawNode, node.Title);
                        UpdateNodePosition(node, newRect.position);
                    }
                    for (int beg = 1, end = m_selectedTree.Count; beg != end; ++beg) {
                        Node node = m_selectedTree.GetNode(beg);
                        DrawConnection(m_selectedTree.GetNode(node.ParentsID).Rect, node.Rect);
                    }
                    EndWindows();
                    // Update window size
                    Rect treeRect = GetTreeRect(m_selectedTree.GetNode(0));
                    Vector2 windowSize = treeRect.size + treeRect.position;
                    GUILayoutUtility.GetRect(windowSize.x, windowSize.y);
                }
            }
        }
        private void DrawNode(int id) {
            EventHandleOnNode(id);

            GUI.DragWindow();
        }
        private void DrawEntry(int id) {
            GUI.DragWindow();
        }
        private static void DrawConnection(Rect parentsRect, Rect childRect) {
            Vector2 parentsSize = parentsRect.size, childSize = childRect.size;
            Vector2 parentsPos = parentsRect.position, childPos = childRect.position;

            // Start position is bottom middle of parents
            Vector2 startPos = new Vector2(parentsPos.x + parentsSize.x * 0.5f, parentsPos.y + parentsSize.y);
            // End position is top middle of child
            Vector2 endPos = new Vector2(childPos.x + childSize.x * 0.5f, childPos.y);

            float tangent = (startPos.y + endPos.y) * 0.5f;
            Vector2 startTangent = new Vector2(startPos.x, tangent);
            Vector2 endTangent = new Vector2(endPos.x, tangent);

            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Color.white, null, 3);
        }
        private static void DrawLog(string message) {
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                GUILayout.Label(message);
            }
        }
        private static void DrawEmptyWindow() {
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.VerticalScope()) {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(Msg_NoExistAsset);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.FlexibleSpace();
            }
        }
        #endregion

        #region Rect Handle
        private static void UpdateNodePosition(Node node, Vector2 position) {
            if (Event.current.button != 0) return;

            if (position.x < 0f) position.x = 0f;
            if (position.y < 0f) position.y = 0f;
            node.Position = position;
        }
        private Rect GetTreeRect(Node root) {
            Vector2 topLeft = root.Position;
            Vector2 bottomRight = topLeft + root.Size;

            if (root.ID != 0) { // Query range
                List<int> childrenIDs = root.ChildrenIDs;
                for (int beg = 0, end = childrenIDs.Count; beg != end; ++beg) {
                    Rect newRect = GetTreeRect(m_selectedTree.GetNode(childrenIDs[beg]));
                    UpdateMaxRext(ref topLeft, ref bottomRight, newRect.position, newRect.position + newRect.size);
                }
            } else { // Query all
                for (int beg = 0, end = m_selectedTree.Count; beg != end; ++beg) {
                    Rect newRect = m_selectedTree.GetNode(beg).Rect;
                    UpdateMaxRext(ref topLeft, ref bottomRight, newRect.position, newRect.position + newRect.size);
                }
            }
            return new Rect(topLeft, bottomRight - topLeft);
        }
        private void UpdateMaxRext(ref Vector2 topLeft, ref Vector2 bottomRight, Vector2 newTopLeft, Vector2 newBottomRight) {
            if (newTopLeft.x < topLeft.x) topLeft.x = newTopLeft.x;
            if (newTopLeft.y < topLeft.y) topLeft.y = newTopLeft.y;
            if (bottomRight.x < newBottomRight.x) bottomRight.x = newBottomRight.x;
            if (bottomRight.y < newBottomRight.y) bottomRight.y = newBottomRight.y;
        }
        #endregion

        #region Event Handle
        bool isDarg;
        Vector2 startPos;
        Vector2 endPos;
        private void EventHandleOnBlackBoard() {
            Event evt = Event.current;
            EventType type = evt.type;

            if (type == EventType.MouseDown) { // Click black board
                m_foucedID = TreeUtility.EmptyID;

                // Open Generic menu
                if (Event.current.button == 1) {
                    DrawContextMenuOnBlackBoard();
                }
                
                evt.Use();
                Repaint();
            }
            // Draw drag box
            if (type == EventType.MouseDown) { // Click black board
                startPos = endPos = evt.mousePosition;
                isDarg = true;
            } else if (type == EventType.MouseDrag) {
                endPos = evt.mousePosition;
                Repaint();
            } else if (type == EventType.MouseUp) {
                isDarg = false;
                Repaint();
            } else if (type == EventType.Layout || type == EventType.Repaint) {
                if (isDarg) {
                    Vector2 center = (startPos + endPos) * 0.5f;
                    Vector2 size = new Vector2(Mathf.Abs(startPos.x - endPos.x), Mathf.Abs(startPos.y - endPos.y));
                    Handles.DrawWireCube(center, size);
                }
            }

        }
        private void EventHandleOnNode(int id) {
            Event evt = Event.current;
            EventType type = evt.type;

            if (type == EventType.MouseDown) { // Click node
                m_foucedID = id;
                if (Event.current.button == 1) {
                    DrawContextMenuForNode(id);
                }
            } else if ((type == EventType.MouseUp || (type == EventType.Ignore && Event.current.rawType == EventType.MouseUp)) && Event.current.button == 0) {
                SortSiblings(id);
            } else if (type == EventType.DragUpdated || type == EventType.DragPerform) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                DragAndDrop.AcceptDrag();
                if (type == EventType.DragPerform) {
                    m_selectedTree.GetNode(id).Data = DragAndDrop.objectReferences[0];
                    DragAndDrop.PrepareStartDrag();
                }
            }
        }

        private void DrawContextMenuOnBlackBoard() { }

        private void DrawContextMenuForNode(int focusID) {
            GenericMenu menu = new GenericMenu();
            if(focusID == TreeUtility.EntryID) {
                if(m_selectedTree.GetNode(focusID).ChildrenIDs.Count < 1) { // Entry has only one child
                    menu.AddItem(new GUIContent(Name_CreateEmptyChild), false, () => CreateEmptyChild(focusID));
                }
                menu.AddItem(new GUIContent(Name_DeleteThisAllChildren), false, () => DeleteAllChildren(focusID));
            } else {
                menu.AddItem(new GUIContent(Name_CreateEmptyChild), false, () => CreateEmptyChild(focusID));
                menu.AddItem(new GUIContent(Name_DeleteThis), false, () => DeleteThis(focusID));
                menu.AddItem(new GUIContent(Name_DeleteThisAllChildren), false, () => DeleteAllChildren(focusID));
            }
            menu.ShowAsContext();
        }
        #endregion

        #region Edit Tree
        // Context menu functions on BlackBoard
        private void SortSiblings(int targetID) {
            if (targetID == TreeUtility.EntryID) return;
            m_selectedTree.SortChildren(m_selectedTree.GetNode(targetID).ParentsID);
            SaveAsset();
        }
        // Context menu functions on Node
        private void CreateEmptyChild(int targetID) {
            Node parentsNode = m_selectedTree.GetNode(targetID);
            List<int> childrenIDs = parentsNode.ChildrenIDs;

            Vector2 position;
            if (childrenIDs.Count > 0) { // Create side sibling
                Node lastSibling = m_selectedTree.GetNode(childrenIDs[childrenIDs.Count - 1]);
                position = lastSibling.Position + lastSibling.Size.x * Vector2.right;
            } else { // Create below parents
                position = parentsNode.Position + parentsNode.Size.y * Vector2.up;
            }
            m_selectedTree.AddNode(null, position, targetID);

            SaveAsset();
        }
        private void DeleteThis(int targetID) {
            m_selectedTree.RemoveNode(targetID);
            m_foucedID = TreeUtility.EmptyID;
            SaveAsset();
        }
        private void DeleteAllChildren(int targetID) {
            Node parents = m_selectedTree.GetNode(targetID);
            List<int> childrenIDs = parents.ChildrenIDs;
            while (childrenIDs.Count > 0) {
                m_selectedTree.RemoveNode(childrenIDs[0]);
            }
            SaveAsset();
        }
        #endregion
    }
}