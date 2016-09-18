using Engine.Core;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Engine.Mechanics
{
    //public enum Player
    //{
    //    Human, Hostile, Passive
    //}

    //public class Unit
    //{
    //    protected Point _location;
    //    public Sprite Sprite;
    //    public UnitType UnitType { get; protected set; }
    //    public short Life;
    //    public short Damage;
    //    public Player Owner;
    //    public short MovementPoints;
    //    public bool Dead { get; protected set; }
    //    public event SimpleEvent OnDeath;
    //    public event PointEvent LocationChanged;

    //    public Point Location
    //    {
    //        get { return _location; }
    //        set
    //        {
    //            Sprite.Pos = EngineGlobals.Level.Surface.TileToPoint(value);
    //            if (LocationChanged != null)
    //            {
    //                var oldLocation = _location;
    //                _location = value;
    //                LocationChanged(this, oldLocation);
    //            }
    //            else
    //                _location = value;
    //        }
    //    }

    //    public Unit(UnitType type, Player owner, Point location)
    //    {
    //        UnitType = type;
    //        Owner = owner;
    //        _location = location;
    //        MovementPoints = UnitType.BaseMovmentPoints;
    //        var p = EngineGlobals.Level.Surface.TileToPoint(location);
    //        Sprite = new Sprite(UnitType.SpriteData,
    //                            new Rectangle((int) p.X, (int) p.Y, UnitType.UnitSizeWidth, UnitType.UnitSizeHeight),
    //                            "idle", true)
    //                     {
    //                         Owner = this
    //                     };
    //        Life = UnitType.HitPoints;
    //        Damage = UnitType.BaseDamage;
    //    }

    //    public void Die()
    //    {
    //        Sprite.IsHidden = true;
    //        Dead = true;
    //        if (OnDeath != null)
    //            OnDeath(this);
    //    }

    //    public void Draw()
    //    {
    //        Sprite.Draw();
    //    }
    //}

    //public class UnitType
    //{
    //    protected string _spritePath;
    //    public string TypeName;
    //    public short HitPoints;
    //    public short BaseSpeed;
    //    public short UnitSizeWidth;
    //    public short UnitSizeHeight;
    //    public short BaseDamage;
    //    public short BaseMovmentPoints;
    //    [ContentSerializerIgnore]
    //    public SpriteData SpriteData;
    //    public string SpritePath
    //    {
    //        set
    //        {
    //            _spritePath = value;
    //            if (EngineGlobals.Content != null)
    //                SpriteData = EngineGlobals.Content.Load<SpriteData>(_spritePath);
    //        }
    //        get { return _spritePath; }
    //    }
    //}
}
