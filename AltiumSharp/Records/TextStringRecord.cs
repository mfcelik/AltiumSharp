using System;
using System.Drawing;
using System.Linq;
using AltiumSharp.BasicTypes;

namespace AltiumSharp.Records
{
    public struct TextJustification: IEquatable<TextJustification>
    {
        public static readonly TextJustification BottomLeft = new TextJustification(0);
        public static readonly TextJustification BottomCenter = new TextJustification(1);
        public static readonly TextJustification BottomRight = new TextJustification(2);
        public static readonly TextJustification MiddleLeft = new TextJustification(3);
        public static readonly TextJustification MiddleCenter = new TextJustification(4);
        public static readonly TextJustification MiddleRight = new TextJustification(5);
        public static readonly TextJustification TopLeft = new TextJustification(6);
        public static readonly TextJustification TopCenter = new TextJustification(7);
        public static readonly TextJustification TopRight = new TextJustification(8);

        public enum HorizontalValue { Left, Center, Right }
        public enum VerticalValue { Bottom, Middle, Top }

        private readonly int _value;

        public TextJustification(int value) => _value = value;

        public TextJustification(HorizontalValue horizontal, VerticalValue vertical) =>
            _value = ((int)vertical * 3) + (int)horizontal;

        public HorizontalValue Horizontal =>  (HorizontalValue)(_value % 3);

        public VerticalValue Vertical => (VerticalValue)(_value / 3);

        public int ToInt32() => _value;

        public static TextJustification FromInt32(int value) => new TextJustification(value);

        static public explicit operator TextJustification(int value) => FromInt32(value);

        static public explicit operator int(TextJustification textJustification) => textJustification.ToInt32();

        #region 'boilerplate'
        public override bool Equals(object obj) => obj is TextJustification other && Equals(other);
        public bool Equals(TextJustification other) => _value == other._value;
        public override int GetHashCode() => _value.GetHashCode();
        public static bool operator ==(TextJustification left, TextJustification right) => left.Equals(right);
        public static bool operator !=(TextJustification left, TextJustification right) => !left.Equals(right);
        #endregion
    }

    [Flags]
    public enum TextOrientations
    {
        Rotated = 1, Flipped = 2
    }

    public class TextStringRecord : SchPrimitive
    {
        public CoordPoint Location { get; internal set; }
        public Color Color { get; internal set; }
        public TextJustification Justification { get; internal set; }
        public TextOrientations Orientations { get; internal set; }
        public int FontId { get; internal set; }
        public string Text { get; internal set; }
        public bool IsMirrored { get; internal set; }
        public bool IsHidden { get; internal set; }

        internal virtual string DisplayText => Text ?? "";
        public override CoordRect CalculateBounds() =>
            new CoordRect(Location.X, Location.Y, 1, 1);
        public override bool IsVisible => base.IsVisible && !IsHidden;

        public override void ImportFromParameters(ParameterCollection p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            base.ImportFromParameters(p);
            Location = new CoordPoint(
                Utils.DxpFracToCoord(p["LOCATION.X"].AsIntOrDefault(), p["LOCATION.X_FRAC"].AsIntOrDefault()),
                Utils.DxpFracToCoord(p["LOCATION.Y"].AsIntOrDefault(), p["LOCATION.Y_FRAC"].AsIntOrDefault()));
            Color = p["COLOR"].AsColorOrDefault();
            Justification = (TextJustification)p["JUSTIFICATION"].AsIntOrDefault();
            Orientations = (TextOrientations)p["ORIENTATION"].AsIntOrDefault();
            FontId = p["FONTID"].AsIntOrDefault();
            Text = p["TEXT"].AsStringOrDefault();
            IsMirrored = p["ISMIRRORED"].AsBool();
            IsHidden = p["ISHIDDEN"].AsBool();
        }

        public override void ExportToParameters(ParameterCollection p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            base.ExportToParameters(p);
            {
                var (n, f) = Utils.CoordToDxpFrac(Location.X);
                if (n != 0 || f != 0) p.Add("LOCATION.X", n);
                if (f != 0) p.Add("LOCATION.X" + "_FRAC", f);
            }
            {
                var (n, f) = Utils.CoordToDxpFrac(Location.Y);
                if (n != 0 || f != 0) p.Add("LOCATION.Y", n);
                if (f != 0) p.Add("LOCATION.Y" + "_FRAC", f);
            }
            p.Add("COLOR", Color);
            p.Add("JUSTIFICATION", (int)Justification);
            p.Add("ORIENTATION", (int)Orientations);
            p.Add("FONTID", FontId);
            p.Add("TEXT", Text);
            p.Add("ISMIRRORED", IsMirrored);
            p.Add("ISHIDDEN", IsHidden);
        }
    }
}
