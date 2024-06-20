
namespace Assets.Scripts.StatusEffects
{
    public interface IStatusEffects<T>
    {
        void Apply(T unit);
        void Remove(T unit);
        bool CanBeAppliedTo(T unit);
        float Duration { get; set; }
        void UpdateDuration(T unit, float deltaTime) 
        {
            Duration -= deltaTime;
            if (Duration <= 0)
            {
                Remove(unit);
            }
        }
    }
}
