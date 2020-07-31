using System.Collections;
using NUnit.Framework;
using UnityEditor.GraphToolsFoundation.Overdrive.GraphElements;
using UnityEditor.GraphToolsFoundation.Overdrive.Model;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.GraphToolsFoundation.Overdrive.Tests.GraphElements.Utilities
{
    public class GraphViewSnappingTester : GraphViewTester
    {
        protected const float k_SnapDistance = 8.0f;

        protected Vector2 m_ReferenceNode1Pos;
        protected Vector2 m_ReferenceNode2Pos;
        protected Vector2 m_SnappingNodePos;
        protected Vector2 m_SelectionOffset = new Vector2(25, 25);

        protected BasicNodeModel snappingNodeModel { get; set; }
        protected BasicNodeModel referenceNode1Model { get; set; }
        protected BasicNodeModel referenceNode2Model { get; set; }

        protected IGTFPortModel m_InputPort;
        protected IGTFPortModel m_OutputPort;

        protected Node m_SnappedNode;
        protected Node m_ReferenceNode1;
        protected Node m_ReferenceNode2;

        protected static void SetUINodeSize(ref Node node, float height, float width)
        {
            node.style.height = height;
            node.style.width = width;
        }

        protected IEnumerator UpdateUINodeSizes(Vector2 snappedNodeSize, Vector2 referenceNode1Size, Vector2 referenceNode2Size = default)
        {
            // Changing the nodes' sizes to make it easier to test the snapping
            SetUINodeSize(ref m_SnappedNode, snappedNodeSize.y, snappedNodeSize.x);
            SetUINodeSize(ref m_ReferenceNode1, referenceNode1Size.y, referenceNode1Size.x);
            SetUINodeSize(ref m_ReferenceNode2, referenceNode2Size.y, referenceNode2Size.x);

            yield return null;
        }

        protected IEnumerator SetUpUIElements(Vector2 snappingNodePos, Vector2 referenceNode1Pos = default,  Vector2 referenceNode2Pos = default, bool isVerticalPort = false, bool isPortSnapping = false)
        {
            m_SnappingNodePos = snappingNodePos;
            m_ReferenceNode1Pos = referenceNode1Pos;
            m_ReferenceNode2Pos = referenceNode2Pos;

            snappingNodeModel = CreateNode("Snapping Node", m_SnappingNodePos);
            referenceNode1Model = CreateNode("Reference Node 1", m_ReferenceNode1Pos);
            referenceNode2Model = CreateNode("Reference Node 2", m_ReferenceNode2Pos);

            if (isPortSnapping)
            {
                if (isVerticalPort)
                {
                    // Add a vertical port on snapping node and reference node 1
                    m_InputPort = snappingNodeModel.AddPort(Orientation.Vertical, Direction.Input, PortCapacity.Single, typeof(float));
                    m_OutputPort = referenceNode1Model.AddPort(Orientation.Vertical, Direction.Output, PortCapacity.Single, typeof(float));
                    Assert.IsNotNull(m_OutputPort);
                    Assert.IsNotNull(m_InputPort);
                }
                else
                {
                    // Add a horizontal port on snapping node and reference node 1
                    m_InputPort = snappingNodeModel.AddPort(Orientation.Horizontal, Direction.Input, PortCapacity.Single, typeof(float));
                    m_OutputPort = referenceNode1Model.AddPort(Orientation.Horizontal, Direction.Output, PortCapacity.Single, typeof(float));
                    Assert.IsNotNull(m_OutputPort);
                    Assert.IsNotNull(m_InputPort);
                }

                graphView.RebuildUI(GraphModel, Store);
                yield return null;

                // Connect the ports together
                var actions = ConnectPorts(m_OutputPort, m_InputPort);
                while (actions.MoveNext())
                {
                    yield return null;
                }
            }
            graphView.RebuildUI(GraphModel, Store);
            yield return null;

            // Get the UI nodes
            m_SnappedNode = snappingNodeModel.GetUI<Node>(graphView);
            m_ReferenceNode1 = referenceNode1Model.GetUI<Node>(graphView);
            m_ReferenceNode2 = referenceNode2Model.GetUI<Node>(graphView);
            Assert.IsNotNull(m_SnappedNode);
            Assert.IsNotNull(m_ReferenceNode1);
            Assert.IsNotNull(m_ReferenceNode2);
        }

        protected IEnumerator MoveElementWithOffset(Vector2 offset)
        {
            Vector2 worldNodePos = graphView.contentViewContainer.LocalToWorld(m_SnappingNodePos);
            Vector2 start = worldNodePos + m_SelectionOffset;

            // Move the snapping node.
            helpers.MouseDownEvent(start);
            yield return null;

            Vector2 end = start + offset;
            helpers.MouseDragEvent(start, end);
            yield return null;

            helpers.MouseUpEvent(end);
            yield return null;
        }
    }
}
