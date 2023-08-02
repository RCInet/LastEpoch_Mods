namespace LastEpochMods.OnSceneChanged
{
    public class Ability_Mutator
    {
        private static System.Collections.Generic.List<AbilityMutator> ability_mutators = null;
        public static void Init()
        {
            ability_mutators = new System.Collections.Generic.List<AbilityMutator>();
            foreach (UnityEngine.Object obj in UniverseLib.RuntimeHelper.FindObjectsOfTypeAll(typeof(AbilityMutator)))
            {
                ability_mutators.Add(obj.TryCast<AbilityMutator>());
            }
        }
        public static AbilityMutator GetMutatorFromAbility(Ability ability)
        {
            AbilityMutator mutator = new AbilityMutator();
            foreach (AbilityMutator obj in ability_mutators)
            {
                if (obj.ability == ability)
                {
                    mutator = obj;
                    break;
                }                
            }

            return mutator;
        }
    }
}
