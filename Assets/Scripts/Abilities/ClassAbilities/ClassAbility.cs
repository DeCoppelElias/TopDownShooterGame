using UnityEngine;

public abstract class ClassAbility : ScriptableObject
{
    public int cooldown = 2;
    public abstract bool Initialise(Player player);
    public abstract void PerformAbility(Player player);
}
