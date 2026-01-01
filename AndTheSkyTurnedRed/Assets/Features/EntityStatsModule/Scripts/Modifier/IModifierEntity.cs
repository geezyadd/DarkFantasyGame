using System;

namespace Features.EntityStatsModule.Scripts.Modifier {
    public interface IModifierEntity<in TStatEnum> where TStatEnum : Enum {
        public void AddModifier(TStatEnum statType, StatModifier statModifier);
        public void RemoveModifier(TStatEnum statType, StatModifier statModifier);
        public void RemoveModifierThatEqual(TStatEnum statType, StatModifier statModifier);
        public void RemoveAllModifiers();
    }
}
