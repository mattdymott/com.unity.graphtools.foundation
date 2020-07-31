using System;
using System.Collections.Generic;
using UnityEditor.GraphToolsFoundation.Overdrive.Model;
using UnityEngine;

namespace UnityEditor.GraphToolsFoundation.Overdrive.Tests.GTFO.UIFromModelTests
{
    public class PlacematModel : IGTFPlacematModel
    {
        public IGTFGraphModel GraphModel => null;

        GUID m_GUID = GUID.Generate();
        public GUID Guid => m_GUID;
        public IGTFGraphAssetModel AssetModel => GraphModel.AssetModel;

        public void AssignNewGuid()
        {
            m_GUID = GUID.Generate();
        }

        public string Title { get; set; }
        public string DisplayTitle => Title;
        public Vector2 Position { get; set; }
        public void Move(Vector2 delta)
        {
            Position += delta;
        }

        public bool IsDeletable => true;
        public bool IsCopiable => true;
        public bool Collapsed { get; set; }
        public Rect PositionAndSize { get; set; }
        public bool IsResizable => true;
        public bool IsRenamable => true;
        public void Rename(string newName)
        {
            Title = newName;
        }

        public Color Color { get; set; }
        public int ZOrder { get; set; }
        public IEnumerable<IGTFGraphElementModel> HiddenElements { get; set; }
        public bool Destroyed { get; private set; }
        public void Destroy() => Destroyed = true;
    }
}
