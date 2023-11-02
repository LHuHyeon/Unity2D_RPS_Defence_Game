using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashProjectile : Projectile
{
    protected override void TouchAbility()
    {
        if ((_stat is WizardStat) == false)
            return;
    }
}
