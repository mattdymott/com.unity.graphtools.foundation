using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.GraphToolsFoundation.Overdrive.Model
{
    public interface IHasTitle
    {
        string Title { get; set; }
        string DisplayTitle { get; }
    }

    public interface IHasPorts
    {
        IEnumerable<IGTFPortModel> Ports { get; }
    }

    public interface IHasSingleInputPort : IHasPorts
    {
        IGTFPortModel InputPort { get; }
    }

    public interface IHasSingleOutputPort : IHasPorts
    {
        IGTFPortModel OutputPort { get; }
    }

    public interface IHasIOPorts : IHasPorts
    {
        IEnumerable<IGTFPortModel> InputPorts { get; }
        IEnumerable<IGTFPortModel> OutputPorts { get; }
    }

    public interface IHasProgress
    {
        bool HasProgress { get; }
    }

    public interface ISelectable
    {
    }

    public interface ICollapsible
    {
        bool Collapsed { get; set;  }
    }

    public interface IResizable
    {
        Rect PositionAndSize { get; set; }
        bool IsResizable { get; }
    }

    public interface IPositioned
    {
        Vector2 Position { get; set; }
        void Move(Vector2 delta);
    }

    public interface ICopiable
    {
        bool IsCopiable { get; }
    }

    public interface IDeletable
    {
        bool IsDeletable { get; }
    }

    public interface IDroppable
    {
        bool IsDroppable { get; }
    }

    public interface IRenamable
    {
        bool IsRenamable { get; }
        void Rename(string newName);
    }

    public interface IGhostEdge
    {
        Vector2 EndPoint { get; }
    }
}
