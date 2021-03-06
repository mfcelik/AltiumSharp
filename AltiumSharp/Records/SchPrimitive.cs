﻿using System;
using System.Collections.Generic;
using System.Linq;
using AltiumSharp.BasicTypes;

namespace AltiumSharp.Records
{
    public class SchPrimitive : Primitive, IContainer
    {
        public int Record { get; internal set; }

        public bool IsNotAccesible { get; internal set; }

        public int OwnerIndex { get; internal set; } = -1;

        public string UniqueId { get; internal set; }

        public int IndexInSheet { get; internal set; } = -1;

        public int CurrentPartId { get; internal set; } = -1;

        public int OwnerPartId { get; internal set; } = -1;

        public int OwnerPartDisplayMode { get; internal set; }

        public bool GraphicallyLocked { get; internal set; }

        public List<SchPrimitive> Primitives { get; } = new List<SchPrimitive>();

        public IEnumerable<T> GetPrimitivesOfType<T>(bool flatten = true) where T : Primitive
        {
            if (flatten)
            {
                return Enumerable.Concat(
                    GetPrimitivesOfType<T>(false),
                    Primitives.SelectMany(p => p.GetPrimitivesOfType<T>(true)));
            }
            else
            {
                return Primitives.OfType<T>();
            }
        }

        public override CoordRect CalculateBounds() => CoordRect.Empty;

        public override bool IsVisible => base.IsVisible && ((Owner as SchComponent)?.DisplayMode ?? 0) == OwnerPartDisplayMode;

        public virtual void ImportFromParameters(ParameterCollection p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            Record = p["RECORD"].AsIntOrDefault();
            IsNotAccesible = p["ISNOTACCESIBLE"].AsBool();
            OwnerIndex = p["OWNERINDEX"].AsIntOrDefault(-1);
            UniqueId = p["UNIQUEID"].AsStringOrDefault();
            IndexInSheet = p["INDEXINSHEET"].AsIntOrDefault(-1);
            OwnerPartId = p["OWNERPARTID"].AsIntOrDefault(-1);
            OwnerPartDisplayMode = p["OWNERPARTDISPLAYMODE"].AsIntOrDefault();
            GraphicallyLocked = p["GRAPHICALLYLOCKED"].AsBool();
        }

        public virtual void ExportToParameters(ParameterCollection p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            p.Add("RECORD", Record);
            p.Add("ISNOTACCESIBLE", IsNotAccesible);
            p.Add("OWNERINDEX", OwnerIndex);
            p.Add("UNIQUEID", UniqueId);
            p.Add("INDEXINSHEET", IndexInSheet);
            p.Add("OWNERPARTID", OwnerPartId);
            p.Add("OWNERPARTDISPLAYMODE", OwnerPartDisplayMode);
            p.Add("GRAPHICALLYLOCKED", GraphicallyLocked);
        }
    }
}