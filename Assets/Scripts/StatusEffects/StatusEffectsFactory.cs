using Model.Config;
using Model.Runtime;
using System;

namespace Assets.Scripts.StatusEffects
{
    public class StatusEffectsFactory
    {
        public IStatusEffects<Unit> CreateStatusEffect(UnitConfig unitConfig)
        {
            switch (unitConfig.Type)
            {
                case "DefaultUnit":
                    return new AttackSpeedBuff();
                case "SecondUnit":
                    return new DoubleAttackBuff();
                case "ThirdUnit":
                    return new AttackRangeBuff();
                case "SupportUnit":
                    return new SpeedBuff();
                default:
                    throw new ArgumentException($"Unknown unit type: {unitConfig.Type}");
            }
        }
    }
}
