using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCreateCard : BaseEditCreateCard
{
    [Tooltip("Position of the new issue card to be spawned. Has to be set manually")]
    public Transform spawnPoint;
}
