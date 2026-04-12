using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    private ICardAbility ability;

    private void Awake()
    {
        ability = GetComponent<ICardAbility>();
    }

    public bool HasAbility => ability != null;

    public void TryUse(UnitStateController owner, IDamageable target)
    {
        if (ability == null) return;
        if (!ability.CanUse()) return;

        ability.Use(owner, target);
    }
}