using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor.GraphToolsFoundation.Overdrive.GraphElements;
using UnityEditor.GraphToolsFoundation.Overdrive.Tests.GTFO.Helpers;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace UnityEditor.GraphToolsFoundation.Overdrive.Tests.GTFO.UIFromModelTests
{
    public class PortActionTests : BaseTestFixture
    {
        GraphView m_GraphView;
        Helpers.TestStore m_Store;
        GraphModel m_GraphModel;

        [SetUp]
        public new void SetUp()
        {
            m_GraphModel = new GraphModel();
            m_Store = new Helpers.TestStore(new Helpers.TestState(m_GraphModel));
            m_GraphView = new TestGraphView(m_Store);

            m_GraphView.name = "theView";
            m_GraphView.viewDataKey = "theView";
            m_GraphView.StretchToParentSize();

            m_Window.rootVisualElement.Add(m_GraphView);
        }

        [UnityTest]
        public IEnumerator DraggingFromPortCreateGhostEdge()
        {
            var nodeModel = new IONodeModel();
            m_GraphModel.AddNode(nodeModel);
            nodeModel.CreatePorts(0, 1);
            var node = new CollapsibleInOutNode();
            node.SetupBuildAndUpdate(nodeModel, m_Store, m_GraphView);
            m_GraphView.AddElement(node);
            yield return null;

            var portModel = nodeModel.Ports.First();
            var port = portModel.GetUI<Port>(m_GraphView);
            Assert.IsNotNull(port);
            Assert.IsNull(port.EdgeConnector.edgeDragHelper.edgeCandidateModel);

            var portConnector = port.Q(PortConnectorPart.k_ConnectorUssName);
            var clickPosition = portConnector.parent.LocalToWorld(portConnector.layout.center);
            Vector2 move = new Vector2(0, 100);
            ClickDragNoRelease(portConnector, clickPosition, move);
            yield return null;

            // edgeCandidateModel != null is the sign that we have a ghost edge
            Assert.IsNotNull(port.EdgeConnector.edgeDragHelper.edgeCandidateModel);

            ReleaseMouse(portConnector, clickPosition + move);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ReleasingInPortCallsDelegate()
        {
            var nodeModel1 = new IONodeModel { Position = Vector2.zero };
            m_GraphModel.AddNode(nodeModel1);
            nodeModel1.CreatePorts(0, 1);
            var node1 = new CollapsibleInOutNode();
            node1.SetupBuildAndUpdate(nodeModel1, m_Store, m_GraphView);
            m_GraphView.AddElement(node1);

            var nodeModel2 = new IONodeModel { Position = new Vector2(0, 200) };
            m_GraphModel.AddNode(nodeModel2);
            nodeModel2.CreatePorts(1, 0);
            var node2 = new CollapsibleInOutNode();
            node2.SetupBuildAndUpdate(nodeModel2, m_Store, m_GraphView);
            m_GraphView.AddElement(node2);
            yield return null;

            var outPortModel = nodeModel1.Ports.First();
            var outPort = outPortModel.GetUI<Port>(m_GraphView);
            Assert.IsNotNull(outPort);

            var inPortModel = nodeModel2.Ports.First();
            var inPort = inPortModel.GetUI<Port>(m_GraphView);
            Assert.IsNotNull(inPort);

            bool insideOutputPortDelegateCalled = false;
            bool insideInputPortDelegateCalled = false;
            bool outsideOutputPortDelegateCalled = false;
            bool outsideInputPortDelegateCalled = false;

            outPort.EdgeConnector.SetDropDelegate((s, e) => insideOutputPortDelegateCalled = true);
            outPort.EdgeConnector.SetDropOutsideDelegate((s, e, v) => outsideOutputPortDelegateCalled = true);
            inPort.EdgeConnector.SetDropDelegate((s, e) => insideInputPortDelegateCalled = true);
            inPort.EdgeConnector.SetDropOutsideDelegate((s, e, v) => outsideInputPortDelegateCalled = true);

            var outPortConnector = outPort.Q(PortConnectorPart.k_ConnectorUssName);
            var inPortConnector = inPort.Q(PortConnectorPart.k_ConnectorUssName);
            var clickPosition = outPortConnector.parent.LocalToWorld(outPortConnector.layout.center);
            var releasePosition = inPortConnector.parent.LocalToWorld(inPortConnector.layout.center);
            Vector2 move = releasePosition - clickPosition;
            ClickDragRelease(outPortConnector, clickPosition, move);
            yield return null;

            Assert.IsFalse(insideInputPortDelegateCalled);
            Assert.IsTrue(insideOutputPortDelegateCalled);
            Assert.IsFalse(outsideInputPortDelegateCalled);
            Assert.IsFalse(outsideOutputPortDelegateCalled);
        }

        [UnityTest]
        public IEnumerator ReleasingOutsidePortCallsDelegate()
        {
            var nodeModel1 = new IONodeModel { Position = Vector2.zero };
            m_GraphModel.AddNode(nodeModel1);
            nodeModel1.CreatePorts(0, 1);
            var node1 = new CollapsibleInOutNode();
            node1.SetupBuildAndUpdate(nodeModel1, m_Store, m_GraphView);
            m_GraphView.AddElement(node1);

            var nodeModel2 = new IONodeModel { Position = new Vector2(0, 200) };
            m_GraphModel.AddNode(nodeModel2);
            nodeModel2.CreatePorts(1, 0);
            var node2 = new CollapsibleInOutNode();
            node2.SetupBuildAndUpdate(nodeModel2, m_Store, m_GraphView);
            m_GraphView.AddElement(node2);
            yield return null;

            var outPortModel = nodeModel1.Ports.First();
            var outPort = outPortModel.GetUI<Port>(m_GraphView);
            Assert.IsNotNull(outPort);

            var inPortModel = nodeModel2.Ports.First();
            var inPort = inPortModel.GetUI<Port>(m_GraphView);
            Assert.IsNotNull(inPort);

            bool insideOutputPortDelegateCalled = false;
            bool insideInputPortDelegateCalled = false;
            bool outsideOutputPortDelegateCalled = false;
            bool outsideInputPortDelegateCalled = false;

            outPort.EdgeConnector.SetDropDelegate((s, e) => insideOutputPortDelegateCalled = true);
            outPort.EdgeConnector.SetDropOutsideDelegate((s, e, v) => outsideOutputPortDelegateCalled = true);
            inPort.EdgeConnector.SetDropDelegate((s, e) => insideInputPortDelegateCalled = true);
            inPort.EdgeConnector.SetDropOutsideDelegate((s, e, v) => outsideInputPortDelegateCalled = true);

            var outPortConnector = outPort.Q(PortConnectorPart.k_ConnectorUssName);
            var inPortConnector = inPort.Q(PortConnectorPart.k_ConnectorUssName);
            var clickPosition = outPortConnector.parent.LocalToWorld(outPortConnector.layout.center);
            var releasePosition = inPortConnector.parent.LocalToWorld(inPortConnector.layout.center);
            Vector2 move = releasePosition - clickPosition + 400 * Vector2.down;
            ClickDragRelease(outPortConnector, clickPosition, move);
            yield return null;

            Assert.IsFalse(insideInputPortDelegateCalled);
            Assert.IsFalse(insideOutputPortDelegateCalled);
            Assert.IsFalse(outsideInputPortDelegateCalled);
            Assert.IsTrue(outsideOutputPortDelegateCalled);
        }
    }
}
