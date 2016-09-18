
namespace Engine.Mechanics.Triggers.Conditions
{
    public interface ICondition : ITriggerItem
    {
        bool Check(EventParams eventParams);

        object Clone();
    }
}
