using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private AuraPulseWeapon auraPulse;
    private OrbitBladeWeapon orbitBlade;

    public bool HasAuraPulse => auraPulse != null && auraPulse.enabled;
    public bool HasOrbitBlade => orbitBlade != null && orbitBlade.enabled;
    public AuraPulseWeapon AuraPulse => auraPulse;
    public OrbitBladeWeapon OrbitBlade => orbitBlade;

    private void Awake()
    {
        auraPulse = GetComponent<AuraPulseWeapon>();
        orbitBlade = GetComponent<OrbitBladeWeapon>();
    }

    public void UnlockAuraPulse()
    {
        if (auraPulse == null)
        {
            auraPulse = gameObject.AddComponent<AuraPulseWeapon>();
        }

        auraPulse.enabled = true;
    }

    public void UnlockOrbitBlade()
    {
        if (orbitBlade == null)
        {
            orbitBlade = gameObject.AddComponent<OrbitBladeWeapon>();
        }

        orbitBlade.enabled = true;
    }
}
