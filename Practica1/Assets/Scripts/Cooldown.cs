using UnityEngine;

[System.Serializable]
public class Cooldown
{
    #region Variables

    [SerializeField] private float cooldownTime;
    private float _nextFireTime;

    #endregion

    public bool IsCoolingDown => Time.time < _nextFireTime;
    public void StartCooldown() => _nextFireTime = Time.time + cooldownTime;
}
//Com utilitzar: crear variable Cooldown Serialitzable. Es es podra canviar el valor de cooldownTime.
//Es podra accedir a la variable bool isCoolingDown de la classe Cooldown
//O també la funció StartCooldown()